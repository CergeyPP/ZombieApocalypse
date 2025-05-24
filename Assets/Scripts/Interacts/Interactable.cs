using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private float _destroyTime = 1.0f;
    public UnityEvent<Interactable, GameObject> InteractEvent;
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        InteractEvent?.Invoke(this, other.gameObject);
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        float timer = 0;
        while (timer < _destroyTime)
        {
            yield return null;
            timer += Time.deltaTime;

            transform.localScale = Vector3.one * Mathf.Clamp01(1.0f - timer / _destroyTime);
        }
        Destroy(gameObject);
    }
}
