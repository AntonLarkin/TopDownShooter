using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    #region Variables

    [SerializeField] private BaseUnit boss;

    private float bossHp;

    #endregion


    #region Unity lifecycle

    private void Update()
    {
        if (boss.currentHealPoints <= 0)
        {
            StartCoroutine(OnVictoryLoadNextScene());
        }
    }

    #endregion


    #region Private methods

    private IEnumerator OnVictoryLoadNextScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.LoadNextScene();
    }

    #endregion
}
