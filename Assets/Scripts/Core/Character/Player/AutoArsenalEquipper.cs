using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoArsenalEquipper : MonoBehaviour
{
    private Inventory _inventory;
    [SerializeField] private WeaponArsenal _arsenal;

    private void OnInventoryUpdated()
    {
        _arsenal.Equip(_inventory.EquippedWeapon.config, _inventory.EquippedWeapon.level);
    }
    private void Awake()
    {
        _inventory = FindAnyObjectByType<Inventory>();
    }

    private void OnEnable()
    {
        _inventory.OnInventoryChanged += OnInventoryUpdated;
    }
    private void OnDisable()
    {
        _inventory.OnInventoryChanged -= OnInventoryUpdated;
    }
}
