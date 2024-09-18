using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum WeaponType
{
    Melee,
    Gun
}

[CreateAssetMenu(fileName = "NewWeaponConfiguration", menuName = "Weapon/Weapon Confugiration", order = 0)]
public class WeaponConfiguration : ScriptableObject
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private AttackConfiguration _attack;
    [SerializeField] private WeaponFX _effect;
    [Header("Build")]
    [SerializeField] private float _damageIncreaseByLevel = 1;
    [SerializeField] private int _maxLevel = 10;
    [SerializeField] private int _startUpgradePrice;
    [SerializeField] private int _upgradePriceIncrease;
    [Header("View")]
    [SerializeField] private Weapon _weaponPrefab;
    [SerializeField] private Sprite _uiIcon;


    public WeaponType Type => _type;
    public AttackConfiguration AttackInfo => _attack;
    public Sprite IconUI => _uiIcon;

    private ObjectPool<WeaponFX> _trailPool;

    public WeaponStat CreateWeaponStat(int level)
    {
        WeaponStat stats = new WeaponStat();
        stats.damage = _attack.Damage + (level - 1) * _damageIncreaseByLevel;
        stats.range = _attack.Range;
        stats.fireRate = _attack.FireRate;
        return stats;
    }
    public Weapon CreateWeapon(int level)
    {
        WeaponStat stats = CreateWeaponStat(level);
        Weapon weapon = Instantiate(_weaponPrefab);
        weapon.SetStats(stats);
        return weapon;
    }

    public int GetUpgradePrice(int currentLevel)
    {
        return _startUpgradePrice + (currentLevel - 1) * _upgradePriceIncrease;
    }

    private WeaponFX CreateEffect()
    {
        WeaponFX instance = Instantiate(_effect, Vector3.zero, Quaternion.identity);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnEnable()
    {
        if (_trailPool == null)
            _trailPool = new ObjectPool<WeaponFX>(CreateEffect);
    }

    public IEnumerator PlayFX(Transform startTransform, Vector3 end)
    {
        WeaponFX instance = _trailPool.Get().GetComponent<WeaponFX>();
        yield return instance.StartFX(startTransform, end);
        _trailPool.Release(instance);
    }
}
