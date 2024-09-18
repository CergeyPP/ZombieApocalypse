using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using YG;

[Serializable]
public class WeaponDesc
{
    public WeaponConfiguration config;
    public int level = 1;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<WeaponDesc> _weapons;
    [SerializeField] private List<WeaponDesc> _purchasedWeapons;
    [SerializeField] private int _equippedWeaponIndex;

    public UnityAction OnInventoryChanged;

    public void Awake()
    {
        YandexGame.GetDataEvent += OnSaveDataChanged;
    }
    public void OnSaveDataChanged()
    {
        _purchasedWeapons.Clear();
        if (YandexGame.savesData.purchasedWeapons.Count == 0)
        {
            WeaponSave wSave = new WeaponSave();
            wSave.id = 0;
            wSave.level = 1;
            YandexGame.savesData.purchasedWeapons.Add(wSave);
        }
        foreach (var weaponSave in YandexGame.savesData.purchasedWeapons)
        {
            WeaponDesc desc = _weapons[weaponSave.id];
            desc.level = weaponSave.level;
            _purchasedWeapons.Add(desc);
        }
        _equippedWeaponIndex = YandexGame.savesData.equippedWeaponID;
        OnInventoryChanged?.Invoke();
    }

    public WeaponDesc EquippedWeapon => _weapons[_equippedWeaponIndex];

    public IEnumerable<WeaponDesc> Weapons => _weapons.AsEnumerable();
    public IEnumerable<WeaponDesc> PurchasedWeapons => _purchasedWeapons.AsEnumerable();
    public IEnumerable<WeaponDesc> UnpurchasedWeapons => _weapons.Except(_purchasedWeapons);
}
