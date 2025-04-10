using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIShow : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _uiToShow;
    [SerializeField] private GameObject _joystick;

    private void Start()
    {
        _gameManager.GameStarted += OnGameStarted;
    }

    private void OnDestroy()
    {
        _gameManager.GameStarted -= OnGameStarted;
    }

    public void OnGameStarted()
    {
        _uiToShow.SetActive(!YG.YandexGame.savesData.isTutorCompleted);
        _joystick.SetActive(YG.YandexGame.savesData.isTutorCompleted);
    }
}
