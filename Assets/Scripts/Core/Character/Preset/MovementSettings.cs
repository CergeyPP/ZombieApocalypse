using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementsSettings", menuName = "Characher Presets/Movement Settings", order = 0)]
public class MovementSettings : ScriptableObject
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _angularSpeed;

    public float MaxSpeed => _maxSpeed;
    public float AngularSpeed => _angularSpeed;
}
