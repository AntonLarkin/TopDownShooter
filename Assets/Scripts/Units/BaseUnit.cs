using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    #region Variables

    [SerializeField] protected float currentHealPoints;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string shootTriggerName;
    [SerializeField] protected string dieTriggerName;
    [SerializeField] protected string isDeadBoolName;


    protected bool isDead;

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

    protected virtual void ApplyDamage(float damage)
    {
        currentHealPoints -= damage;

        if (currentHealPoints <= 0)
        {
            UnitDie();
        }
    }

    protected virtual void UnitDie()
    {
        animator.SetTrigger(dieTriggerName);
        animator.SetBool(isDeadBoolName, true);
        isDead = true;

        GetComponent<CircleCollider2D>().enabled = false;
    }

    protected void PlayShootingAnimation()
    {
        animator.SetTrigger(shootTriggerName);
    }

    #endregion


    #region Public methods

    public void GetDamage(float damage)
    {
        ApplyDamage(damage);
    }

    #endregion

}
