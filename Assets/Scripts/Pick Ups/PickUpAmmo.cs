using UnityEngine;
using Lean.Pool;

public class PickUpAmmo : MonoBehaviour
{
    #region Variables

    [SerializeField] private float additionalAmmo;

    #endregion


    #region Unity lifecycle

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            if (player.CurrentAmmo < player.MaxAmmo)
            {
                player.GiveAdditionalAmmo(additionalAmmo);

                LeanPool.Despawn(gameObject);
            }
        }
    }

    #endregion
}
