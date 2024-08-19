using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Slider _bar;

    private Health _health;

    private void Awake()
    {
        _gameManager.OnGameStarted += OnInitLevel;
    }

    public void UpdateBar()
    {
        _bar.value = _health.Percentage / 100;
    }

    private void OnInitLevel()
    {
        _health = _gameManager.PlayerCharacter.Health;
        OnEnable();
        UpdateBar();
    }
    private void OnEnable()
    {
        if (_health != null)
        {
            _health.Healed += OnHealthChanged;
            _health.Damaged += OnHealthChanged;
        }
    }
    private void OnDisable()
    {
        if (_health != null)
        {
            _health.Healed -= OnHealthChanged;
            _health.Damaged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float delta, float health, GameObject causer)
    {
        UpdateBar();
    }
}
