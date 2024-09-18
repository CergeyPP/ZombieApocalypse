using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;


public class PlayerMovementProvider : MonoBehaviour, IMovementProvider
{
    [SerializeField] private EnemyDetector _enemyDetector;
    [SerializeField] private bool _turnTowardsEnemyWhileStand = false;
    
    private PlayerInput _playerInput;
    private Vector3 _movementDirection;
    private float _lookTargetAngle = 0;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void Start()
    {
        _lookTargetAngle = transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }
    private void OnDisable()
    {
        _playerInput.Disable();
    }

    public Vector3 GetMovementDirection()
    {
        return _movementDirection;
    }
    public float GetTargetLookAngle()
    {
        return _lookTargetAngle;
    }

    private void Update()
    {
        Vector2 axisMoveInput = _playerInput.InGameActions.Move.ReadValue<Vector2>();
        _movementDirection = axisMoveInput.y * Vector3.forward + axisMoveInput.x * Vector3.right;
        _movementDirection = Vector3.ClampMagnitude(_movementDirection, 1);
        if (_movementDirection != Vector3.zero)
            _lookTargetAngle = Vector3.SignedAngle(Vector3.forward, _movementDirection, Vector3.up);
        if (_turnTowardsEnemyWhileStand)
        {
            GameObject enemy = _enemyDetector.Target;
            if (enemy != null)
            {
                Vector3 targetPos = enemy.transform.position;
                Vector3 viewDirection = targetPos - transform.position;
                viewDirection.y = 0;
                _lookTargetAngle = Vector3.SignedAngle(Vector3.forward, viewDirection.normalized, Vector3.up);
            }
        }
    }
}
