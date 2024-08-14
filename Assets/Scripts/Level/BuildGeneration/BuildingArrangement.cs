using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingArrangement : MonoBehaviour
{
    [SerializeField] private List<Transform> _smallHousePoints;
    [SerializeField] private List<Transform> _largeHousePoints;
    [SerializeField] private List<Transform> _cornerHousePoints;

    public List<Transform> SmallHousePoints => _smallHousePoints;
    public List<Transform> LargeHousePoints => _largeHousePoints;
    public List<Transform> CornerHousePoints => _cornerHousePoints;
}
