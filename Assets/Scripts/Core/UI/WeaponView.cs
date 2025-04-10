using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private WeaponConfiguration _weaponConfig;
    [SerializeField] private int _weaponLevel;
    [SerializeField] private Image _imageForIcon;
    [SerializeField] private TMP_Text _damageField;
    [SerializeField] private TMP_Text _rangeField;
    [SerializeField] private TMP_Text _firerateField;

    public WeaponConfiguration Weapon => _weaponConfig;
    public int WeaponLevel => _weaponLevel;

    private Inventory _inventory;
    public Inventory Inventory => _inventory;
    public Action OnWeaponViewUpdated;

    private void Awake()
    {
        SetWeaponSettings(_weaponConfig, _weaponLevel);
        _inventory = FindAnyObjectByType<Inventory>();
        _inventory.OnWeaponPurchased += OnWeaponActionPerformed;
        _inventory.OnWeaponLeveledUp += OnWeaponActionPerformed;
        _inventory.OnInventoryChanged += RecalculateView;
    }

    private void OnDestroy()
    {
        _inventory.OnWeaponPurchased -= OnWeaponActionPerformed;
        _inventory.OnWeaponLeveledUp -= OnWeaponActionPerformed;
        _inventory.OnInventoryChanged -= RecalculateView;
    }

    public void SetWeaponSettings(WeaponConfiguration weaponConfig, int weaponLevel)
    {
        _weaponConfig = weaponConfig;
        _weaponLevel = weaponLevel;
        if (_weaponConfig != null)
        {
            _imageForIcon.sprite = _weaponConfig.IconUI;
            RecalculateView();
        }
    }

    public void OnWeaponActionPerformed(WeaponDesc weaponDesc)
    {
        if (weaponDesc.config == _weaponConfig)
        {
            _weaponLevel = weaponDesc.level;
            RecalculateView();
        }
    }

    public void RecalculateView()
    {
        WeaponStat stats = _weaponConfig.CreateWeaponStat(_weaponLevel);
        if (_damageField != null)
        {
            _damageField.text = stats.damage.ToString();
        }
        if (_rangeField != null)
        {
            _rangeField.text = stats.range.ToString();
        }
        if (_firerateField != null)
        {
            _firerateField.text = stats.fireRate.ToString();
        }
        OnWeaponViewUpdated?.Invoke();
    }
}
