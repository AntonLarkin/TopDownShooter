using System.Collections;
using UnityEngine;

public class Enemy : LongRangeUnit
{
    #region Variables

    [SerializeField] private float timeDelay;

    private Transform playerTransform;
    private WaitForSeconds shootDelay;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        shootDelay = new WaitForSeconds(timeDelay);
    }

    private void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;

        StartCoroutine(OnTimeOutShoot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        InitiateDamage(collision);
    }

    private void Update()
    {
        if (!isDead)
        {
            LookAtPlayer();
        }
    }

    #endregion


    #region Private methods

    private void LookAtPlayer()
    {
        transform.up = -playerTransform.transform.position + transform.position;
    }

    private IEnumerator OnTimeOutShoot()
    {
        while (!isDead)
        {
            yield return shootDelay;

            CreateBullet();
            PlayShootingAnimation();

        }
    }

    #endregion
}
