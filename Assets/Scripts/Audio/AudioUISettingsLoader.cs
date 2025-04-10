using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class AudioUISettingsLoader : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;

    private void Start()
    {
        _masterSlider.value = YandexGame.savesData.masterVolume;
        _musicSlider.value = YandexGame.savesData.musicVolume;
    }
}
