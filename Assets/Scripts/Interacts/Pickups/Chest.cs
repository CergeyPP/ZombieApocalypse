using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private Interactable _interactor;
    [SerializeField] private int _scoreAmount;

    private void Start()
    {
        _interactor.InteractEvent += OnInteract;
    }

    private void OnInteract(Interactable trigger, GameObject causer)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        Wallet wallet = gameManager.CurrentRunWallet;
        wallet.Add(_scoreAmount);
    }

}
