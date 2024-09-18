using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
public class AIController : MonoBehaviour, IAttackProvider, IMovementProvider
{
    [SerializeField] private EnemyDetector _enemyDetector;
    [Header("Attack Settings")]
    [SerializeField] private AttackConfiguration _attackConfig;
    [SerializeField] private float _startAttackDistanceThreshold;
    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private LayerMask _enemyLayer;

    public event Action AttackStart;
    private NavMeshAgent _navMeshAgent;
    private NavMeshObstacle _obstacle;

    private Vector3 _movementDirection;
    private float _lookTargetAngle;

    private Timer _attackTimer;

    private bool _bWasAbleToAttack = false;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _attackTimer = new Timer(this);
    }

    private void Start()
    {
        _obstacle.enabled = false;
        _lookTargetAngle = transform.eulerAngles.y;
    }

    private void OnEnable()
    {
        _navMeshAgent.enabled = true;
        _obstacle.enabled = false;
    }
    private void OnDisable()
    {
        _navMeshAgent.enabled = false;
        _obstacle.enabled = false;
    }

    public Vector3 GetMovementDirection()
    {
        if (!enabled) return Vector3.zero;
        return _movementDirection;
    }

    public float GetTargetLookAngle()
    {
        return _lookTargetAngle;
    }

    private bool IsColliderOnEnemyLayer(Collider collider)
        => ((_enemyLayer >> collider.gameObject.layer) & 1) == 1 && !collider.isTrigger;
    private RaycastHit RaycastForward(float range)
    {
        Debug.DrawLine(_raycastPoint.position, _raycastPoint.position + _raycastPoint.forward * 5);
        RaycastHit info;
        Physics.Raycast(_raycastPoint.position,
            _raycastPoint.forward,
            out info,
            range,
            _enemyLayer, QueryTriggerInteraction.Ignore);

        return info;
    }

    public bool CanAttack()
    {
        return _attackTimer.CanPerform;
    }

    public void StartAttack()
    {
        AttackStart?.Invoke();
        _attackTimer.StartTime(_attackConfig.FireRate);
    }

    public void OnAttackEventTriggered()
    {
        RaycastHit info = RaycastForward(_attackConfig.Range);

        if (info.collider != null && IsColliderOnEnemyLayer(info.collider))
        {
            IDamagable victim = info.collider.GetComponent<IDamagable>();
            if (victim != null)
            {
                victim.RegisterDamage(_attackConfig.Damage, gameObject);
            }
        }
    }

    private Vector3 CalculateMovementDirection()
    {
        Vector3 movementDirection = (_navMeshAgent.steeringTarget - transform.position);
        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;
        return movementDirection;
    }

    public void Update()
    {
        if (_navMeshAgent.enabled)
            _navMeshAgent.destination = FindFirstObjectByType<PlayerMovementProvider>().transform.position;
        _movementDirection = CalculateMovementDirection();
        GameObject enemy = _enemyDetector.Target;
        if (enemy != null)
        {
            Vector3 targetPos = enemy.transform.position;
            Vector3 viewDirection = targetPos - transform.position;
            viewDirection.y = 0;
            _lookTargetAngle = Vector3.SignedAngle(Vector3.forward, viewDirection.normalized, Vector3.up);
        }
        else if (_movementDirection != Vector3.zero)
        {
            _lookTargetAngle = Vector3.SignedAngle(Vector3.forward, _movementDirection, Vector3.up);
        }

        if (!CanAttack())
        {
            _movementDirection = Vector3.zero;
            if (CanAttack() != _bWasAbleToAttack)
            {
                _navMeshAgent.enabled = false;
                _obstacle.enabled = true;

            }
            _bWasAbleToAttack = false;
            return;
        }
        else
        {
            if (CanAttack() != _bWasAbleToAttack)
            {
                _navMeshAgent.enabled = true;
                _obstacle.enabled = false;
            }
            _bWasAbleToAttack = true;
        }

        bool bWantsToAttack = false;

        RaycastHit info = RaycastForward(_attackConfig.Range - _startAttackDistanceThreshold);
        if (info.collider != null && IsColliderOnEnemyLayer(info.collider))
        {
            bWantsToAttack = true;
        }

        if (bWantsToAttack)
        {
            StartAttack();
        }
    }

    private void OnDestroy()
    {
        _obstacle.enabled = false;
    }
}
