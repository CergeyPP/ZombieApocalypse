using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGenerationPreset", menuName = "Generation/General Settings", order = 0)]
public class LevelGenerationPreset : ScriptableObject
{
    [SerializeField] private List<TileDesc> _tileSheet;
    [SerializeField] private TileWaysFlags _startTileWays;
    [SerializeField] private float _tileSize = 24;
    [SerializeField] private int _tileCount;

    public IEnumerable<TileDesc> AllTiles
    {
        get {
            foreach (var item in _tileSheet)
                yield return item;
        }
    }
    public TileWaysFlags StartTileWays => _startTileWays;
    public float TileSize => _tileSize;
    public int TileCount => _tileCount;
}
