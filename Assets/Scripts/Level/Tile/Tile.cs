using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum TileWaysFlags : byte
{
    None = 0x0,
    Up = 0x1,
    Down = 0x2,
    Left = 0x4,
    Right = 0x8,
    All = 0xF,
}

[Serializable]
public class TileDesc
{
    [SerializeField] private List<Road> _roadVariants;
    [SerializeField] private float _tileRotation;
    [SerializeField] private TileWaysFlags _tileWays;

    public Road RandomRoad => _roadVariants[UnityEngine.Random.Range(0, _roadVariants.Count)];
    public TileWaysFlags TileWays => _tileWays;
    public float TileRotation => _tileRotation;
}
