using UnityEngine;
using Lean.Pool;

public class LongRangeUnit : BaseUnit
{
    #region Variables

    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletSpawnPoint;

    [SerializeField] protected float maxAmmo;

    protected float currentAmmo;

    #endregion


    #region Properties

    public float MaxAmmo => maxAmmo;
    public float CurrentAmmo => currentAmmo;

    #endregion

    #region Unity lifecycle

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    #endregion

    #region Private methods

    protected void CreateBullet()
    {
        if (currentAmmo > 0)
        {
            LeanPool.Spawn(bulletPrefab, bulletSpawnPoint.position, unitTransform.rotation);
            ShootBullet();
        }
    }

    protected void ShootBullet()
    {
        currentAmmo--;
    }

    #endregion


    #region Public methods

    public void GiveAdditionalAmmo(float ammo)
    {
        currentAmmo += ammo;
        if (currentAmmo >= maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }

    #endregion
}
