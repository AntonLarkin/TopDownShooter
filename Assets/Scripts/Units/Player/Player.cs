using System.Collections;
using UnityEngine;

public class Player : LongRangeUnit
{
    #region Variables

    [SerializeField] private float shootDelay;

    private float currentShootDelay;

    private Vector3 startPosition;

    #endregion


    #region Properties

    public bool IsDead => isDead;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        OnDied += OnDied_ReloadPlayer;
    }

    private void OnDisable()
    {
        OnDied -= OnDied_ReloadPlayer;
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


    #region Public methods

    //public override void ApplyDamage(float damage)
    //{
    //    base.ApplyDamage(damage);

    //    if (currentHealPoints <= 0)
    //    {
    //        UnitDie();

    //        //StartCoroutine(OnGameOverReload());
    //    }
    //}

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

    protected override void UnitDie()
    {
        base.UnitDie();

        GetComponent<PlayerMovement>().enabled = false;

        //StartCoroutine(OnGameOverReload());
    }

    //private IEnumerator OnGameOverReload()
    //{
    //    yield return new WaitForSeconds(1f);
    //    SceneLoader.ReloadScene();
    //}

    #endregion


    #region Event handlers

    private void OnDied_ReloadPlayer()
    {
        transform.position = startPosition;
        GetComponent<PlayerMovement>().enabled = true;
        UnitLive();

        var enemies = FindObjectsOfType<BaseUnit>();    //лучше не придумал
        foreach(BaseUnit enemy in enemies)
        {
            if (enemy.GetComponent<Zombie>())
            {
                enemy.GetComponent<Zombie>().Reload();
            }
        }
    }

    #endregion
}



