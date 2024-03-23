using System;
using System.Collections;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class CoronaScoreManager : MonoBehaviour
{
    public Text scoreText;
    private float score;
    private bool isGameOver = false;

    // Feedback UI elements
    [SerializeField] private Text startoffFeedback;
    [SerializeField] private Text score5Feedback;
    [SerializeField] private Text score10Feedback;
    [SerializeField] private Text score20Feedback;
    [SerializeField] private Text score30Feedback;
    [SerializeField] private Text score40Feedback;
    [SerializeField] private Text score50Feedback;
    private User user;
    private Firebase.FirebaseApp app;
    private string userID;

    private string gameID;
    private string gameInstanceId;

    // Start is called before the first frame update
    void Start()
    {
        // Start scoring when the game begins
        StartScoring();

        startoffFeedback.gameObject.SetActive(true);
        score5Feedback.gameObject.SetActive(false);
        score10Feedback.gameObject.SetActive(false);
        score20Feedback.gameObject.SetActive(false);
        score30Feedback.gameObject.SetActive(false);
        score40Feedback.gameObject.SetActive(false);
        score50Feedback.gameObject.SetActive(false);

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

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            score += 1 * Time.deltaTime;
            scoreText.text = ((int)score).ToString();

            // Check for score milestones and display feedback
            CheckScoreMilestones();
        }
    }

    // Call this method when the game is over
    public void GameOver()
    {
        isGameOver = true;
        
     
        if (AuthManager.CurrentUser != null)
        {
            userID = AuthManager.CurrentUser.UserId;
            Debug.Log("User ID obtained from user object: " + userID);
        }
        else
        {
            Debug.LogError("User object is null");
        }

        gameID = "gameid";
        GameUtils gameUtils = new GameUtils();
        gameInstanceId = gameUtils.GenerateGameInstanceId();
        int scoreInt = (int)score; // Cast the score from float to int
        float gameTime = CoronaTimerManager.Instance.GetElapsedTime();
        gameUtils.SaveGameDataToFirestore(userID, gameID, gameInstanceId, scoreInt, (float)Math.Round(gameTime,2));
   
    }

    // Start scoring
    public void StartScoring()
    {
        // Reset score when scoring starts
        score = 0f;
        isGameOver = false;
    }

    // Check for score milestones and display feedback
    private void CheckScoreMilestones()
    {
        int intScore = (int)score;

        if (intScore == 0)
        {
            StartCoroutine(DisplayStartoffFeedback());
        }
        else if (intScore == 5)
        {
            StartCoroutine(DisplayScore5Feedback());
        }
        else if (intScore == 10)
        {
            StartCoroutine(DisplayScore10Feedback());
        }
        else if (intScore == 20)
        {
            StartCoroutine(DisplayScore20Feedback());
        }
        else if (intScore == 30)
        {
            StartCoroutine(DisplayScore30Feedback());
        }
        else if (intScore == 40)
        {
            StartCoroutine(DisplayScore40Feedback());
        }
        else if (intScore == 50)
        {
            StartCoroutine(DisplayScore50Feedback());
        }
    }

    // Feedback coroutine methods

    private IEnumerator DisplayStartoffFeedback()
    {
        startoffFeedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        startoffFeedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore5Feedback()
    {
        score5Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score5Feedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore10Feedback()
    {
        score10Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score10Feedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore20Feedback()
    {
        score20Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score20Feedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore30Feedback()
    {
        score30Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score30Feedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore40Feedback()
    {
        score40Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score40Feedback.gameObject.SetActive(false);
    }

    private IEnumerator DisplayScore50Feedback()
    {
        score50Feedback.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        score50Feedback.gameObject.SetActive(false);
    }

}
