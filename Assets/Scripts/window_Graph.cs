using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    
    private void Awake()
{
//     graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
graphContainer = transform.Find("ScrollView/graphContainer").GetComponent<RectTransform>();
   graphContainer.pivot = new Vector2(0, 0.5f); // Set the pivot to expand to the right
       StartCoroutine(RetrieveAndShowGraphAsync());
}
    private IEnumerator RetrieveAndShowGraphAsync()     {
         RetrieveData retrieveData = new RetrieveData();
         yield return retrieveData.RetrieveGameData("newplayer", "gameid"); // Fix: Change the return type of RetrieveGameDataFromFirestore to IEnumerator
         List<int> valueList = retrieveData.scoreList();
         ShowGraph(valueList);
  
        
        // List<int> valueList=new List<int>(){5,0,99,0,78,54,30,23,9,4,4,60,50,90,40};
        // ShowGraph(valueList);
    }


private GameObject CreateCircle(Vector2 anchoredPosition) {
    // GameObject gameObject = new GameObject("circle", typeof(Image));
    GameObject gameObject = new GameObject("circle", typeof(Image), typeof(RectTransform));
    gameObject.transform.SetParent(graphContainer, false);
    gameObject.GetComponent<Image>().sprite = circleSprite; // Fix: Access the Image component and assign the circleSprite to its sprite property
    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    rectTransform.anchoredPosition = anchoredPosition;
    rectTransform.sizeDelta = new Vector2(11, 11);
    rectTransform.anchorMin = new Vector2(0, 0);
    rectTransform.anchorMax = new Vector2(0, 0);
    return gameObject;
}
// private void ShowGraph(List<int> valueList){
//     float graphHeight= graphContainer.sizeDelta.y;
//     float ymaximum=100f;
//     float xsize=50f;
//     GameObject lastCircleGameObject=null;
//     for (int i=0; i<valueList.Count; i++){
//         float xPosition = xsize + i * xsize;
//         float yPosition = (valueList[i] / ymaximum) * graphHeight;
//         GameObject circleGameObject=CreateCircle(new Vector2(xPosition, yPosition));
//         if(lastCircleGameObject!=null){
//             CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
//         }
//         lastCircleGameObject=circleGameObject;
//     }

// }

private void ShowGraph(List<int> valueList){
    float graphHeight= graphContainer.sizeDelta.y;
    float ymaximum=100f;
    float xsize=50f;
    GameObject lastCircleGameObject=null;
    for (int i=0; i<valueList.Count; i++){
        if (valueList[i] == 0) continue; // Skip if the score is zero
        float xPosition = xsize + i * xsize;
        float yPosition = (valueList[i] / ymaximum) * graphHeight;
        GameObject circleGameObject=CreateCircle(new Vector2(xPosition, yPosition));
        if(lastCircleGameObject!=null){
            CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
        }
        lastCircleGameObject=circleGameObject;
    }
    // Update the width of the graphContainer to fit all points
    graphContainer.sizeDelta = new Vector2((valueList.Count - 1) * xsize, graphContainer.sizeDelta.y);
}

private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB){
    GameObject gameObject = new GameObject("dotConnection", typeof(Image));
    gameObject.transform.SetParent(graphContainer, false);
    // gameObject.GetComponent<Image>().color = new Color(1,1,1,.5f);
    gameObject.GetComponent<Image>().color = new Color(1, 0.5f, 0); // Set color to orange
    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    Vector2 dir = (dotPositionB - dotPositionA).normalized;
    float distance = Vector2.Distance(dotPositionA, dotPositionB);
    rectTransform.anchorMin = new Vector2(0,0);
    rectTransform.anchorMax = new Vector2(0,0);
    rectTransform.sizeDelta = new Vector2(distance, 3f);
    rectTransform.anchoredPosition = dotPositionA+ dir*distance*.5f;
    rectTransform.localEulerAngles = new Vector3(0,0,Vector2.SignedAngle(Vector2.right, dir));    

}
}
