using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public enum WeaponType
{
    Melee,
    Gun
}

[CreateAssetMenu(fileName = "NewWeaponConfiguration", menuName = "Weapon/Weapon Confugiration", order = 0)]
public class WeaponConfiguration : ScriptableObject
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private AttackConfiguration _attack;
    [SerializeField] private WeaponFX _effect;


    public WeaponType Type => _type;
    public AttackConfiguration AttackInfo => _attack;

    private ObjectPool<WeaponFX> _trailPool;
    private WeaponFX CreateEffect()
    {
        WeaponFX instance = Instantiate(_effect, Vector3.zero, Quaternion.identity);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnEnable()
    {
        if (_trailPool == null)
            _trailPool = new ObjectPool<WeaponFX>(CreateEffect);
    }

    public IEnumerator PlayFX(Transform startTransform, Vector3 end)
    {
        WeaponFX instance = _trailPool.Get().GetComponent<WeaponFX>();
        yield return instance.StartFX(startTransform, end);
        _trailPool.Release(instance);
    }
}
