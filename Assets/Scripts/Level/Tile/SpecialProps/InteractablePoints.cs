using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractablePoints
{
    [SerializeField] private Interactable _endLevelTrigger;
    [SerializeField] private Interactable _medKitPrefab;
    [SerializeField] private Interactable _chestPrefab;
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> _medKitPositions;
    [SerializeField] private List<Transform> _chestPositions;

    public Interactable EndLevelTrigger => _endLevelTrigger;

    public IEnumerable<Interactable> InstantiateMedkits()
    {
        return InstantiateInteracteblesByTransforms(_medKitPrefab, _medKitPositions);
    }

    public IEnumerable<Interactable> InstantiateChests()
    {
        return InstantiateInteracteblesByTransforms(_chestPrefab, _chestPositions);
    }

    private IEnumerable<Interactable> InstantiateInteracteblesByTransforms(Interactable original, IEnumerable<Transform> transforms)
    {
        foreach (var transform in transforms)
        {
            Interactable clone = Object.Instantiate(original, transform.position, transform.rotation);
            yield return clone;
        }
    }
}
