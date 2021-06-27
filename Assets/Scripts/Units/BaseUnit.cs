using UnityEngine;
using System;

public abstract class BaseUnit : MonoBehaviour
{
    #region Variables

    [SerializeField] public float currentHealPoints;
    [SerializeField] public float maxHealPoints;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string shootTriggerName;
    [SerializeField] protected string dieTriggerName;
    [SerializeField] protected string isDeadBoolName;
    [SerializeField] protected string liveTriggerName;

    [SerializeField] protected Transform unitTransform;
    [SerializeField] private UiManager uiManager;

    protected bool isDead;

    #endregion


    #region Events

    public event Action<float,float> OnChanged;
    public event Action OnDied;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        currentHealPoints=maxHealPoints;
    }

    #endregion

    #region Public methods

    public virtual void ApplyDamage(float damage)
    {
        currentHealPoints -= damage;

        
        OnChanged?.Invoke(currentHealPoints,maxHealPoints);

        if (currentHealPoints <= 0)
        {
            UnitDie();

            OnDied?.Invoke();
        }
    }

    #endregion

    #region Private methods

    protected void InitiateDamage(Collider2D collision)
    {
        var damageInitiator = collision.GetComponent<DamageInitiator>();

        if (damageInitiator != null)
        {
            ApplyDamage(damageInitiator.Damage);
        }
    }

    protected virtual void UnitDie()
    {
        animator.SetTrigger(dieTriggerName);
        animator.SetBool(isDeadBoolName, true);
        isDead = true;
        
        GetComponent<CircleCollider2D>().enabled = false;
    }

    protected virtual void UnitLive()
    {
        animator.SetTrigger(liveTriggerName);
        animator.SetBool(isDeadBoolName, false);
        isDead = false;

        GetComponent<CircleCollider2D>().enabled = true;

        ReloadHealPoints();
    }
    
    protected void ReloadHealPoints()
    {
        currentHealPoints = maxHealPoints;
    }

    protected void PlayShootingAnimation()
    {
        animator.SetTrigger(shootTriggerName);
    }

    #endregion

    #region Event handlers

    private void UImanager_OnReloadButton()
    {
        
    }

    #endregion

}
