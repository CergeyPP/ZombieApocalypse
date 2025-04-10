using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private Character _character;
    [SerializeField] private Transform _decal;
    [SerializeField] private SphereCollider _collider;

    private List<GameObject> _enemies = new List<GameObject>();
    private GameObject _closestEnemy = null;

    public GameObject Target => _closestEnemy;
    public void SetDetectionRadius(float newRadius)
    {
        _collider.radius = newRadius;

        if (newRadius == 0)
            _decal?.gameObject.SetActive(false);
        else if (_decal != null)
        {
            _decal.gameObject.SetActive(true);
            _decal.localScale = Vector3.one * newRadius * 2; // 2 because of width = diameter
        }
    }

    private void Update()
    {
        _closestEnemy = GetClosestEnemy();
    }

    private GameObject GetClosestEnemy()
    {
        GameObject _closestEnemy = null;
        foreach(GameObject enemy in _enemies)
        {
            if (_closestEnemy == null)
                _closestEnemy = enemy;
            else
            {
                Vector3 distanceToClosest = _character.transform.position - _closestEnemy.transform.position;
                Vector3 distance = _character.transform.position - enemy.transform.position;
                if (distance.sqrMagnitude < distanceToClosest.sqrMagnitude)
                {
                    _closestEnemy = enemy;
                }
            }

        }

        return _closestEnemy;
    }

    private bool IsColliderOnEnemyLayer(Collider collider) 
        => ((_enemyLayer >> collider.gameObject.layer) & 1) == 1;

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (IsColliderOnEnemyLayer(other) && damagable != null)
        {
            _enemies.Add(other.gameObject);
            damagable.Died += OnEnemyDied;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if (IsColliderOnEnemyLayer(other) && damagable != null)
        {
            _enemies.Remove(other.gameObject);
            damagable.Died -= OnEnemyDied;
        }
    }

    private void OnEnemyDied(GameObject dead)
    {
        foreach (var enemy in _enemies)
        {
            if (dead == enemy.gameObject)
            {
                _enemies.Remove(enemy);
                if (enemy == _closestEnemy)
                    _closestEnemy = null;

                return;
            }
        }
    }
}
