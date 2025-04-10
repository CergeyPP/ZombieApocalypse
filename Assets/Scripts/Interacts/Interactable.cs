using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public UnityEvent<Interactable, GameObject> InteractEvent;
    private void OnTriggerEnter(Collider other)
    {
        InteractEvent?.Invoke(this, other.gameObject);
        Destroy(gameObject);
    }
}
