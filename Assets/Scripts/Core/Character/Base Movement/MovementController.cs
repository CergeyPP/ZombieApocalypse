using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    private CharacterController _cc;
    private MovementSettings _moveSettings;
    private IMovementProvider _movementProvider;

    public MovementController(CharacterController cc, IMovementProvider movementProvider, MovementSettings movementSettings)
    {
        _cc = cc;
        _moveSettings = movementSettings;
        _movementProvider = movementProvider;
    }
    public MovementSettings MovementSettings => _moveSettings;
    public CharacterController Controller => _cc;

    public Vector3 HandleMovement(float deltaTime)
    {
        Vector3 totalMovementDirection = _movementProvider.GetMovementDirection();
        
        Vector3 totalMovementXZ = totalMovementDirection * MovementSettings.MaxSpeed;

        Vector3 upMovement = Vector3.down * 0.05f;
        if (!_cc.isGrounded)
        {
            upMovement = (Vector3.up * _cc.velocity.y + Physics.gravity * deltaTime) * deltaTime; 
        }

        return totalMovementXZ * deltaTime + upMovement;
    }

    public Quaternion HandleLookRotation(float deltaTime)
    {
        Quaternion targetRotation = Quaternion.AngleAxis(_movementProvider.GetTargetLookAngle(), Vector3.up);
        Quaternion currentRotation = _cc.transform.rotation;

        Quaternion resultRotation = Quaternion.RotateTowards(currentRotation, targetRotation, deltaTime * MovementSettings.AngularSpeed);
        return resultRotation;
    }
}
