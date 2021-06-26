using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{

    #region Variables

    [SerializeField] private float damage;
    [SerializeField] private float damageArea;
    [SerializeField] protected Animator animator;
    [SerializeField] private string explodeTriggerName;

    #endregion


    #region Unity lifecycle

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageInitiator = collision.GetComponent<DamageInitiator>();

        if (damageInitiator != null)
        {
            Explode();
            animator.SetTrigger(explodeTriggerName);

            StartCoroutine(OnAnimationEndDestroy());
        }
        
    }

    #endregion


    #region Private methods

    private IEnumerator OnAnimationEndDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, damageArea);
    }

    private void Explode()
    {
        var unitsInRadius = Physics2D.OverlapCircleAll(transform.position, damageArea);

        foreach(Collider2D baseUnit in unitsInRadius)
        {
            if (baseUnit.GetComponent<BaseUnit>())
            {
                var unit = baseUnit.GetComponent<BaseUnit>();
                unit.GetDamage(damage);
            }
        }
    }

    #endregion

}
