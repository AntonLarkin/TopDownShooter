using System;
using UnityEngine;

public abstract class VictoryCondition : MonoBehaviour
{

    #region Events

    public event Action OnCompleteCondition;

    #endregion


    #region Private methods

    protected void InvokeOnCompliteCondition()
    {
        OnCompleteCondition?.Invoke();
    }

    #endregion

}
