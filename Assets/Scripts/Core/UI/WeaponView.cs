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
    [SerializeField] private TextMeshPro _damageField;
    [SerializeField] private TextMeshPro _rangeField;
    [SerializeField] private TextMeshPro _firerateField;

    public WeaponConfiguration Weapon => _weaponConfig;
    public int WeaponLevel => _weaponLevel;

    public void Awake()
    {
        SetWeaponSettings(_weaponConfig, _weaponLevel);
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
    }
}
