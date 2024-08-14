using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

public struct TileRequirements
{
    public TileWaysFlags NeedWays;
    public TileWaysFlags ExcludedWays;
    public bool IsCompleted;

    public TileRequirements(TileWaysFlags excludedWays = TileWaysFlags.None, TileWaysFlags needWays = TileWaysFlags.None, bool isCompleted = false) : this()
    {
        NeedWays = needWays;
        IsCompleted = isCompleted;
        ExcludedWays = excludedWays;
    }
}

public struct TileSample
{
    public Vector2Int Position;
    public TileDesc Tile;
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelGenerationPreset _generationPreset;
    [SerializeField] private Transform _generationStartTransform;
    [SerializeField] private NavMeshSurface _navMesh;
    [SerializeField] private GameObject _playerSystem;

    private List<Road> _generatedRoads;
    private TileRequirements[,] _tileReqsMap;
    private Queue<Vector2Int> _tileQueue;
    private List<TileSample> _completeTiles;

    private GameObject _genPlayerSystem;

    private int GeneratedTilesCount => _completeTiles.Count;
    private int RemainingTileCount => _generationPreset.TileCount - GeneratedTilesCount;
    private int AvailableTileCount => RemainingTileCount - _tileQueue.Count;

    private int GetWayCount (TileWaysFlags wayFlags)
    {
        int wayCount = 0;
        int wayMask = (int)wayFlags;
        while (wayMask != 0)
        {
            wayCount += wayMask & 1;
            wayMask >>= 1;
        }
        return wayCount;
    }

    public void StartLevel()
    {
        GenerateLevel();
        _navMesh.BuildNavMesh();
        _navMesh.UpdateNavMesh(_navMesh.navMeshData);
        _genPlayerSystem = Instantiate(_playerSystem, _generationStartTransform.position, _generationStartTransform.rotation);
        InstantiateRoadEnemies();
    }
    public void ClearLevel()
    {
        foreach (var item in _generatedRoads)
        {
            Destroy(item.gameObject);
        }
        _generatedRoads.Clear();
        Destroy(_genPlayerSystem);
    }

    public IEnumerable<Character> GetAllEnemies()
    {
        foreach (var road in _generatedRoads)
        {
            if (road.EnemiesOnRoad == null) continue;

            foreach (var enemy in road.EnemiesOnRoad)
                yield return enemy;
        }
    }

    public Character PlayerCharacter => _genPlayerSystem.GetComponentInChildren<Character>();

    private void OnDestroy()
    {
        ClearLevel();
    }

    private void InstantiateRoadEnemies()
    {
        foreach (var item in _generatedRoads)
        {
            item.GenerateEnemies();
        }
    }

    private void GenerateLevel()
    {
        _generatedRoads = new List<Road>();
        _tileReqsMap = new TileRequirements[_generationPreset.TileCount * 2, _generationPreset.TileCount * 2];
        _tileQueue = new Queue<Vector2Int>();
        TileRequirements startTileReqs = new TileRequirements();
        startTileReqs.NeedWays = _generationPreset.StartTileWays;
        startTileReqs.ExcludedWays = TileWaysFlags.All ^ _generationPreset.StartTileWays;
        Vector2Int startTilePos = Vector2Int.one * _generationPreset.TileCount;
        _tileQueue.Enqueue(startTilePos);
        _tileReqsMap[startTilePos.x, startTilePos.y] = startTileReqs;

        _completeTiles = new List<TileSample>();
        GenerateLevelTheme();

        InstantiateRoads();
    }

    private void GenerateLevelTheme()
    {
        while (RemainingTileCount > 0)
        {
            Vector2Int tilePosition = _tileQueue.Peek();
            _tileQueue = new Queue<Vector2Int>(_tileQueue.Where(position => position != tilePosition));
            ref TileRequirements tileReqs = ref _tileReqsMap[tilePosition.x, tilePosition.y];
            if (!tileReqs.NeedWays.HasFlag(TileWaysFlags.Down)) 
            {
                tileReqs.ExcludedWays |= TileWaysFlags.Down;
            }
            TileDesc completeTileDesc = CompleteTileDescription(tileReqs);
            tileReqs.NeedWays = completeTileDesc.TileWays;
            tileReqs.ExcludedWays = TileWaysFlags.All ^ tileReqs.NeedWays;
            tileReqs.IsCompleted = true;
            TileSample sample = new TileSample();
            sample.Tile = completeTileDesc;
            sample.Position = tilePosition;
            _completeTiles.Add(sample);

            ValidateUpNeightbour(tilePosition, tileReqs);
            ValidateDownNeightbour(tilePosition, tileReqs);
            ValidateLeftNeightbour(tilePosition, tileReqs);
            ValidateRightNeightbour(tilePosition, tileReqs);
        }
    }

