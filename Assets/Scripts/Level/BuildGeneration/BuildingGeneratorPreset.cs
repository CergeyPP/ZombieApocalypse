using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingGenerationPreset", menuName = "Generation/BuildingPreset", order = 999)]
public class BuildingGeneratorPreset : ScriptableObject
{
    [SerializeField] private List<GameObject> _smallTownHouses;
    [SerializeField] private List<GameObject> _smallUrbanHouses;
    [SerializeField] private List<GameObject> _largeTownHouses;
    [SerializeField] private List<GameObject> _largeUrbanHouses;
    [SerializeField] private List<GameObject> _cornerTownHouses;
    [SerializeField] private List<GameObject> _cornerUrbanHouses;

    public GameObject GenerateSmallTownHouse(Transform parent)
    {
        if (_smallTownHouses.Count == 0)
            return null;
        return Instantiate(_smallTownHouses[Random.Range(0, _smallTownHouses.Count)], parent);
    }
    public GameObject GenerateLargeTownHouse(Transform parent)
    {
        if (_largeTownHouses.Count == 0)
            return null;
        return Instantiate(_largeTownHouses[Random.Range(0, _largeTownHouses.Count)], parent);
    }

    public GameObject GenerateCornerTownHouse(Transform parent)
    {
        if (_cornerTownHouses.Count == 0)
            return null;
        return Instantiate(_cornerTownHouses[Random.Range(0, _cornerTownHouses.Count)], parent);
    }

    public GameObject GenerateSmallUrbanHouse(Transform parent)
    {
        if (_smallUrbanHouses.Count == 0)
            return null;
        return Instantiate(_smallUrbanHouses[Random.Range(0, _smallUrbanHouses.Count)], parent);
    }
    public GameObject GenerateLargeUrbanHouse(Transform parent)
    {
        if (_largeUrbanHouses.Count == 0)
            return null;
        return Instantiate(_largeUrbanHouses[Random.Range(0, _largeUrbanHouses.Count)], parent);
    }

    public GameObject GenerateCornerUrbanHouse(Transform parent)
    {
        if (_cornerUrbanHouses.Count == 0)
            return null;
        return Instantiate(_cornerUrbanHouses[Random.Range(0, _cornerUrbanHouses.Count)], parent);
    }
}
