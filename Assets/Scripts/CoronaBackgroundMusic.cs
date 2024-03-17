using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoronaBackgroundMusic : MonoBehaviour
{
    private static CoronaBackgroundMusic backgroundMusic;

    void Awake()
    {
        if(backgroundMusic == null)
        {
            backgroundMusic = this;
            DontDestroyOnLoad(backgroundMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
