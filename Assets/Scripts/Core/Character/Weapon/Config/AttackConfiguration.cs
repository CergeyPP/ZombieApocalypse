using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackConfiguration", menuName = "Attack/Confugiration", order = 0)]
public class AttackConfiguration : ScriptableObject
{
    [SerializeField] private float _damage;
    [SerializeField] private float _range;
    [SerializeField] private float _fireRate;

    public float Damage => _damage;
    public float Range => _range;
    public float FireRate => _fireRate;
}
