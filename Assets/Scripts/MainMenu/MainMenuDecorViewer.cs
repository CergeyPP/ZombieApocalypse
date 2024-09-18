using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDecorViewer : MonoBehaviour
{
    [SerializeField] private Camera _menuCamera;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
