using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public event Action<Interactable, GameObject> InteractEvent;
    private void OnTriggerEnter(Collider other)
    {
        InteractEvent?.Invoke(this, other.gameObject);
        Destroy(gameObject);
    }
}
