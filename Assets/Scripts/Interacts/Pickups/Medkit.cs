using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    [SerializeField] private Interactable _interactor;
    [SerializeField] private float _healthRestoreAmount;

    private void Start()
    {
        _interactor.InteractEvent.AddListener(OnInteract);
    }

    private void OnInteract(Interactable trigger, GameObject gameObject)
    {
        Health _health = gameObject.GetComponent<Health>();
        if ( _health != null )
        {
            _health.Heal(_healthRestoreAmount, gameObject);
        }
    }
}
