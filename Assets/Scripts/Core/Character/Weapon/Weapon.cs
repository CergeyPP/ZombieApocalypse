using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponStat
{
    public float damage;
    public float fireInterval;
    public float range;

    public float fireRate => 10.0F / fireInterval;
}

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponConfiguration _config;
    [SerializeField] private Transform _attackPoint;

    private WeaponStat _stats;
    public WeaponStat Stats => _stats;
    private Timer _timer;
    public bool CanAttack => _timer.CanPerform;
    public WeaponConfiguration Config => _config;

    private void Awake()
    {
        _timer = new Timer(this);
    }

    public void SetStats(WeaponStat stats)
    {
        _stats = stats;
    }

    public void Attack()
    {
        _timer.StartTime(_stats.fireInterval);
    }

    public void DealDamage(IDamagable victim, GameObject causer)
    {
        if (victim != null)
        {
            victim.RegisterDamage(_stats.damage, causer);
        }
    }

    public void DrawEffect(Vector3 impactPoint)
    {
        StopCoroutine(nameof(_config.PlayFX));
        StartCoroutine(_config.PlayFX(_attackPoint, impactPoint));
    }

    public void Equip()
    {
        StopAllCoroutines();
    }
    public void Unequip()
    {
        StopAllCoroutines();
    }
}
