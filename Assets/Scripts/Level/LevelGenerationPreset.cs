using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGenerationPreset", menuName = "Generation/General Settings", order = 0)]
public class LevelGenerationPreset : ScriptableObject
{
    [SerializeField] private List<TileDesc> _tileSheet;
    [SerializeField] private float _tileSize = 24;
    [SerializeField] private int _tileCount;
    [SerializeField] private int _medkitCount;
    [SerializeField] private int _chestCount;

    [SerializeField] private TileDesc _startTile;

    public IEnumerable<TileDesc> AllTiles
    {
        get {
            foreach (var item in _tileSheet)
                yield return item;
        }
    }
    public TileDesc StartTile => _startTile;
    public float TileSize => _tileSize;
    public int TileCount => _tileCount;

    public int MedkitCount => _medkitCount;
    public int ChestCount => _chestCount;
}
