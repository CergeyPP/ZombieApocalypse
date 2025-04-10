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
    public List<TileDesc> Tiles;
    public TileWaysFlags Ways;
    public int WaysCount
    {
        get
        {
            int count = 0;
            int flags = (int)Ways;
            while (flags > 0)
            {
                count += flags | 1;
                flags >>= 1;
            }
            return count;
        }
    }
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelGenerationPreset _mainGenerationPreset;
    [SerializeField] private LevelGenerationPreset _tutorialGenerationPreset;
    [SerializeField] private Transform _generationStartTransform;
    [SerializeField] private NavMeshSurface _navMesh;
    [SerializeField] private GameObject _playerSystem;

    private LevelGenerationPreset _generationPreset;

    private List<Road> _generatedRoads;
    private TileRequirements[,] _tileReqsMap;
    private Queue<Vector2Int> _tileQueue;
    private List<TileSample> _completeTiles;

    private GameObject _genPlayerSystem;

    private int GeneratedTilesCount => _completeTiles.Count;
    private int RemainingTileCount => _generationPreset.TileCount - GeneratedTilesCount;
    private int AvailableTileCount => RemainingTileCount - _tileQueue.Count;

    private Interactable _endLevelTrigger;
    public Interactable EndLevelTrigger => _endLevelTrigger;
    private List<Interactable> _chests;
    private List<Interactable> _medkits;
    public List<Interactable> Chests => _chests;
    public List<Interactable> MedKits => _chests;
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
        if (YG.YandexGame.savesData.isTutorCompleted)
        {
            _generationPreset = _mainGenerationPreset;
        }
        else
        {
            _generationPreset = _tutorialGenerationPreset;
        }
        _navMesh.BuildNavMesh();
        GenerateLevel();
        _genPlayerSystem = Instantiate(_playerSystem, _generationStartTransform.position, _generationStartTransform.rotation);
    }
    public void UpdateNavMesh()
    {
        _navMesh.UpdateNavMesh(_navMesh.navMeshData);
    }
    public void ClearLevel()
    {
        if (_genPlayerSystem != null)
        {
            foreach (var item in _generatedRoads)
            {
                Destroy(item.gameObject);
            }
            foreach (var item in _medkits)
            {
                Destroy(item.gameObject);
            }
            foreach (var item in _chests)
            {
                Destroy(item.gameObject);
            }
            _generatedRoads.Clear();
            Destroy(_genPlayerSystem);
            _navMesh.BuildNavMesh();
        }
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

    public void InstantiateRoadEnemies()
    {
        foreach (var item in _generatedRoads)
        {
            item.GenerateEnemies();
        }
    }

    private int chestCount => _chests.Count;
    private int medkitCount => _medkits.Count;

    private void GenerateLevel()
    {
        _chests = new List<Interactable>();
        _medkits = new List<Interactable>();
        _generatedRoads = new List<Road>();
        _tileReqsMap = new TileRequirements[_generationPreset.TileCount * 2 + 1, _generationPreset.TileCount * 2 + 1];
        _tileQueue = new Queue<Vector2Int>();
        TileRequirements startTileReqs = new TileRequirements();
        startTileReqs.NeedWays = _generationPreset.StartTile.TileWays;
        startTileReqs.ExcludedWays = TileWaysFlags.All ^ _generationPreset.StartTile.TileWays;
        Vector2Int startTilePos = Vector2Int.one * _generationPreset.TileCount;
        _tileQueue.Enqueue(startTilePos);
        _tileReqsMap[startTilePos.x, startTilePos.y] = startTileReqs;

        _completeTiles = new List<TileSample>();
        GenerateLevelTheme();

        InstantiateRoads();
    }

    private void OnMedkitPickedUp(Interactable medkit, GameObject causer)
    {
        _medkits.Remove(medkit);
    }
    private void OnChestPickedUp(Interactable chest, GameObject causer)
    {
        _chests.Remove(chest);
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
            List<TileDesc> completeTileDescList = CompleteTileDescription(tileReqs).ToList();
            tileReqs.NeedWays = completeTileDescList[0].TileWays;
            tileReqs.ExcludedWays = TileWaysFlags.All ^ tileReqs.NeedWays;
            tileReqs.IsCompleted = true;
            TileSample sample = new TileSample();
            sample.Tiles = completeTileDescList;
            sample.Position = tilePosition;
            sample.Ways = tileReqs.NeedWays;
            _completeTiles.Add(sample);

            ValidateUpNeightbour(tilePosition, tileReqs);
            ValidateDownNeightbour(tilePosition, tileReqs);
            ValidateLeftNeightbour(tilePosition, tileReqs);
            ValidateRightNeightbour(tilePosition, tileReqs);
        }
    }

    private void InstantiateRoads()
    {
        Vector2Int startTilePos = _completeTiles[0].Position;
        List<TileSample> endPaths = new List<TileSample>();
        foreach(var item in _completeTiles)
        {
            Vector3 roadPosition = _generationStartTransform.position + new Vector3(item.Position.x - _generationPreset.TileCount, 0, item.Position.y - _generationPreset.TileCount) * _generationPreset.TileSize;
            TileDesc resultTile;
            if (GetWayCount(item.Ways) == 1 && startTilePos != item.Position)
            {
                endPaths.Add(item);
                continue;
            }
            else
                resultTile = item.Tiles[Random.Range(0, item.Tiles.Count)];

            Road newRoad = Instantiate(resultTile.RandomRoad, roadPosition, Quaternion.Euler(0, resultTile.TileRotation, 0));
            _generatedRoads.Add(newRoad);

            newRoad.GenerateBuildings();
            newRoad.GenerateProps();
            _chests.AddRange(newRoad.GenerateChests());
            _medkits.AddRange(newRoad.GenerateMedkits());
        }

        TileSample furthest = endPaths.Aggregate((furthest, item) =>
                (furthest.Position - startTilePos).magnitude > (item.Position - startTilePos).magnitude ? furthest : item);
        List<TileDesc> furthestEndLevelTiles = furthest.Tiles.Where(tile => { return tile.HasLevelEscape; }).ToList();
        TileDesc furthestEndLevel = furthestEndLevelTiles[Random.Range(0, furthestEndLevelTiles.Count)];
        Vector3 endLevelPosition = _generationStartTransform.position + new Vector3(furthest.Position.x - _generationPreset.TileCount, 0, furthest.Position.y - _generationPreset.TileCount) * _generationPreset.TileSize;

        Road endLevelRoad = Instantiate(furthestEndLevel.RandomRoad, endLevelPosition, Quaternion.Euler(0, furthestEndLevel.TileRotation, 0));
        _generatedRoads.Add(endLevelRoad);
        endLevelRoad.GenerateBuildings();
        endLevelRoad.GenerateProps();
        _endLevelTrigger = endLevelRoad.EndLevelTrigger;
        _chests.AddRange(endLevelRoad.GenerateChests());
        _medkits.AddRange(endLevelRoad.GenerateMedkits());

        endPaths.Remove(furthest);

        foreach (var item in endPaths)
        {
            Vector3 roadPosition = _generationStartTransform.position + new Vector3(item.Position.x - _generationPreset.TileCount, 0, item.Position.y - _generationPreset.TileCount) * _generationPreset.TileSize;
            List<TileDesc> endPathTiles = item.Tiles.Where(tile => { return !tile.HasLevelEscape; }).ToList();
            TileDesc resultTile = endPathTiles[Random.Range(0, endPathTiles.Count)];

            Road newRoad = Instantiate(resultTile.RandomRoad, roadPosition, Quaternion.Euler(0, resultTile.TileRotation, 0));
            _generatedRoads.Add(newRoad);

            newRoad.GenerateBuildings();
            newRoad.GenerateProps();

            if (chestCount < _generationPreset.ChestCount)
            {
                _chests.AddRange(newRoad.GenerateChests());

            }
            else if (medkitCount < _generationPreset.MedkitCount)
            {
                _medkits.AddRange(newRoad.GenerateChests());
            }
        }

        foreach (var item in _chests)
        {
            item.InteractEvent.AddListener(OnChestPickedUp);
        }
        foreach (var item in _medkits)
        {
            item.InteractEvent.AddListener(OnMedkitPickedUp);
        }
    }
    private IEnumerable<TileDesc> CompleteTileDescription(TileRequirements requirements)
    {
        if (_completeTiles.Count == 0)
        {
            yield return _generationPreset.StartTile;
            yield break;
        }
        else
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

            TileDesc exampleTile = possibleTiles[Random.Range(0, possibleTiles.Count)];

            TileDesc[] resultTiles = possibleTiles.Where(tile => { return tile.TileWays == exampleTile.TileWays; }).ToArray();
            foreach (var item in resultTiles)
            {
                yield return item; 
            }
        }
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
