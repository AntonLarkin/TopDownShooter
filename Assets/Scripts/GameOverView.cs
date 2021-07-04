using UnityEngine;
using System;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    #region Variables

    [Header("UI")]

    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    #endregion

    #region Events

    public static event Action OnRestartButton;

    #endregion

    #region Unity lifecycle

    void Start()
    {
        restartButton.onClick.AddListener(RestartClickHandler);
        exitButton.onClick.AddListener(ExitClickHandler);
    }

    #endregion

    #region Event Handlers

    private void RestartClickHandler()
    {
        OnRestartButton?.Invoke();
    }

    private void ExitClickHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    #endregion
}