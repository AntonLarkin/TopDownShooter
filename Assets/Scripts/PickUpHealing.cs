using UnityEngine;

public class PickUpHealing : MonoBehaviour
{
    #region Variables

    [SerializeField] private float healPoints;

    #endregion


    #region Public methods

    public void HealPlayer(float currentHealPoints)
    {
        currentHealPoints += healPoints;
    }

    #endregion

}
