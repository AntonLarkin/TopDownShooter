using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeUnit : BaseUnit
{
    #region Variables

    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;

    #endregion


    #region Private methods

    protected void CreateBullet()
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, unitTransform.rotation);
    }

    #endregion
}
