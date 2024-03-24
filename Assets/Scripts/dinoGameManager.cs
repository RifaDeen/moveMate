using System;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class dinoGameManager : MonoBehaviour
{
    public static dinoGameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { 
        get; 
        private set; 
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Text timerText;    
    [SerializeField] private TextMeshProUGUI getReadyText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private Text score0Feedback;
    [SerializeField] private Text score2Feedback;
    [SerializeField] private Text score5Feedback;
    [SerializeField] private Text score10Feedback;
    [SerializeField] private Text score20Feedback;
    [SerializeField] private Text score30Feedback;
    [SerializeField] private Text score50Feedback;
    [SerializeField] private Text score75Feedback;

    public dinoBackgroundMusic backgroundMusic;
    private DinoPlayer player;
    private dinoSpawner spawner;
    private bool isGameActive = false;
    private float score;
    public float Score => score;
    private User user;
    private Firebase.FirebaseApp app;
    private string userID;

    private string gameID;
    private string gameInstanceId;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<DinoPlayer>();
        spawner = FindObjectOfType<dinoSpawner>();
        score = 0f;

        // Show "Get Ready" text and retry button initially
        getReadyText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        retryButton.interactable = true;  // Enable retry button
        gameOverText.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);

        score0Feedback.gameObject.SetActive(true);
        score2Feedback.gameObject.SetActive(false);
        score5Feedback.gameObject.SetActive(false);
        score10Feedback.gameObject.SetActive(false);
        score20Feedback.gameObject.SetActive(false);
        score30Feedback.gameObject.SetActive(false);
        score50Feedback.gameObject.SetActive(false);
        score75Feedback.gameObject.SetActive(false);

         // Find the dinoBackgroundMusic object in the scene and get its component
        backgroundMusic = FindObjectOfType<dinoBackgroundMusic>();

        // Play the background music when the game starts
        backgroundMusic.PlayMusic();

        // Stop the timer when the game is over
        dinoTimerManager.Instance.StopTimer();

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

    public void NewGame()
    {
        dinoObstacle[] obstacles = FindObjectsOfType<dinoObstacle>();

        foreach (var obstacle in obstacles) {
            Destroy(obstacle.gameObject);
        }

        score = 0f;
        scoreText.text = "000"; // Reset score text to 0
        gameSpeed = initialGameSpeed;
        enabled = true;

        // Start the timer when the game begins
        dinoTimerManager.Instance.RestartTimer();

        // Play the background music when the game starts
        backgroundMusic.PlayMusic();

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        getReadyText.gameObject.SetActive(false);
        score0Feedback.gameObject.SetActive(false);

        // Enable retry button for the next game
        retryButton.interactable = true;

        // Allow player control
        isGameActive = true;
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        getReadyText.gameObject.SetActive(false);

        // Stop the background music when the game is over
        backgroundMusic.StopMusic();

        // Stop the timer when the game is over
        dinoTimerManager.Instance.StopTimer();

        UpdateHiscore();

        // Disable player control
        isGameActive = false;

     
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
        float gameTime = dinoTimerManager.Instance.GetElapsedTime();
        gameUtils.SaveGameDataToFirestore(userID, gameID, gameInstanceId, scoreInt, (float)Math.Round(gameTime,2));
    }

    public void Retry()
    {
        // Hide "Game Over" text and retry button
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        // Show "Get Ready" text and retry button for the next game
        getReadyText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        // Enable retry button for the next game
        retryButton.interactable = true;

        // Start a new game
        NewGame();
    }

    public void ObstaclePassed()
    {
        if (isGameActive)
        {
            // Increase score by one
            score += 1;
            scoreText.text = Mathf.FloorToInt(score).ToString("D3");

            // Check for score milestones and display feedback
            CheckScoreMilestones();
        }
    }

    private void CheckScoreMilestones()
    {
        int intScore = Mathf.FloorToInt(score);

        if (intScore == 0)
        {
            StartCoroutine(DisplayFeedback(score0Feedback));
        }
        else if (intScore == 2)
        {
            StartCoroutine(DisplayFeedback(score2Feedback));
        }
        else if (intScore == 5)
        {
            StartCoroutine(DisplayFeedback(score5Feedback));
        }
        else if (intScore == 10)
        {
            StartCoroutine(DisplayFeedback(score10Feedback));
        }
        else if (intScore == 20)
        {
            StartCoroutine(DisplayFeedback(score20Feedback));
        }
        else if (intScore == 30)
        {
            StartCoroutine(DisplayFeedback(score30Feedback));
        }
        else if (intScore == 50)
        {
            StartCoroutine(DisplayFeedback(score50Feedback));
        }
        else if (intScore == 75)
        {
            StartCoroutine(DisplayFeedback(score75Feedback));
        }
    }

    private System.Collections.IEnumerator DisplayFeedback(Text feedbackText)
    {
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        feedbackText.gameObject.SetActive(false);
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D3");
    }

     public void OnExitButtonClick()
        {
            // Load another scene before quitting
            SceneManager.LoadScene("gamePage");

            // Quit the game
            Application.Quit();
        }
}
