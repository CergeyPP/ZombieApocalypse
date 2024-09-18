using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wallet
{
    private int _coins;
    public int Coins => _coins;
    
    public event Action<int, int> CoinAdded; //1 - coin addition, 2 - result coins
    public event Action<int, int> CoinSpent;
    public void Add(int addCoinCount)
    {
        _coins += addCoinCount;
        CoinAdded?.Invoke(addCoinCount, _coins);
    }

    public Wallet(int coins)
    {
        _coins = coins;
    }

    public bool CanPurchase(int price)
    {
        return _coins >= price;
    }

    public void Purchase(int price)
    {
        if (!CanPurchase(price))
        {
            return;
        }
        _coins -= price;
        CoinSpent?.Invoke(price, _coins);
    }
}
