using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private YandexGame _ygSingleton;
    [Header("UI")]
    [SerializeField] private CanvasGroup _gameUI;
    [SerializeField] private CanvasGroup _pauseMenu;

    [Header("Game Options")]
    [SerializeField] private LevelGenerator _levelGenerator;

    public event Action GameStarted;

    bool _isGamePaused = false;

    private List<Character> _enemies = new List<Character>();
    
    private Character _playerCharacter;
    public Character PlayerCharacter => _playerCharacter;
    public YandexGame YG => _ygSingleton;

    private Wallet _inRunWallet;

    public Wallet CurrentRunWallet => _inRunWallet;

    private void Start()
    {
        StartCoroutine(StartLevel());
    }

    public IEnumerator StartLevel()
    {
        yield return null;
        _pauseMenu.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(true);
        _levelGenerator.StartLevel();
        _inRunWallet = new Wallet();
        yield return null;
        _levelGenerator.InstantiateRoadEnemies();
        _enemies = _levelGenerator.GetAllEnemies().ToList();
        foreach (var enemy in _enemies)
        {
            enemy.Died += OnEnemyDied;
        }
        _playerCharacter = _levelGenerator.PlayerCharacter;

        GameStarted?.Invoke();
    }

    private void OnEnemyDied(GameObject obj)
    {
        _enemies.RemoveAll(enemy => { return enemy.gameObject == obj; });
        Character character = obj.GetComponent<Character>();
        if (character != null)
        {
            _inRunWallet.Add(character.ScoreReward);
        }
    }

    public void RestartLevel()
    {
        _levelGenerator.ClearLevel();
        foreach (var enemy in _enemies)
        {
            Destroy(enemy.gameObject);
        }
        Destroy(_playerCharacter.gameObject);
        //
        // other destroys
        //
        Unpause();
        StartCoroutine(StartLevel());
    }

    public void OnLevelEndedSuccessful()
    {
        YandexGame.savesData.Wallet.Add(_inRunWallet.Coins);
        YandexGame.SaveProgress();
    }

    public void OnGameOver()
    {

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
        _pauseMenu.gameObject.SetActive(true);
        _gameUI.gameObject.SetActive(false);
        Time.timeScale = 0;
        _isGamePaused = true;

    }
    private void Unpause()
    {
        _isGamePaused = false;
        _pauseMenu.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
}
