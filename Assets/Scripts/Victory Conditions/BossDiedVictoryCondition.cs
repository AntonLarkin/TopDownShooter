using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class BossDiedVictoryCondition : VictoryCondition
{
    #region Variables

    [SerializeField] private BaseUnit boss;

    #endregion


    #region Unity lifecycle

    private void OnEnable()
    {
        boss.OnDied += Boss_OnDied;
    }

    private void OnDisable()
    {
        boss.OnDied -= Boss_OnDied;
    }

    #endregion


    #region Private methods

    private IEnumerator OnVictoryLoadNextScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadNextScene();
    }

    #endregion


    #region Event handlers

    private void Boss_OnDied()
    {
        InvokeOnCompliteCondition();
    }

    #endregion
}
