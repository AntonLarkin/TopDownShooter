using System.Collections;
using UnityEngine;

public class Player : BasePerson
{
    #region Variables

    [SerializeField] private float shootDelay;

    private PickUpHealing pickUp;
    private float currentShootDelay;

    #endregion


    #region Properties

    public bool IsDead => isDead;

    #endregion


    #region Unity lifecycle

    private void Start()
    {
        pickUp = FindObjectOfType<PickUpHealing>();
    }

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
            PersonDie();

            StartCoroutine(OnGameOverReload());
        }
    }

    protected override void PersonDie()
    {
        base.PersonDie();

        StartCoroutine(OnGameOverReload());
    }

    #endregion


    #region Coroutines

    private IEnumerator OnGameOverReload()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.ReloadScene();
    }

    #endregion
}



