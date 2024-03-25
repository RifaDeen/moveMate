using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOrientationControllerDino : MonoBehaviour
{
    // Define the scene name that requires landscape orientation
    public string landscapeSceneName;

    // Define the desired orientation
    public ScreenOrientation desiredOrientation = ScreenOrientation.LandscapeLeft;

    void Start()
    {
        // Check if the current scene is the one that requires landscape orientation
        if (SceneManager.GetActiveScene().name == landscapeSceneName)
        {
            // Set the desired screen orientation
            Screen.orientation = desiredOrientation;
        }
    }
}
