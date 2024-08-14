using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(IMovementProvider))]
[RequireComponent(typeof(IAttackProvider))]
[RequireComponent(typeof(Health))]
public class Character : MonoBehaviour
{
    [SerializeField] private MovementSettings _movementSettings;

    private IMovementProvider _movementProvider;
    private IAttackProvider _attackProvider;
    private CharacterController cc;
    private MovementController _movementController;
    private Health _health;
    public MovementController MovementController => _movementController;
    public Health Health => _health;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        _movementProvider = GetComponent<IMovementProvider>();
        _attackProvider = GetComponent<IAttackProvider>();
        _movementController = new MovementController(cc, _movementProvider, _movementSettings);
        _health = GetComponent<Health>();
    }

    void Update()
    {
        transform.rotation = _movementController.HandleLookRotation(Time.deltaTime);
        Vector3 moveVector = _movementController.HandleMovement(Time.deltaTime);
        cc.Move(moveVector);
    }

    private void OnEnable()
    {
        // подписаться на события
        _health.Damaged += OnDamaged;
        _health.Died += OnDied;
        _health.Healed += OnHealed;
    }
    private void OnDisable()
    {
        // отписаться от событий
        _health.Damaged -= OnDamaged;
        _health.Died -= OnDied;
        _health.Healed -= OnHealed;
    }

    public event Action<float, float, GameObject> Damaged;
    public event Action<float, float, GameObject> Healed;
    public event Action<IDamagable> Died;

    private void OnDamaged(float dmg, float hp, GameObject causer)
    {
        Damaged?.Invoke(dmg, hp, causer);
        if (!_attackProvider.enabled)
            _attackProvider.enabled = true;
        if (!_movementProvider.enabled)
            _movementProvider.enabled = true;
    }
    private void OnDied(IDamagable dead)
    {
        cc.enabled = false;
        enabled = false;
        _movementController = null;

        MonoBehaviour attackProvider = GetComponent<IAttackProvider>() as MonoBehaviour;
        if (attackProvider != null)
            attackProvider.enabled = false;
        MonoBehaviour movementProvider = GetComponent<IMovementProvider>() as MonoBehaviour;
        if (movementProvider != null)
            movementProvider.enabled = false;

        StartCoroutine(DeathDelay(5));
        Died?.Invoke(_health);
    }
    private void OnHealed(float heal, float hp, GameObject causer)
    {
        Healed?.Invoke(heal, hp, causer);
    }

    private IEnumerator DeathDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
