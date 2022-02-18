using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    {
        instance = this;
    }

    public AudioSource matchSound, themeMusic;
    public void PlayWhenMatchGem()
    {
        matchSound.Stop();
        matchSound.Play();
    }

    public void StopThemeMusic()
    {
        themeMusic.Stop();
    }
    public void PlayThemeMusic()
    {
        themeMusic.Stop();
        themeMusic.Play();
    }
}

