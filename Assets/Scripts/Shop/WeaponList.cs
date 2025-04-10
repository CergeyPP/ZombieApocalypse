using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeaponList : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private RectTransform _content;
    [SerializeField] private WeaponView _weaponViewPrefab;
    [SerializeField] private bool _showOnlyPurchased;

    private List<WeaponView> _instantiatedViews;

    private void Awake()
    {
        _instantiatedViews = new List<WeaponView>();
    }

    private void Start()
    {
        if (_inventory.Wallet != null)
            OnInventoryChanged();
        _inventory.OnInventoryChanged += OnInventoryChanged;
        _inventory.OnWeaponPurchased += OnWeaponPurchased;
    }

    public void OnWeaponPurchased(WeaponDesc _)
    {
        if (_showOnlyPurchased)
            OnInventoryChanged();
    }

    public void OnInventoryChanged()
    {
        bool isCountWeaponChanged = false;
        if (_showOnlyPurchased)
        {
            if (_instantiatedViews.Count != _inventory.PurchasedWeapons.ToList().Count)
                isCountWeaponChanged = true;
        }
        else if (_instantiatedViews.Count != _inventory.Weapons.ToList().Count)
            isCountWeaponChanged = true;

        if (isCountWeaponChanged)
        {
            foreach (var item in _instantiatedViews)
            {
                Destroy(item.gameObject);
            }
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
                _instantiatedViews.Add(view);
            }
            _scrollbar.value = 0;
        } 
    }
}
