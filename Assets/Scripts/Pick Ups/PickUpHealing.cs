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
            player.Heal(healPoints);
            Destroy(gameObject);
        }
    }

    #endregion
}
