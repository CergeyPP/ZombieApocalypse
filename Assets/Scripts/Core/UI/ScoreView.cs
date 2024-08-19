using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Text _text;
    [SerializeField] private float _maxTotalUpdateTime;
    [SerializeField] private AnimationCurve _increaseCurve;
    private int _score;

    private Wallet _wallet;

    private void Awake()
    {
        _gameManager.GameStarted += OnGameStarted;
    }

    private void OnEnable()
    {
        if (_wallet != null)
            _wallet.CoinAdded += OnScoreChanged;
    }

    private void OnDisable()
    {
        if (_wallet != null)
            _wallet.CoinAdded -= OnScoreChanged;
    }

    private void OnGameStarted()
    {
        _wallet = _gameManager.CurrentRunWallet;
        if (_wallet != null && enabled)
            _wallet.CoinAdded += OnScoreChanged;
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
