using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dinoBackgroundMusic : MonoBehaviour
{
    private static dinoBackgroundMusic backgroundMusic;

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
