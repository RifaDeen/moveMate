using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using NUnit.Framework;

// test result- the cirlce game object is not created in the test

public class Graph
{
    [Test]
    public void ShowGraph_ProducesExpectedGraph()
    {
        // Arrange
        var gameObject = new GameObject();
        var graphComponent = gameObject.AddComponent<window_Graph>();
        var graphContainer = new GameObject().AddComponent<RectTransform>();
        var circleSprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
        graphComponent.SetGraphContainer(graphContainer);
        graphComponent.circleSprite = circleSprite;

        var valueList = new List<int> { 50, 75, 100 }; // Example score values

        // Act
        graphComponent.ShowGraph(valueList);

        // Assert
        // Add assertions here to check if the graph is displayed as expected
        // For example, you could check if the correct number of circles and connections are created
        Assert.AreEqual(3, graphContainer.childCount); // Assuming each point creates a circle game object
        Assert.AreEqual(2, graphContainer.GetChild(0).childCount); // Assuming each connection creates a dot connection game object
    }

    public class window_Graph : MonoBehaviour
    {
        [SerializeField] public Sprite circleSprite;
        private RectTransform graphContainer;
        private const int MAX_SCORE = 100;
        private Color pointColor = new Color32(116, 64, 222, 255); // Set the color of the points
        
        public void ShowGraph(List<int> valueList)
        {
            if (graphContainer == null)
            {
                Debug.LogError("Graph container is not set.");
                return;
            }

            float graphHeight = graphContainer.sizeDelta.y;
            float yMaximum = 100f;
            float xSize = 50f;
            
            GameObject lastCircleGameObject = null;
            for (int i = 0; i < valueList.Count; i++)
            {
                if (valueList[i] == 0)
                    continue;

                float xPosition = (i + 1) + i * xSize;
                float yPosition = (valueList[i] / yMaximum) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));

                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                                         circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                }

                lastCircleGameObject = circleGameObject;
            }

            graphContainer.sizeDelta = new Vector2((valueList.Count - 1) * xSize, graphContainer.sizeDelta.y);
        }

        private GameObject CreateCircle(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("circle", typeof(Image), typeof(RectTransform));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = circleSprite;
            gameObject.GetComponent<Image>().color = pointColor;
            
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }

        private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = new Color(1, 0.5f, 0);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, dir));
        }

        // Setter for graph container
        public void SetGraphContainer(RectTransform container)
        {
            graphContainer = container;
        }
    }
}
