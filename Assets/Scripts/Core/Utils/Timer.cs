using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timer
{
    private MonoBehaviour _context;

    private bool _canPerform;
    public bool CanPerform => _canPerform;
    public Timer(MonoBehaviour context)
    {
        _context = context;
        _canPerform = true;
    }

    private IEnumerator Cooldown(float dilation)
    {
        yield return new WaitForSeconds(dilation);
        _canPerform = true;
    }

    public void StartTime(float dilation)
    {
        if (_canPerform)
        {
            _canPerform = false;
            _context.StartCoroutine(Cooldown(dilation));
        }
    }
}
