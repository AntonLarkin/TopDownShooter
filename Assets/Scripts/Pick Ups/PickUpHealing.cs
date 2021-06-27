using UnityEngine;

public class PickUpHealing : MonoBehaviour
{
    #region Variables

    [SerializeField] private float healPoints;

    #endregion


    #region Unity lifecycle

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();

        if(player != null)
        {
            if (player.currentHealPoints < player.maxHealPoints)
            {
                player.ApplyDamage(-healPoints);
                if (player.currentHealPoints + healPoints >= player.maxHealPoints)
                {
                    player.currentHealPoints = player.maxHealPoints;
                }
                Destroy(gameObject);
            }
        }
    }

    #endregion
}
