using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _pauseMenu;

    [Header("Game Options")]
    [SerializeField] private LevelGenerator _levelGenerator;

    public event Action OnGameStarted;

    bool _isGamePaused = false;

    private List<Character> _enemies = new List<Character>();
    
    private Character _playerCharacter;
    public Character PlayerCharacter => _playerCharacter;

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        _pauseMenu.gameObject.SetActive(false);
        _levelGenerator.StartLevel();

        _enemies = _levelGenerator.GetAllEnemies().ToList();
        _playerCharacter = _levelGenerator.PlayerCharacter;

        OnGameStarted?.Invoke();
    }

    public void RestartLevel()
    {
        _levelGenerator.ClearLevel();
        foreach (var enemy in _enemies)
        {
            Destroy(enemy.gameObject);
        }
        //
        // other destroys
        //
        Unpause();
        StartLevel();
    }

    public void OnPauseButtonClicked()
    {
        if (_isGamePaused)
        {
            Unpause();
        } 
        else
        {
            Pause();
        }

        _isGamePaused = !_isGamePaused;
    }

    private void Pause()
    {
        _pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    private void Unpause()
    {
        _pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
