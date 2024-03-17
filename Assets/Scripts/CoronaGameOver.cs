using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoronaGameOver : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Camera mainCamera; // Reference to your main camera

    // Default camera size
    private float defaultCameraSize;

    void Start()
    {
        // Store the default camera size on Start
        if (mainCamera != null)
        {
            defaultCameraSize = mainCamera.orthographicSize;
        }
    }

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            GameOverAction();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Reset camera size after restarting the scene
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = defaultCameraSize;
        }
    }

    // Show game over panel and perform other actions
    private void GameOverAction()
    {
        gameOverPanel.SetActive(true);
    }
}
