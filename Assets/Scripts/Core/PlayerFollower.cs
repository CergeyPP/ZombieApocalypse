using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private float _followSpeedMultiplier = 1.0f;

    private void Start()
    {
        OnObjectTeleported();
    }
    private void LateUpdate()
    {
        FollowObject(false);
    }

    public void OnObjectTeleported()
    {
        FollowObject(true);
    }

    private void FollowObject(bool isTeleporting)
    {
        if (!isTeleporting)
        {
            transform.position = Vector3.Slerp(transform.position, _objectToFollow.position, Time.deltaTime * _followSpeedMultiplier);
        }
        else
        {
            transform.position = _objectToFollow.position;
        }
    }

#if UNITY_EDITOR
    [Header("Editor Helper")]
    [SerializeField] private float _targetDistance;
    [SerializeField] private Transform _cameraChild;

    [ContextMenu("Apply Camera settings")]
    public void PoseCamera()
    {
        Undo.RecordObject(transform, "Apply Camera settings");
        FollowObject(true);
        Undo.RecordObject(_cameraChild, "Apply Camera settings");
        _cameraChild.transform.localPosition = -transform.forward * _targetDistance;
        _cameraChild.localRotation = transform.localRotation;
        transform.localRotation = Quaternion.identity;
    }
#endif
}
