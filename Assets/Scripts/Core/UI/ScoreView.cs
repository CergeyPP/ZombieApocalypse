using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private Text _text;
    [SerializeField] private float _maxTotalUpdateTime;
    [SerializeField] private AnimationCurve _increaseCurve;
    private int _score;

    private Wallet _wallet;

    private void Awake()
    {
        if (_gameManager != null)
            _gameManager.GameStarted += OnInit;
        if (_mainMenu != null)
            _mainMenu.Opened += OnInit;
    }

    private void OnEnable()
    {
        if (_wallet != null)
        {
            _wallet.CoinAdded += OnCoinAdded;
            _wallet.CoinSpent += OnCoinSpent;
        }
    }

    private void OnDisable()
    {
        if (_wallet != null)
        {
            _wallet.CoinAdded -= OnCoinAdded;
            _wallet.CoinSpent -= OnCoinSpent;
        }
    }

    private void OnInit()
    {
        if (_gameManager != null)
        {
            _wallet = _gameManager.CurrentRunWallet;
            _score = 0;
        }
        else if (_mainMenu != null)
        {
            _wallet = _mainMenu.CoinWallet;
            _score = _wallet.Coins;
        }
        UpdateScore();
        StopAllCoroutines();
        if (_wallet != null && enabled)
            OnEnable();
    }

    private void OnCoinAdded(int delta, int totalScore)
    {
        OnScoreChanged(delta, totalScore);
    }
    private void OnCoinSpent(int delta, int totalScore)
    {
        delta *= -1;
        OnScoreChanged(delta, totalScore);
    }

    private void OnScoreChanged(int delta, int totalScore)
    {
        StartCoroutine(ScoreViewChangeCoroutine(delta, totalScore));
    }

    private IEnumerator ScoreViewChangeCoroutine(int delta, int totalScore)
    {
        int initScore = totalScore - delta;
        int prevScore = initScore;
        float curveEval = 0;
        while (initScore < totalScore)
        {
            int newScore = initScore + (int)((float)delta * _increaseCurve.Evaluate(curveEval));
            curveEval += Time.unscaledDeltaTime;
            int deltaScoreFrame = newScore - prevScore;
            prevScore = newScore;
            if (Mathf.Abs(deltaScoreFrame) > 0)
            {
                _score += deltaScoreFrame;
                UpdateScore();
            }

            yield return null;
        }

        int lastAdd = totalScore - prevScore;
        if (Mathf.Abs(lastAdd) > 0)
        {
            _score += lastAdd;
            UpdateScore();
        }
    }

    private void UpdateScore()
    {
        _text.text = _score.ToString();
    }
}
