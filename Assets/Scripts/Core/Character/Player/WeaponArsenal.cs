using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponArsenal : MonoBehaviour, IAttackProvider
{
    [SerializeField] private Transform _gunSlot;
    [SerializeField] private Transform _meleeSlot;
    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private LayerMask _enemyLayer;

    private Weapon _equippedWeapon;
    public Weapon EquippedWeapon => _equippedWeapon;

    private bool _wantToShoot;
    private bool IsColliderOnEnemyLayer(Collider collider)
        => ((_enemyLayer >> collider.gameObject.layer) & 1) == 1;
    private RaycastHit RaycastForward
    {
        get
        {
            Debug.DrawLine(_raycastPoint.position, _raycastPoint.position + _raycastPoint.forward * 5);
            RaycastHit info;
            Physics.Raycast(_raycastPoint.position,
                _raycastPoint.forward,
                out info,
                _equippedWeapon.Stats.range, //ренж оружия
                _raycastLayers, QueryTriggerInteraction.Ignore);
            return info;
        }
    }

    public event Action AttackStart;
    public event Action<Weapon> WeaponChanged;
    public bool CanAttack() => _equippedWeapon != null && _equippedWeapon.CanAttack;
    public void StartAttack()
    {
        if (!CanAttack()) return;
        _equippedWeapon.Attack();
        AttackStart?.Invoke();
    }

    public void Equip(WeaponConfiguration weapon, int level)
    {
        Weapon prevWeapon = _equippedWeapon;
        if (weapon != null)
        {
            if (prevWeapon != null && weapon == _equippedWeapon.Config) return;
            Weapon weaponInstance = weapon.CreateWeapon(level);
            //= Instantiate(weapon, slot);
            Transform slot;
            switch (weapon.Type)
            {
                case WeaponType.Melee:
                    slot = _meleeSlot;
                    break;
                case WeaponType.Gun:
                    slot = _gunSlot;
                    break;
                default:
                    throw new NotImplementedException();
            }
            weaponInstance.transform.parent = slot;
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            weaponInstance.transform.localScale = Vector3.one;
            _equippedWeapon = weaponInstance;
            _enemyDetector.SetDetectionRadius(weaponInstance.Stats.range);
            weaponInstance.Equip();
            WeaponChanged?.Invoke(weaponInstance);
        }
        else
        {
            _equippedWeapon = null;
            _enemyDetector.SetDetectionRadius(0);
            WeaponChanged?.Invoke(null);
        }
        if (prevWeapon != null)
        {
            prevWeapon.Unequip();
            Destroy(prevWeapon.gameObject);
        }
    }

    private void Update()
    {
        if (_equippedWeapon == null) return;
        if (!CanAttack()) return;

        RaycastHit info = RaycastForward;
        if (info.collider != null && IsColliderOnEnemyLayer(info.collider))
        {
            _wantToShoot = true;
        }
        else _wantToShoot = false;

        if (_wantToShoot)
        {
            StartAttack();
            _equippedWeapon.DrawEffect(info.point);
        }
    }

    public void OnAttackEventTriggered()
    {
        RaycastHit info = RaycastForward;

        if (info.collider != null && IsColliderOnEnemyLayer(info.collider))
        {
            IDamagable victim = info.collider.GetComponent<IDamagable>();
            _equippedWeapon.DealDamage(victim, gameObject);
        }
    }
}
