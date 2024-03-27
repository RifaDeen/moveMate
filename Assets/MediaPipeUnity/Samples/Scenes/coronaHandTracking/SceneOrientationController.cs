using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOrientationController : MonoBehaviour
{
    // Define the scene name that requires landscape orientation
    public string landscapeSceneName;

    // Define the desired orientation
    public ScreenOrientation desiredOrientation = ScreenOrientation.LandscapeLeft;

    // Define the default orientation
    private ScreenOrientation defaultOrientation = ScreenOrientation.Portrait;

    void Start()
    {
        // Check if the current scene is the one that requires landscape orientation
        if (SceneManager.GetActiveScene().name == landscapeSceneName)
        {
            // Set the desired screen orientation
            Screen.orientation = desiredOrientation;
        }
    }

    // Called when the GameObject is disabled
    void OnDisable()
    {
        // Set the default screen orientation when exiting the scene
        Screen.orientation = defaultOrientation;
    }
}
