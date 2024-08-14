using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IAttackProvider
{
    public event Action AttackStart;
    public void StartAttack();
    public bool CanAttack();
    public void OnAttackEventTriggered();

    public bool enabled { get; set; }
}
