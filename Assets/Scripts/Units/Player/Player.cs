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
        GameOverView.OnRestartButton += GameOverView_OnRestartButton;
    }

    private void OnDisable()
    {
        GameOverView.OnRestartButton -= GameOverView_OnRestartButton;
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

    protected override void UnitDie()
    {
        base.UnitDie();

        GetComponent<PlayerMovement>().enabled = false;
    }

    #endregion


    #region Event handlers

    private void GameOverView_OnRestartButton()
    {
        transform.position = startPosition;
        base.UnitLive();
        GetComponent<PlayerMovement>().enabled = true;
    }

    #endregion
}



