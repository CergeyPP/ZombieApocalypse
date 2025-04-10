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

    private Wallet _wallet;
    public Wallet Wallet => _wallet;
    public void Awake()
    {
        YandexGame.GetDataEvent += OnSaveDataChanged;
        YandexGame.RewardVideoEvent += OnRewardAdShown;
    }
    private void OnRewardAdShown(int id)
    {
        WeaponDesc upgradedWeapon = _weapons[id];
        UpgradeWeaponForced(upgradedWeapon.config, upgradedWeapon.level);
    }

    public void ShowRewardAdForUpgradeWeapon(WeaponConfiguration weapon)
    {
        if (!IsWeaponAlreadyBought(weapon)) return;
        int weaponIndex = _weapons.FindIndex(item => { return weapon == item.config; });
        WeaponDesc weaponDesc = _weapons[weaponIndex];
        if (!IsAbleToUpgrade(weaponDesc.config, weaponDesc.level)) return;
        YandexGame.RewVideoShow(weaponIndex);
    }

    public bool IsWeaponAlreadyBought(WeaponConfiguration weapon)
    {
        return _purchasedWeapons.Any((item) =>
        {
            return item.config == weapon;
        });
    }

    public bool IsEnoughToBuy(WeaponConfiguration weapon)
    {
        return _wallet.Coins >= weapon.BuyPrice;
    }

    public bool IsAbleToUpgrade(WeaponConfiguration weapon, int requiredLevel)
    {
        return weapon.MaxLevel > requiredLevel;
    }

    public bool IsEnoughToUpgrade(WeaponConfiguration weapon, int requiredLevel)
    {
        return weapon.GetUpgradePrice(requiredLevel) <= _wallet.Coins;
    }

    public void SelectWeapon(WeaponConfiguration weapon)
    {
        if (!IsWeaponAlreadyBought(weapon)) return;

        _equippedWeaponIndex = _weapons.FindIndex(x => x.config == weapon);
        YandexGame.savesData.equippedWeaponID = _equippedWeaponIndex;
        YandexGame.SaveProgress();
        OnInventoryChanged?.Invoke();
    }
    public void BuyWeapon(WeaponConfiguration weapon)
    {
        if (IsWeaponAlreadyBought(weapon)) return;
        if (!IsEnoughToBuy(weapon)) return;

        _wallet.Purchase(weapon.BuyPrice);
        WeaponSave weaponSave = new WeaponSave();
        weaponSave.id = _weapons.FindIndex(item => item.config == weapon);
        weaponSave.level = 1;
        
        YandexGame.savesData.purchasedWeapons.Add(weaponSave);
        YandexGame.savesData.purchasedWeapons = YandexGame.savesData.purchasedWeapons.OrderBy(save => save.id).ToList();
        
        YandexGame.savesData.coins = _wallet.Coins;
        YandexGame.SaveProgress();
        
        OnSaveDataChanged();
        OnWeaponPurchased?.Invoke(_purchasedWeapons[_purchasedWeapons.FindIndex(item => { return item.config == weapon; })]);
    }

    public void UpgradeWeapon(WeaponConfiguration weapon, int upgradeLevel)
    {
        if (!IsWeaponAlreadyBought(weapon)) return;
        
        if (!IsEnoughToUpgrade(weapon, upgradeLevel)) return;

        _wallet.Purchase(weapon.GetUpgradePrice(upgradeLevel));

        UpgradeWeaponForced(weapon, upgradeLevel);
    }

    public void UpgradeWeaponForced(WeaponConfiguration weapon, int upgradeLevel)
    {
        if (!IsAbleToUpgrade(weapon, upgradeLevel)) return;
        int weaponIndex = _weapons.FindIndex(item => item.config == weapon);
        int saveIndex = YandexGame.savesData.purchasedWeapons.FindIndex(save => { return weaponIndex == save.id; });

        _weapons[weaponIndex].level += 1;
        WeaponSave newSave = new WeaponSave();
        newSave.id = YandexGame.savesData.purchasedWeapons[saveIndex].id;
        newSave.level = YandexGame.savesData.purchasedWeapons[saveIndex].level + 1;
        YandexGame.savesData.purchasedWeapons[saveIndex] = newSave;
        YandexGame.savesData.coins = _wallet.Coins;
        YandexGame.SaveProgress();
        OnSaveDataChanged();
        OnWeaponLeveledUp?.Invoke(_weapons[weaponIndex]);
    }

    public void OnSaveDataChanged()
    {
        if (_wallet == null)
        {
            //    if (YandexGame.savesData.coins > _wallet.Coins)
            //        _wallet.Add(YandexGame.savesData.coins - _wallet.Coins);
            //    else
            //        _wallet.Purchase(_wallet.Coins - YandexGame.savesData.coins);
            //else
            _wallet = new Wallet(YandexGame.savesData.coins);
        }
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

    public Action<WeaponDesc> OnWeaponLeveledUp;
    public Action<WeaponDesc> OnWeaponPurchased;

    public WeaponDesc EquippedWeapon => _weapons[_equippedWeaponIndex];

    public IEnumerable<WeaponDesc> Weapons => _weapons.AsEnumerable();
    public IEnumerable<WeaponDesc> PurchasedWeapons => _purchasedWeapons.AsEnumerable();
    public IEnumerable<WeaponDesc> UnpurchasedWeapons => _weapons.Except(_purchasedWeapons);
}
