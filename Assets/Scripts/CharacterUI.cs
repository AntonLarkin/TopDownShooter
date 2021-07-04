using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    #region Variables

    [SerializeField] private BaseUnit baseUnit;
    [SerializeField] private Image image;

    #endregion


    #region Unity lifecycle

    private void OnEnable()
    {
        baseUnit.OnChanged += BaseUnit_OnChanged;
    }
    private void OnDisable()
    {
        baseUnit.OnChanged -= BaseUnit_OnChanged;
    }

    private void Update()
    {
        ReloadHealthBar(baseUnit.currentHealPoints, baseUnit.maxHealPoints);
    }

    #endregion


    #region Private methods

    private void UpdateUi(float currentHp, float maxHp)
    {
        if (currentHp >= 0)
        {
            var fillAmount = currentHp / maxHp;
            image.fillAmount = fillAmount;
        }
        if (currentHp <= 0)
        {
            image.fillAmount = 0;
            gameObject.GetComponentInChildren<Image>().enabled = false;
        }
    }

    private void ReloadHealthBar(float currentHp,float maxHp)
    {
        if (currentHp == maxHp)
        {
            gameObject.GetComponentInChildren<Image>().enabled = true;

            image.fillAmount = 1;
        }

    }

    #endregion


    #region Event handlers

    private void BaseUnit_OnChanged(float currentHp,float maxHp)
    {
        UpdateUi(currentHp, maxHp);
    }

    #endregion

}
