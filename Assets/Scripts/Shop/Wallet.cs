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

    public void Add(int addCoinCount)
    {
        _coins += addCoinCount;
        CoinAdded?.Invoke(addCoinCount, _coins);
    }

    public Wallet()
    {
        _coins = 0;
    }
}
