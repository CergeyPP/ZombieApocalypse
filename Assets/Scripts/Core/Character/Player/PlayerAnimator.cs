using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator
{
    [SerializeField] private WeaponArsenal _weaponry;
    [SerializeField] private string _interactProperty;
    [SerializeField] private string _weaponLocomotionLayer;
    [SerializeField] private string _meleeWeaponLayer;
    [SerializeField] private string _gunLayer;

    private int _interactBoolID;

    private int _weaponLocoLayerID;
    private int _meleeWeaponLayerID;
    private int _gunLayerID;

    protected void Awake()
    {
        base.Awake();
        _attackProvider = _weaponry;
    }

    protected void Start()
    {
        base.Start();

        _interactBoolID = Animator.StringToHash(_interactProperty);
       
        _weaponLocoLayerID = _animator.GetLayerIndex(_weaponLocomotionLayer);
        _meleeWeaponLayerID = _animator.GetLayerIndex(_meleeWeaponLayer);
        _gunLayerID = _animator.GetLayerIndex(_gunLayer);

        _weaponry.WeaponChanged += OnWeaponChanged;
    }

    protected void OnEnable()
    {
        _weaponry.WeaponChanged += OnWeaponChanged;
    }
    protected void OnDisable()
    {
        _weaponry.WeaponChanged -= OnWeaponChanged;
    }

    protected override void OnDied(IDamagable dead) 
    {
        DeactivateAllLayers();
        base.OnDied(dead);
    }
    private void OnWeaponChanged(Weapon weapon)
    {
        if (weapon == null)
        {
            ActivateMeleeLayers();
            return;
        }

        switch (weapon.Config.Type)
        {
            case WeaponType.Melee:
                ActivateMeleeLayers();
                break;
            case WeaponType.Gun:
                ActivateGunLayers();
                break;
        }
    }
    private void ActivateGunLayers()
    {
        _animator.SetLayerWeight(_weaponLocoLayerID, 1);
        _animator.SetLayerWeight(_gunLayerID, 1);
        _animator.SetLayerWeight(_meleeWeaponLayerID, 0);
    }

    private void ActivateMeleeLayers()
    {
        _animator.SetLayerWeight(_weaponLocoLayerID, 0);
        _animator.SetLayerWeight(_gunLayerID, 0);
        _animator.SetLayerWeight(_meleeWeaponLayerID, 1);
    }

    private void DeactivateAllLayers()
    {
        for (int layerIndex = 0; layerIndex < _animator.layerCount; layerIndex++)
        {
            _animator.SetLayerWeight(layerIndex, 0);
        }
    }
}
