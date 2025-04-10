using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class AudioManager : MonoBehaviour
{
    public float MasterVolume
    {
        get
        {
            return YandexGame.savesData.masterVolume;
        }
        set
        {
            YandexGame.savesData.masterVolume = value;
            MasterVolumeChanged?.Invoke(value);
        }
    }

    public float MusicVolume
    {
        get
        {
            return YandexGame.savesData.musicVolume;
        }
        set
        {
            YandexGame.savesData.musicVolume = value;
            MusicVolumeChanged?.Invoke(value);
        }
    }

    public event Action<float> MasterVolumeChanged;
    public event Action<float> MusicVolumeChanged;

    private void Start()
    {
        MasterVolumeChanged?.Invoke(MasterVolume);
        MusicVolumeChanged?.Invoke(MusicVolume);
    }
}
