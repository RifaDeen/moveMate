using UnityEngine;
using UnityEngine.UI;

public class piggy_TimerManager : MonoBehaviour
{
    public static piggy_TimerManager Instance { get; private set; }

    [SerializeField] private Text timerText;
    private float elapsedTime;
    private bool isTimerRunning = false;
    private bool gameStarted;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void RestartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = false;
        UpdateTimerText();
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        UpdateTimerText();
    }

    public void StartGame()
    {
        gameStarted = true; // Set game started
    }

    public void ShowStartScreen()
    {
        gameStarted = false; // Set game not started
        UpdateTimerText(); // Update the timer text to keep it at 00:00
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
