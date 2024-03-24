using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private GameObject tooltip;
    private RectTransform graphContainer;
    private const int MAX_SCORE = 100;
    private Color pointColor = new Color32(116, 64, 222, 255); // Set the color of the points
    private User user;
    private string userID;
    
    private void Awake()
{

graphContainer = transform.Find("ScrollView/graphContainer").GetComponent<RectTransform>();
graphContainer.pivot = new Vector2(0, 0.5f); // Set the pivot to expand to the right
CreateAxisLines();
StartCoroutine(RetrieveAndShowGraphAsync());
}
private void AddScore(int score, List<int> scoreList)
{
    if(score > MAX_SCORE)
    {
        scoreList.Add(MAX_SCORE);
    }
    else
    {
        scoreList.Add(score);
    }
}
    private IEnumerator RetrieveAndShowGraphAsync()     {
      
        if (AuthManager.CurrentUser != null)
        {
            userID = AuthManager.CurrentUser.UserId;
            Debug.Log("User ID obtained from user object: " + userID);
        }
        else
        {
            Debug.LogError("User object is null");
        }
        
         RetrieveData retrieveData = new RetrieveData();
         yield return retrieveData.RetrieveGameData(userID, "gameid"); // Fix: Change the return type of RetrieveGameDataFromFirestore to IEnumerator
        //  List<int> valueList = retrieveData.scoreList();
         List<int> valueList = new List<int>();
            foreach (int score in retrieveData.scoreList())
            {
                AddScore(score, valueList);
            }
                ShowGraph(valueList);
        }

    private void CreateAxisLines()
{
    // Create X-Axis line
    GameObject xAxisLine = new GameObject("xAxisLine", typeof(Image));
    xAxisLine.transform.SetParent(graphContainer, false);
    RectTransform xAxisRectTransform = xAxisLine.GetComponent<RectTransform>();
    xAxisRectTransform.anchorMin = new Vector2(0, 0);
    xAxisRectTransform.anchorMax = new Vector2(1,0);
    xAxisRectTransform.sizeDelta = new Vector2(0, 2);
    xAxisRectTransform.anchoredPosition = Vector2.zero;
    xAxisLine.GetComponent<Image>().color = Color.black;

    // Create Y-Axis line
    GameObject yAxisLine = new GameObject("yAxisLine", typeof(Image));
    yAxisLine.transform.SetParent(graphContainer, false);
    RectTransform yAxisRectTransform = yAxisLine.GetComponent<RectTransform>();
    yAxisRectTransform.anchorMin = new Vector2(0, 0);
    yAxisRectTransform.anchorMax = new Vector2(0, 1);
    yAxisRectTransform.sizeDelta = new Vector2(2, 0);
    yAxisRectTransform.anchoredPosition = Vector2.zero;
    yAxisLine.GetComponent<Image>().color = Color.black;
}


private GameObject CreateCircle(Vector2 anchoredPosition) {
    // GameObject gameObject = new GameObject("circle", typeof(Image));
    GameObject gameObject = new GameObject("circle", typeof(Image), typeof(RectTransform));
    gameObject.transform.SetParent(graphContainer, false);
    gameObject.GetComponent<Image>().sprite = circleSprite; // Fix: Access the Image component and assign the circleSprite to its sprite property
    gameObject.GetComponent<Image>().color = pointColor; // Set the color of the points
    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
    rectTransform.anchoredPosition = anchoredPosition;
    rectTransform.sizeDelta = new Vector2(11, 11);
    rectTransform.anchorMin = new Vector2(0, 0);
    rectTransform.anchorMax = new Vector2(0, 0);
    return gameObject;
}

 private void CreateLabel(Vector2 anchoredPosition, string text) {
        GameObject gameObject = new GameObject("label", typeof(Text));
        gameObject.transform.SetParent(graphContainer, false);
        Text label = gameObject.GetComponent<Text>();
        label.text = text;
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); // Set the font to Arial
        label.fontSize = 14; // Set the font size to 14
        label.color = Color.black; // Set the text color to black
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(100, 20); // Set the size of the label
        rectTransform.pivot = new Vector2(0.5f, 1); // Set the pivot to center top
    }

private void ShowGraph(List<int> valueList){
    float graphHeight= graphContainer.sizeDelta.y;
    float ymaximum=100f;
    float xsize=50f;
    
    GameObject lastCircleGameObject=null;
    for (int i=0; i<valueList.Count; i++){
        if (valueList[i] == 0) continue; // Skip if the score is zero
        float xPosition = (i+1)+ i * xsize; //
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
