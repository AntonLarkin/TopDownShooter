using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity=-transform.up*speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroyObject();
    }

    private void OnBecameInvisible()
    {
        DestroyObject();
    }

    #endregion


    #region Private methods

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    #endregion

}
