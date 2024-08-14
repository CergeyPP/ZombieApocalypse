using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class WeaponFX : MonoBehaviour
{
    [SerializeField] protected float _dilation;
    [SerializeField] protected float _duration;

    abstract public IEnumerator StartFX(Transform startPoint, Vector3 endPoint);
}
