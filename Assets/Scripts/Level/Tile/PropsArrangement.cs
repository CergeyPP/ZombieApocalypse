using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PropsArrangement : MonoBehaviour
{
    [SerializeField] private Character _basicEnemy;
    [SerializeField] private List<Transform> _basicEnemySpawnPositions;

    private List<Character> _genEnemies;
    public IEnumerable<Character> EnemiesOnRoad
    {
        get
        {
            foreach (var enemy in _genEnemies)
            {
                yield return enemy;
            }
        }
    }

    private Collider _collider;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _genEnemies = new List<Character>();
    }
    public void InstantiateEnemies()
    {
        foreach (var item in _basicEnemySpawnPositions)
        {
            Vector3 worldPosition = item.position;

            Character basicEnemy = Instantiate(_basicEnemy, worldPosition, Quaternion.identity);
            basicEnemy.GetComponent<IMovementProvider>().enabled = false;
            _genEnemies.Add(basicEnemy);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in _genEnemies)
        {
            item.GetComponent<IMovementProvider>().enabled = true;
        }
        _collider.enabled = false;
    }
}
