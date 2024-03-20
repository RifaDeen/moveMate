using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
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
    [SerializeField] private Button exitImage; // Reference to the "Game Over" image


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

        // Show "Get Ready" text and retry button initially
        getReadyText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitImage.gameObject.SetActive(true);
        retryButton.interactable = true;  // Enable retry button
        gameOverText.gameObject.SetActive(false);
        

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
        gameSpeed = initialGameSpeed;
        enabled = true;

        // Start the timer when the game begins
        dinoTimerManager.Instance.RestartTimer();

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        getReadyText.gameObject.SetActive(false);
        exitImage.gameObject.SetActive(false);


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
        getReadyText.gameObject.SetActive(false);
        exitImage.gameObject.SetActive(true);

        // Stop the timer when the game is over
        dinoTimerManager.Instance.StopTimer();

        UpdateHiscore();

        // Disable player control
        isGameActive = false;

        // if (user != null)
        // {
        //     userID = user.UserId;
        // }
        // else
        // {
        //     Debug.LogError("User object is null");
        // }

        userID = "newplayer";

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

        // Show "Get Ready" text and retry button for the next game
        getReadyText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        // Enable retry button for the next game
        retryButton.interactable = true;

        // Start a new game
        NewGame();
    }

    private void Update()
    {
        if (!isGameActive)
        {
            // Disable player control during "Get Ready" phase
            return;
        }

        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

    public void OnExitButtonClick()
        {
            // Load another scene before quitting
            SceneManager.LoadScene("gamePage");

            // Quit the game
            Application.Quit();
        }

}
