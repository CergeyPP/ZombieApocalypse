using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectItem : MonoBehaviour
{
    [SerializeField] private WeaponView _view;
    [SerializeField] private Button _selectButton;
    [SerializeField] private Color _unselectedColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.gray;

    private void Awake()
    {
        _view.OnWeaponViewUpdated += OnViewUpdated;
    }

    public void OnSelected()
    {
        _view.Inventory.SelectWeapon(_view.Weapon);
    }

    private void OnViewUpdated()
    {
        if (_view.Inventory.EquippedWeapon.config == _view.Weapon)
        {
            _selectButton.interactable = false;
            _selectButton.image.color = _selectedColor;
        }
        else
        {
            _selectButton.interactable = true;
            _selectButton.image.color = _unselectedColor;
        }
    }
}
