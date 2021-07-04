using UnityEngine;
using Lean;

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
