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
    [SerializeField] private Inventory _playerInventory;
    [Header("Game Options")]
    [SerializeField] private LevelGenerator _levelGenerator;
    [Header("UI")]
    [SerializeField] private Canvas _gameCanvas;
    [SerializeField] private CanvasGroup _gameUI;
    [SerializeField] private CanvasGroup _pauseMenu;
    [SerializeField] private CanvasGroup _gameOverUI;
    [SerializeField] private CanvasGroup _endLevelUI;
    [Header("MainMenu")]
    [SerializeField] private MainMenu _mainMenu;

    public event Action GameStarted;

    bool _isGamePaused = false;

    private List<Character> _enemies = new List<Character>();
    
    private Character _playerCharacter;
    public Character PlayerCharacter => _playerCharacter;
    public YandexGame YG => _ygSingleton;

    private Wallet _inRunWallet;
    public Wallet CurrentRunWallet => _inRunWallet;

    private CanvasGroup _raisedUI;

    private void Start()
    {
        BackToMainMenu();
    }

    public IEnumerator StartLevel()
    {
        _pauseMenu.gameObject.SetActive(false);
        _gameOverUI.gameObject.SetActive(false);
        _endLevelUI.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(true);

        _levelGenerator.StartLevel();
        _inRunWallet = new Wallet(0);
        yield return null;
        _levelGenerator.UpdateNavMesh();
        _levelGenerator.InstantiateRoadEnemies();
        _enemies = _levelGenerator.GetAllEnemies().ToList();
        foreach (var enemy in _enemies)
        {
            enemy.Died += OnEnemyDied;
        }
        _playerCharacter = _levelGenerator.PlayerCharacter;
        Transform playerSystem = _playerCharacter.transform.parent;
        Camera playerCamera = playerSystem.GetComponentInChildren<Camera>();
        playerCamera.enabled = true;
        //disable mainmenu camera
        _playerCharacter.Died += OnPlayerDied;
        _levelGenerator.EndLevelTrigger.InteractEvent.AddListener(OnEndLevelTriggered);
        _playerCharacter.GetComponent<WeaponArsenal>().Equip(_playerInventory.EquippedWeapon.config,
            _playerInventory.EquippedWeapon.level);
        GameStarted?.Invoke();
    }

    private void OnPlayerDied(GameObject obj)
    {
        OnGameOver();
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

    private void OnEndLevelTriggered(Interactable tirgger, GameObject triggerCauser)
    {
        OnLevelEndedSuccessful();
    }

    private void RestartLevel()
    {
        ClearLevel();
        _levelGenerator.UpdateNavMesh();
        StartCoroutine(StartLevel());
        Unpause();
    }

    public void OnLevelEndedSuccessful()
    {
        YandexGame.savesData.isTutorCompleted = true;
        YandexGame.SaveProgress();
        Pause(_endLevelUI);
    }

    public void OnGameOver()
    {
        Pause(_gameOverUI);
    }

    public void OnPauseButtonClicked()
    {
        if (_isGamePaused)
        {
            Unpause();
        } 
        else
        {
            Pause(_pauseMenu);
        }

    }

    private void ClearLevel()
    {
        _levelGenerator.ClearLevel();

        foreach (var enemy in _enemies)
        {
            Destroy(enemy.gameObject);
        }
        if (_playerCharacter != null)
            Destroy(_playerCharacter.gameObject);
    }

    public void StartGame()
    {
        YandexGame.FullscreenShow();
        _mainMenu.CloseMenu();
        Unpause();
        _gameCanvas.gameObject.SetActive(true);
        StartCoroutine(StartLevel());
    }

    public void BackToMainMenu()
    {
        if (_inRunWallet != null)
        {
            _playerInventory.Wallet.Add(_inRunWallet.Coins);
            YandexGame.SaveProgress();
            _playerInventory.OnInventoryChanged?.Invoke();
        }
        YandexGame.FullscreenShow();
        Unpause();
        ClearLevel();
        _gameCanvas.gameObject.SetActive(false);
        _mainMenu.OpenMenu();
    }
    private void Pause(CanvasGroup uiToRaise)
    {
        uiToRaise.gameObject.SetActive(true);
        _raisedUI = uiToRaise;
        _gameUI.gameObject.SetActive(false);
        Time.timeScale = 0;
        _isGamePaused = true;

    }
    private void Unpause()
    {
        _isGamePaused = false;
        _raisedUI?.gameObject.SetActive(false);
        _gameUI.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
}
