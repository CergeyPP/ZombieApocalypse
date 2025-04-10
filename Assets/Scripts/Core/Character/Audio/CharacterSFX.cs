using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(IAttackProvider))]
public class CharacterSFX : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Health _health;
    [Header("Character clips")]
    [SerializeField] private AudioClip _footstep;
    [SerializeField] private AudioClip _attackSFX;
    [SerializeField] private AudioClip _hitSFX;
    [SerializeField] private AudioClip _deathSFX;

    private IAttackProvider _attackProvider;
    private void Awake()
    {
        _attackProvider = GetComponent<IAttackProvider>();
    }

    private void OnEnable()
    {
        _attackProvider.AttackStart += OnAttackStarted;
        _health.Damaged += OnHit;
        _health.Died += OnDied;
    }

    private void OnDisable()
    {
        _attackProvider.AttackStart -= OnAttackStarted;
        _health.Damaged -= OnHit;
        _health.Died -= OnDied;
    }

    private void OnDied(GameObject obj)
    {
        PlaySound(_deathSFX);
    }

    private void OnHit(float damage, float health, GameObject causer)
    {
        PlaySound(_hitSFX);
    }

    private void OnAttackStarted()
    {
        WeaponArsenal arsenal = _attackProvider as WeaponArsenal;
        if (arsenal != null)
        {
            PlaySound(arsenal.EquippedWeapon.Config.WeaponSFX);
        }
        else
        {
            PlaySound(_attackSFX);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        _audioSource.PlayOneShot(clip);
    }
}
