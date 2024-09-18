using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private RectTransform _content;
    [SerializeField] private WeaponView _weaponViewPrefab;
    [SerializeField] private bool _showOnlyPurchased;

    private List<WeaponView> _instantiatedViews;

    private void Awake()
    {
        _instantiatedViews = new List<WeaponView>();
    }

    private void OnEnable()
    {
        _inventory.OnInventoryChanged += OnInventoryChanged;
    }
    private void OnDisable()
    {
        _inventory.OnInventoryChanged -= OnInventoryChanged;
    }

    public void OnInventoryChanged()
    {
        _instantiatedViews.Clear();
        IEnumerable<WeaponDesc> weapons;
        if (_showOnlyPurchased)
        {
            weapons = _inventory.PurchasedWeapons;
        }
        else
        {
            weapons = _inventory.Weapons;
        }

        foreach (var weapon in weapons)
        {
            WeaponView view = Instantiate(_weaponViewPrefab, _content);
            view.SetWeaponSettings(weapon.config, weapon.level);
        }
    }
}
