using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementProvider 
{
    public Vector3 GetMovementDirection();
    public float GetTargetLookAngle();
    public bool enabled { get; set; }
}
