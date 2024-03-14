// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity.Sample.HandTracking
{
  public class HandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
  {
    [SerializeField] private DetectionListAnnotationController _palmDetectionsAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromPalmDetectionsAnnotationController;
    [SerializeField] private MultiHandLandmarkListAnnotationController _handLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromLandmarksAnnotationController;

    // public bool IsHandClosed = false;

    public HandTrackingGraph.ModelComplexity modelComplexity
    {
      get => graphRunner.modelComplexity;
      set => graphRunner.modelComplexity = value;
    }

    public int maxNumHands
    {
      get => graphRunner.maxNumHands;
      set => graphRunner.maxNumHands = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

protected override void OnStartRun()
{
    graphRunner.OnPalmDetectectionsOutput += OnPalmDetectionsOutput;
    graphRunner.OnHandRectsFromPalmDetectionsOutput += OnHandRectsFromPalmDetectionsOutput;
    graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
    // TODO: render HandWorldLandmarks annotations
    graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
    graphRunner.OnHandednessOutput += OnHandednessOutput;

    var imageSource = ImageSourceProvider.ImageSource;
    SetupAnnotationController(_palmDetectionsAnnotationController, imageSource, true);
    SetupAnnotationController(_handRectsFromPalmDetectionsAnnotationController, imageSource, true);
    SetupAnnotationController(_handLandmarksAnnotationController, imageSource, true);
    SetupAnnotationController(_handRectsFromLandmarksAnnotationController, imageSource, true);
}


    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      var task = graphRunner.WaitNext();
      yield return new WaitUntil(() => task.IsCompleted);

      var result = task.Result;
      _palmDetectionsAnnotationController.DrawNow(result.palmDetections);
      _handRectsFromPalmDetectionsAnnotationController.DrawNow(result.handRectsFromPalmDetections);
      _handLandmarksAnnotationController.DrawNow(result.handLandmarks, result.handedness);
      // TODO: render HandWorldLandmarks annotations
      _handRectsFromLandmarksAnnotationController.DrawNow(result.handRectsFromLandmarks);
    }

    private void OnPalmDetectionsOutput(object stream, OutputStream<List<Detection>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(Detection.Parser);
      _palmDetectionsAnnotationController.DrawLater(value);
    }

    private void OnHandRectsFromPalmDetectionsOutput(object stream, OutputStream<List<NormalizedRect>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(NormalizedRect.Parser);
      _handRectsFromPalmDetectionsAnnotationController.DrawLater(value);
    }

public bool LeftTouching;

private void OnHandLandmarksOutput(object stream, OutputStream<List<NormalizedLandmarkList>>.OutputEventArgs eventArgs)
{
    var packet = eventArgs.packet;
    var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);

    if (value != null && value.Count > 0 && value[0].Landmark.Count >= 21)
    {
        // Log the landmarks for debugging
        for (int i = 0; i < value[0].Landmark.Count; i++)
        {
            var landmark = value[0].Landmark[i];
            Debug.Log($"Landmark {i}: ({landmark.X}, {landmark.Y}, {landmark.Z})");
        }

        // Thumb tip is at index 4
        Vector3 thumbTipPosition = new Vector3(value[0].Landmark[4].X, value[0].Landmark[4].Y, value[0].Landmark[4].Z);

        // Index finger tip is at index 8
        Vector3 indexTipPosition = new Vector3(value[0].Landmark[8].X, value[0].Landmark[8].Y, value[0].Landmark[8].Z);

        // Check if thumb tip touches index finger tip
        if (Vector3.Distance(thumbTipPosition, indexTipPosition) < 0.05f)
        {
            LeftTouching = true;
            Debug.Log("Left is touching");
        }
        else
        {
            LeftTouching = false;
        }
    }

    _handLandmarksAnnotationController.DrawLater(value);
}


    private void OnHandRectsFromLandmarksOutput(object stream, OutputStream<List<NormalizedRect>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(NormalizedRect.Parser);
      _handRectsFromLandmarksAnnotationController.DrawLater(value);
    }

    private void OnHandednessOutput(object stream, OutputStream<List<ClassificationList>>.OutputEventArgs eventArgs)
    {
      var packet = eventArgs.packet;
      var value = packet == null ? default : packet.Get(ClassificationList.Parser);
      _handLandmarksAnnotationController.DrawLater(value);
    }
  }
}