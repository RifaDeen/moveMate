using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBackgroundMusic : MonoBehaviour
{
    private static BoxBackgroundMusic backgroundMusic;

    private AudioSource audioSource; // Reference to the AudioSource component

    void Awake()
    {
        if (backgroundMusic == null)
        {
            backgroundMusic = this;
            DontDestroyOnLoad(backgroundMusic);

            // Add an AudioSource component if not present
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
