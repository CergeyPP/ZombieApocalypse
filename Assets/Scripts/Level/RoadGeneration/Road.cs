using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] private PropsGenerationPreset _propsPreset;
    [SerializeField] private BuildingGeneratorPreset _buildingGenerator;
    [SerializeField] private BuildingArrangementPreset _houseArranmegentPreset;

    private PropsArrangement _props;
    private BuildingArrangement _buildArrangement;
    private List<GameObject> _buildings;

    public IEnumerable<Character> EnemiesOnRoad => _props?.EnemiesOnRoad;

    public PropsArrangement Props => _props;

    public void GenerateProps()
    {
        if (_propsPreset == null)
            return;
        _props = _propsPreset.GenerateProps(transform);
    }

    public void GenerateBuildings()
    {
        if (_buildingGenerator == null)
            return;
        if (_houseArranmegentPreset == null)
            return;

        _buildArrangement = _houseArranmegentPreset.GenerateArrangement(transform);
        _buildings = new List<GameObject>();
        foreach (var point in _buildArrangement.SmallHousePoints)
        {
            _buildings.Add(_buildingGenerator.GenerateSmallTownHouse(point));
        }
        foreach (var point in _buildArrangement.LargeHousePoints)
        {
            _buildings.Add(_buildingGenerator.GenerateLargeTownHouse(point));
        }
        foreach (var point in _buildArrangement.CornerHousePoints)
        {
            _buildings.Add(_buildingGenerator.GenerateCornerTownHouse(point));
        }
    }

    public void GenerateEnemies()
    {
        if (_props != null)
            _props.InstantiateEnemies();
    }

    public IEnumerable<Interactable> GenerateMedkits()
    {
        if (_props != null)
            return _props.InstantiateMedkits();
        else
            return Enumerable.Empty<Interactable>();
    }
    public IEnumerable<Interactable> GenerateChests()
    {
        if (_props != null)
            return _props.InstantiateChests();
        else
            return Enumerable.Empty<Interactable>();
    }
    public Interactable EndLevelTrigger => _props.EndLevelTrigger;

    public void OnDestroy()
    {
        Destroy(_buildArrangement.gameObject);
        foreach (var item in _buildings)
        {
            Destroy(item.gameObject);
        }
        _buildings = null;
        if (_props != null)
            Destroy(_props.gameObject);
    }
}
