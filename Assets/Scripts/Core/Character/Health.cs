using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private float _maxHealth = 100.0f;
    private float _health;

    public event Action<float, float, GameObject> Damaged;
    public event Action<float, float, GameObject> Healed;
    public event Action<GameObject> Died;

    public bool IsDead => _health == 0;
    public float HP => _health;
    public float Percentage => _health / _maxHealth * 100;
    private void Awake()
    {
        _health = _maxHealth;
    }

    public void RegisterDamage(float damage, GameObject causer)
    {
        _health -= damage;
        if (_health < 0.0f)
            _health = 0;

        Damaged?.Invoke(damage, _health, causer);
        if (IsDead)
            Died?.Invoke(gameObject);
    }

    public void Heal(float heal, GameObject causer)
    {
        _health += heal;
        if (_health > _maxHealth)
            _health = _maxHealth;

        Healed?.Invoke(heal, _health, causer);
    }
}
