using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePerson : MonoBehaviour
{
    #region Variables

    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;
    [SerializeField] protected float currentHealPoints;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string shootTriggerName;
    [SerializeField] protected string dieTriggerName;


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
            PersonDie();
        }
    }

    protected virtual void PersonDie()
    {
        animator.SetTrigger(dieTriggerName);
        animator.SetBool("IsDead", true);
        isDead = true;

        gameObject.GetComponent<CircleCollider2D>().enabled = false;

    }
    protected void CreateBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
    }

    protected void PlayShootingAnimation()
    {
        animator.SetTrigger(shootTriggerName);
    }

    #endregion

}
