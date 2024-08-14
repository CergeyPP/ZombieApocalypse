using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponConfiguration _config;
    [SerializeField] private Transform _attackPoint;
    private Timer _timer;
    public bool CanAttack => _timer.CanPerform;
    public WeaponConfiguration Config => _config;
    private void Awake()
    {
        _timer = new Timer(this);
    }
    public void Attack()
    {
        _timer.StartTime(_config.AttackInfo.FireRate);
    }

    public void DealDamage(IDamagable victim, GameObject causer)
    {
        if (victim != null)
        {
            victim.RegisterDamage(_config.AttackInfo.Damage, causer);
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
