using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;


public class CoronaScoreManager : MonoBehaviour
{
    public Text scoreText;
    private float score;
    private bool isGameOver = false;
    private User user;
    private Firebase.FirebaseApp app;
    private string userID;

    private string gameID;
    private string gameInstanceId;

    // Update is called once per frame
    
     private void Start()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
        // Create and hold a reference to your FirebaseApp,
        // where app is a Firebase.FirebaseApp property of your application class.
            app = Firebase.FirebaseApp.DefaultInstance;
            Debug.Log("Firebase is ready to use!");

        // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
             UnityEngine.Debug.LogError(System.String.Format(
             "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
    });
    }
    
    void Update()
    {
        if (!isGameOver)
        {
            score += 1 * Time.deltaTime;
            scoreText.text = ((int)score).ToString();
        }
    }

    // Call this method when the game is over
    public void GameOver()
    {
        isGameOver = true;
        // Implement any additional actions when the game is over
        // For example, you can show a game over panel or perform other actions.

         // if (user != null)
        // {
        //     userID = user.UserId;
        // }
        // else
        // {
        //     Debug.LogError("User object is null");
        // }

        userID = "test_user";

        gameID = "corona_game";
        GameUtils gameUtils = new GameUtils();
        gameInstanceId = gameUtils.GenerateGameInstanceId();
        gameUtils.SaveGameDataToFirestore(userID, gameID, gameInstanceId, (int)score);

    }
}
