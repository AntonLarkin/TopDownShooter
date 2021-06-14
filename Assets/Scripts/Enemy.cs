using System.Collections;
using UnityEngine;

public class Enemy : BasePerson
{
    #region Variables

    private Transform player;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        player = FindObjectOfType<Player>().transform;
    }

    private void Start()
    {
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
        var playerPosition = player.position;
        transform.up = -player.transform.position + transform.position;
    }

    #endregion


    #region Coroutines

    private IEnumerator OnTimeOutShoot()
    {
        while (!isDead)
        {
            CreateBullet();
            PlayShootingAnimation();

            yield return new WaitForSeconds(2f);
        }
    }

    #endregion

}
