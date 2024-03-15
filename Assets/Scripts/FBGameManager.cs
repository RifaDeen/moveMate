using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;

public class FBGameManager : MonoBehaviour
{
    public static FBGameManager Instance { get; private set; }
   [SerializeField] private FBPlayer player;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject getReadyImage; // Reference to the "Get Ready" image
    [SerializeField] private GameObject gameOverImage; // Reference to the "Game Over" image
    private FBBackgroundMusic backgroundMusic; 

    private User user;
    private Firebase.FirebaseApp app;

    private string userID;

    private string gameID;
    private string gameInstanceId;
    private int score;
    public int Score => score;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
            Pause();

            backgroundMusic = FindObjectOfType<FBBackgroundMusic>();
        }
    }

    private void Start()
    {
        // Show "Get Ready" image and play button when the game starts
        getReadyImage.SetActive(true);
        playButton.SetActive(true);
         gameOverImage.SetActive(false);

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

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();
        playButton.SetActive(false);
        gameOverImage.SetActive(false); // Hide "Game Over" image
        getReadyImage.SetActive(false); // Hide "Get Ready" image

        Time.timeScale = 1f;
        player.enabled = true;

        // Play background music
        if (backgroundMusic != null)
        {
            backgroundMusic.PlayMusic();
        }

        FBPipes[] pipes = FindObjectsOfType<FBPipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }

        // Restart the timer through the singleton instance
        if (FBTimerManager.Instance != null)
        {
            FBTimerManager.Instance.RestartTimer();
            Debug.Log("Play Button Pressed - Timer Restarted");
        }
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        getReadyImage.SetActive(false); // Hide "Get Ready" image
        gameOverImage.SetActive(true); // Show "Game Over" image

        Pause();

        // Stop background music
        if (backgroundMusic != null)
        {
            backgroundMusic.StopMusic();
        }

        // if (user != null)
        // {
        //     userID = user.UserId;
        // }
        // else
        // {
        //     Debug.LogError("User object is null");
        // }

        userID = "Ahamad";   
        gameID = "flappy_bird";
        gameInstanceId = GenerateGameInstanceId();
        SaveGameDataToFirestore(userID, gameID, gameInstanceId, score);

    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

      private string GenerateGameInstanceId()
    {
        long timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        System.Random random = new System.Random();
        int randomComponent = random.Next(10000);
        string gameInstanceId = $"{timestamp}-{randomComponent}";
        return gameInstanceId;
    }

    private void SaveGameDataToFirestore(string userId, string gameId, string gameInstanceId, int score)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        DocumentReference gameHistoryRef = db.Collection("users").Document(userId)
            .Collection("game_history").Document(gameId).Collection("game_instances").Document(gameInstanceId);

        Dictionary<string, object> gameData = new Dictionary<string, object>
        {
            { "score", score },
          //  { "time", },
            { "date", DateTime.UtcNow } // Store the current date
        };

        gameHistoryRef.SetAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to write game data to Firestore: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Firestore write operation was cancelled");
            }
            else
            {
                Debug.Log("Game data successfully written to Firestore");
            }
        });
    }
}
