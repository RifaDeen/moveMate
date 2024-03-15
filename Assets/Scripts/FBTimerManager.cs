using UnityEngine;
using UnityEngine.UI;

public class FBTimerManager : MonoBehaviour
{
    public static FBTimerManager Instance { get; private set; }

    [SerializeField] private Text timerText;
    private float elapsedTime;

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
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void RestartTimer()
    {
        elapsedTime = 0f;
    }
}
