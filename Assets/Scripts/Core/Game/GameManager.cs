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
        StartCoroutine(StartLevel());
    }

    public IEnumerator StartLevel()
    {
        yield return null;
        _pauseMenu.gameObject.SetActive(false);
        _levelGenerator.StartLevel();
        yield return null;
        _levelGenerator.InstantiateRoadEnemies();
        _enemies = _levelGenerator.GetAllEnemies().ToList();
        foreach (var enemy in _enemies)
        {
            enemy.Died += OnEnemyDied;
        }
        _playerCharacter = _levelGenerator.PlayerCharacter;

        OnGameStarted?.Invoke();
    }

    private void OnEnemyDied(GameObject obj)
    {
        _enemies.RemoveAll(enemy => { return enemy.gameObject == obj; });
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
        StartCoroutine(StartLevel());
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

    }

    private void Pause()
    {
        _isGamePaused = true;
        _pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    private void Unpause()
    {
        _isGamePaused = false;
        _pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
