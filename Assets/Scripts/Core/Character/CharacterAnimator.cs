using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private string _speedProperty;
    [SerializeField] private float _speedCrossfade = 0.2f;
    [SerializeField] private string _hitReactProperty;
    [SerializeField] private string _deathProperty;
    [SerializeField] private string _attackProperty;
    [SerializeField] private ParticleSystem _damageParticle;
    [SerializeField] private Transform _particlePlayPoint;
    [SerializeField] private float _particleDuration;
    [SerializeField] private ParticleSystem _deathParticle;

    protected IAttackProvider _attackProvider;
    protected Animator _animator;
    protected Character _character;

    protected int _speedPropID;
    protected int _hitTrigID;
    protected int _dieTrigID;
    protected int _attackTrigID;

    protected float _prevMoveSpeed = 0;

    private ObjectPool<ParticleSystem> _damageParticlePool;
    private List<ParticleSystem> _activeParticles;

    private ParticleSystem CreateDamageParticle()
    {
        ParticleSystem instance = Instantiate(_damageParticle, Vector3.zero, Quaternion.identity);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetParticlesFromPool(ParticleSystem particle)
    {
        _activeParticles.Add(particle);
    }
    private void OnReleaseParticlesToPool(ParticleSystem particle)
    {
        _activeParticles.Remove(particle);
    }
    private void OnDestroyParticleFromPool(ParticleSystem particle)
    {
        Destroy(particle.gameObject);
    }

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        _character = GetComponent<Character>();
        _damageParticlePool = new ObjectPool<ParticleSystem>(CreateDamageParticle, 
            OnGetParticlesFromPool, OnReleaseParticlesToPool, 
            OnDestroyParticleFromPool);
        _activeParticles = new List<ParticleSystem>();
    }

    protected void Start()
    {
        _speedPropID = Animator.StringToHash(_speedProperty);
        _hitTrigID = Animator.StringToHash(_hitReactProperty);
        _dieTrigID = Animator.StringToHash(_deathProperty);
        _attackTrigID = Animator.StringToHash(_attackProperty);
        

        _attackProvider.AttackStart += OnAttackPerformed;
        _character.Damaged += OnHitReact;
        _character.Died += OnDied;

    }

    protected void OnEnable()
    {
        // подписаться на события
        _attackProvider.AttackStart += OnAttackPerformed;
        _character.Damaged += OnHitReact;
        _character.Died += OnDied;
    }

    protected void OnDisable()
    {
        // отписаться от событий
        _attackProvider.AttackStart -= OnAttackPerformed;
        _character.Damaged -= OnHitReact;
        _character.Died -= OnDied;
    }
    protected void Update()
    {
        Vector3 characterVelocity = _character.MovementController.Controller.velocity;
        characterVelocity.y = 0;
 
        float moveTwdVal = _character.MovementController.MovementSettings.MaxSpeed / _speedCrossfade;
        float moveSpeed = Mathf.MoveTowards(_prevMoveSpeed, characterVelocity.magnitude, moveTwdVal * Time.deltaTime);
        _prevMoveSpeed = moveSpeed;
        _animator.SetFloat(_speedPropID, moveSpeed);
    }

    private void OnDestroy()
    {
        _damageParticlePool.Clear();
        foreach (var particle in _activeParticles)
        {
            OnDestroyParticleFromPool(particle);
        }
        StopAllCoroutines();
    }


    protected void OnHitReact(float dmg, float hp, GameObject causer)
    {
        Vector3 damageDirection = transform.position - causer.transform.position;
        StartCoroutine(PlayParticle(_particlePlayPoint, damageDirection));
        _animator.SetTrigger(_hitTrigID);
    }

    protected virtual void OnDied(GameObject dead)
    {
        MonoBehaviour attack = GetComponent<IAttackProvider>() as MonoBehaviour;
        attack.StopAllCoroutines();
        _animator.SetFloat(_speedPropID, 0);
        _animator.SetTrigger(_dieTrigID);
        enabled = false;
        _deathParticle?.Emit(_character.ScoreReward);
    }

    protected void OnAttackPerformed()
    {
        _animator.SetTrigger(_attackTrigID);
    }
    
    private IEnumerator PlayParticle(Transform point, Vector3 direction)
    {
        ParticleSystem particles = _damageParticlePool.Get();
        particles.gameObject.SetActive(true);
        particles.transform.position = point.position;
        yield return null;
        particles.transform.LookAt(direction + particles.transform.position);
        particles.Play();
        yield return new WaitForSeconds(_particleDuration);
        particles.gameObject.SetActive(false);
        _damageParticlePool.Release(particles);
        if (_damageParticlePool.CountAll == 0)
            OnDestroyParticleFromPool(particles);
    }
}