    private void InstantiateRoads()
    {
        foreach(var item in _completeTiles)
        {
            Vector3 roadPosition = _generationStartTransform.position + new Vector3(item.Position.x - _generationPreset.TileCount, 0, item.Position.y - _generationPreset.TileCount) * _generationPreset.TileSize;

            Road newRoad = Instantiate(item.Tile.RandomRoad, roadPosition, Quaternion.Euler(0, item.Tile.TileRotation, 0));
            _generatedRoads.Add(newRoad);

            newRoad.GenerateBuildings();
            newRoad.GenerateProps();
        }
    }
    private TileDesc CompleteTileDescription(TileRequirements requirements)
    {
         List<TileDesc> possibleTiles = _generationPreset.AllTiles.Where((tileDesc) =>
            {
                bool isPossible = true;
                if (requirements.NeedWays != TileWaysFlags.None)
                {
                    isPossible = (tileDesc.TileWays & requirements.NeedWays) == requirements.NeedWays;
                    if (!isPossible) return false;
                }
                if (requirements.ExcludedWays != TileWaysFlags.None) 
                { 
                    isPossible = (tileDesc.TileWays & requirements.ExcludedWays) == TileWaysFlags.None;
                }

                if (isPossible)
                {
                    //additional conditions
                    int NeedWayCount = GetWayCount(requirements.NeedWays);
                    int TileWayCount = GetWayCount(tileDesc.TileWays);
                    int FreeTileWayCount = TileWayCount - NeedWayCount;
                    if (FreeTileWayCount < 0)
                        throw new System.Exception();

                    if (AvailableTileCount - 1 < FreeTileWayCount)
                        return false;

                    if (GeneratedTilesCount > 0)
                        if (FreeTileWayCount == 0 && AvailableTileCount - 1 > 0)
                            return false;
                    
                }
                return isPossible;
            }).ToList();

        return possibleTiles[Random.Range(0, possibleTiles.Count)];
    }

    private void ValidateUpNeightbour(Vector2Int tilePosition, TileRequirements tileRequirements)
    {
        Vector2Int nextTilePos = tilePosition + Vector2Int.up;
        ref TileRequirements nextTileReqs = ref _tileReqsMap[nextTilePos.x, nextTilePos.y];
        if (!nextTileReqs.IsCompleted)
        {
            if (tileRequirements.NeedWays.HasFlag(TileWaysFlags.Up))
            {
                nextTileReqs.NeedWays |= TileWaysFlags.Down;
                _tileQueue.Enqueue(nextTilePos);
            }
            else
            {
                nextTileReqs.ExcludedWays |= TileWaysFlags.Down;
            }
        }
    }
    private void ValidateDownNeightbour(Vector2Int tilePosition, TileRequirements tileRequirements)
    {
        Vector2Int nextTilePos = tilePosition + Vector2Int.down;
        ref TileRequirements nextTileReqs = ref _tileReqsMap[nextTilePos.x, nextTilePos.y];
        if (!nextTileReqs.IsCompleted)
        {
            if (tileRequirements.NeedWays.HasFlag(TileWaysFlags.Down))
            {
                nextTileReqs.NeedWays |= TileWaysFlags.Up;
                _tileQueue.Enqueue(nextTilePos);
            }
            else
            {
                nextTileReqs.ExcludedWays |= TileWaysFlags.Up;
            }
        }
    }
    private void ValidateLeftNeightbour(Vector2Int tilePosition, TileRequirements tileRequirements)
    {
        Vector2Int nextTilePos = tilePosition + Vector2Int.left;
        ref TileRequirements nextTileReqs = ref _tileReqsMap[nextTilePos.x, nextTilePos.y];
        if (!nextTileReqs.IsCompleted)
        {
            if (tileRequirements.NeedWays.HasFlag(TileWaysFlags.Left))
            {
                nextTileReqs.NeedWays |= TileWaysFlags.Right;
                _tileQueue.Enqueue(nextTilePos);
            }
            else
            {
                nextTileReqs.ExcludedWays |= TileWaysFlags.Right;
            }
        }
    }
    private void ValidateRightNeightbour(Vector2Int tilePosition, TileRequirements tileRequirements)
    {
        Vector2Int nextTilePos = tilePosition + Vector2Int.right;
        ref TileRequirements nextTileReqs = ref _tileReqsMap[nextTilePos.x, nextTilePos.y];
        if (!nextTileReqs.IsCompleted)
        {
            if (tileRequirements.NeedWays.HasFlag(TileWaysFlags.Right))
            {
                nextTileReqs.NeedWays |= TileWaysFlags.Left;
                _tileQueue.Enqueue(nextTilePos);
            }
            else
            {
                nextTileReqs.ExcludedWays |= TileWaysFlags.Left;
            }
        }
    }

}
