using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPropsGenerationPreset", menuName = "Generation/propsPreset", order = 999)]
public class PropsGenerationPreset : ScriptableObject
{
    [SerializeField] private List<PropsArrangement> _propsPrefab;

    public PropsArrangement GenerateProps(Transform parent)
    {
        if (_propsPrefab.Count == 0)
            return null;
        PropsArrangement prop = _propsPrefab[Random.Range(0, _propsPrefab.Count)];
        return Instantiate(prop, parent);
    }
}
