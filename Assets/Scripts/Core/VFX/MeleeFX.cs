using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MeleeFX : WeaponFX
{
    [SerializeField] private TrailRenderer _trail;
    public override IEnumerator StartFX(Transform startPoint, Vector3 endPoint)
    {
        transform.SetParent(startPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
        _trail.enabled = true;
        yield return new WaitForSeconds(_dilation);
        _trail.emitting = true;
        yield return new WaitForSeconds(_duration);
        transform.parent = null;
        _trail.emitting = false;
        yield return new WaitForSeconds(_trail.time);

        gameObject.SetActive(false);
        _trail.enabled = false;
    }
}
