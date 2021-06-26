using System.Collections;
using UnityEngine;

public class Player : LongRangeUnit
{
    #region Variables

    [SerializeField] private float shootDelay;

    private float currentShootDelay;

    #endregion


    #region Properties

    public bool IsDead => isDead;

    #endregion


    #region Unity lifecycle

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InitiateDamage(collision);
    }

    private void Update()
    {
        Shoot();
    }

    #endregion


    #region Private methods

    private void Shoot()
    {
        if (Input.GetButton("Fire1") && currentShootDelay <= 0 && !isDead)
        {
            currentShootDelay = shootDelay;
            CreateBullet();
            PlayShootingAnimation();
        }

        currentShootDelay -= Time.deltaTime;
    }

    protected override void ApplyDamage(float damage) 
    {
        currentHealPoints -= damage;

        if (currentHealPoints <= 0)
        {
            UnitDie();

            StartCoroutine(OnGameOverReload());
        }
    }

    protected override void UnitDie()
    {
        base.UnitDie();

        GetComponent<PlayerMovement>().enabled = false;

        StartCoroutine(OnGameOverReload());
    }

    private IEnumerator OnGameOverReload()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.ReloadScene();
    }

    #endregion


    #region Public methods

    public void Heal(float healPoints)
    {
        currentHealPoints += healPoints;
    }

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);
    }

    #endregion
}



