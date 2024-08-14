using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimator : CharacterAnimator
{
    [SerializeField] private AIController _attackController;

    protected void Awake()
    {
        base.Awake();
        _attackProvider = _attackController;
    }
}
