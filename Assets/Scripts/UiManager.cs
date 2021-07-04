using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{

    #region Variables

    [Header("UI")]
    [SerializeField] private GameObject gameOverView;

    private Player player;
    private bool isViewActive;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        GameOverView.OnRestartButton += GameOverView_OnRestartButton;
        player.OnDied += Player_OnDied;
    }

    private void OnDisable()
    {
        GameOverView.OnRestartButton -= GameOverView_OnRestartButton;
        player.OnDied -= Player_OnDied;
    }

    #endregion

    #region Private methods

    private void SetActive(bool isActive)
    {

    }

    #endregion

    #region Public methods

    public void ShowGameOverView()
    {
        gameOverView.SetActive(true);
    }

    public void HideGameOverView()
    {
        gameOverView.SetActive(false);
    }

    #endregion

    #region Event handlers

    private void Player_OnDied()
    {
        gameOverView.SetActive(true);
    }

    private void GameOverView_OnRestartButton()
    {
        HideGameOverView();
    }

    #endregion
}
