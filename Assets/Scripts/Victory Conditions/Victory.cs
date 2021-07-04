using System;
using System.Collections;
using UnityEngine;

public class Victory : MonoBehaviour
{
    #region Variables

    [SerializeField] private VictoryCondition[] conditions;

    #endregion


    #region Events
    
    public static event Action OnVictory;

    #endregion

    #region Unity lifecycle

    private void OnEnable()
    {
        ApplyWithConditions(condition => condition.OnCompleteCondition += VictoryCondition_OnCompliteCondition);
    }

    private void OnDisable()
    {
        ApplyWithConditions(condition => condition.OnCompleteCondition -= VictoryCondition_OnCompliteCondition);
    }

    #endregion


    #region Private methods

    private void ApplyWithConditions(Action<VictoryCondition> action)
    {
        if (conditions != null)
        {
            foreach(var condition in conditions)
            {
                action?.Invoke(condition);
            }
        }
    }

    private IEnumerator OnVictoryLoadNextScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadNextScene();
    }

    #endregion


    #region Event handlers

    private void VictoryCondition_OnCompliteCondition()
    {
        OnVictory?.Invoke();
        StartCoroutine(OnVictoryLoadNextScene());
    }

    #endregion

}
