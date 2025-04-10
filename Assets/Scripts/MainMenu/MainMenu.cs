using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MainMenuDecorViewer _mainMenuDecor;
    [SerializeField] private Canvas _mainMenuCanvas;

    public Action Opened;
    public Action Closed;
    public Wallet CoinWallet => _inventory.Wallet;

    public void OpenMenu()
    {
        _mainMenuCanvas.enabled = true;
        gameObject.SetActive(true);
        _mainMenuDecor.Show();
        Opened?.Invoke();
    }

    public void CloseMenu()
    {
        _mainMenuCanvas.enabled = false;
        gameObject.SetActive(false);
        _mainMenuDecor.Hide();
        Closed?.Invoke();
    }
}
