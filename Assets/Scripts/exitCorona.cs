using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ExitCorona : MonoBehaviour
{
    public void ExitCoronaMethod(string sceneName)
    {
        //   if (CoronaScoreManager.backgroundMusic != null)
        // {
        //     CoronaScoreManager.backgroundMusic.StopMusic();
        // }

        SceneManager.LoadScene(sceneName);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif


    }
}
