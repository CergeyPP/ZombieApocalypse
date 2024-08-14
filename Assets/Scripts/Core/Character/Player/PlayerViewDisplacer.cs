using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewDisplacer : MonoBehaviour
{
    [SerializeField] private Character _player;
    [SerializeField] private PlayerMovementProvider _playerMovementProvider;
    [SerializeField] private float _maxDisplacement;

    private MovementController _movementController;
    private float _currentDisplacement = 0;

    private void Start()
    {
        _movementController = _player.MovementController;
    }

    private void LateUpdate()
    {
        Vector3 playerVelocity = _movementController.Controller.velocity;
        playerVelocity.y = 0;
        //playerVelocity /= Time.deltaTime;
        _currentDisplacement = _maxDisplacement * playerVelocity.magnitude / _movementController.MovementSettings.MaxSpeed;
        Quaternion rotation = Quaternion.AngleAxis(_playerMovementProvider.GetTargetLookAngle(), Vector3.up);
        transform.position = _player.transform.position + rotation * Vector3.forward * _currentDisplacement;
    }


}
