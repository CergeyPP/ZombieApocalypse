using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArrangementPreset", menuName = "Generation/BuildingArramgement", order = 999)]
public class BuildingArrangementPreset : ScriptableObject
{
    [SerializeField] private List<BuildingArrangement> _arrangements;

    public BuildingArrangement GenerateArrangement(Transform parent)
    {
        return Instantiate(_arrangements[Random.Range(0, _arrangements.Count)], parent);
    }
}
