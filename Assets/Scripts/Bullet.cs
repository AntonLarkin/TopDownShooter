using UnityEngine;
using Lean.Pool;

public class Bullet : MonoBehaviour,IPoolable
{
    #region Variables

    [SerializeField] private float speed;

    #endregion


    #region Unity lifecycle

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
        LeanPool.Despawn(gameObject);
    }

    #endregion


    #region Public methods

    public void OnSpawn()
    {
        GetComponent<Rigidbody2D>().velocity = -transform.up * speed;
    }

    public void OnDespawn()
    {

    }

    #endregion

}
