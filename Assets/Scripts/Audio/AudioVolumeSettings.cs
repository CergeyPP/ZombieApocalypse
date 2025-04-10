using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private bool _isMusic = false;

    private AudioManager _manager;

    private void Awake()
    {
        _manager = FindFirstObjectByType<AudioManager>();
    }

    private void OnEnable()
    {
        if (_isMusic)
        {
            _manager.MusicVolumeChanged += OnMusicVolumeChanged;
        }
        else
        {
            _manager.MasterVolumeChanged += OnMasterVolumeChanged;
        }
    }

    private void OnDisable()
    {
        if (_isMusic)
        {
            _manager.MusicVolumeChanged -= OnMusicVolumeChanged;
        }
        else
        {
            _manager.MasterVolumeChanged -= OnMasterVolumeChanged;
        }
    }

    private void Start()
    {
        if (_isMusic)
        {
            OnMusicVolumeChanged(0);
        }
        else
        {
            OnMasterVolumeChanged(0);
        }

    }

    private void OnMusicVolumeChanged(float vol)
    {
        SetVolume(_manager.MasterVolume / 100 * _manager.MusicVolume);
    }

    private void OnMasterVolumeChanged(float vol)
    {
        SetVolume(_manager.MasterVolume);
    }

    private void SetVolume(float volume)
    {
        _source.volume = volume / 100;
    }
}
