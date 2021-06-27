using UnityEngine;
using System;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;

    public event Action OnReloadButton;

    public void OnReloadButtonClicked()
    {
        OnReloadButton?.Invoke();
    }
}
