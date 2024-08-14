using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFX : WeaponFX
{
    [SerializeField] private MeshRenderer _mesh;

    public override IEnumerator StartFX(Transform startPoint, Vector3 endPoint)
    {
        transform.SetParent(startPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
        
        yield return new WaitForSeconds(_dilation);
        _mesh.enabled = true;
        yield return new WaitForSeconds(_duration);
        _mesh.enabled = false;
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
