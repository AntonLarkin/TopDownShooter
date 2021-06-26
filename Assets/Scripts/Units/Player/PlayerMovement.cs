using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moveSpeedName;

    private Rigidbody2D rb;
    private Player player;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        MovePlayer();
        RotatePlayer();

    }

    #endregion


    #region Private methods

    private void MovePlayer()
    {
        var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rb.velocity = direction * speed;
        animator.SetFloat(moveSpeedName, direction.magnitude);
    }

    private void RotatePlayer()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var directionInvert = Vector3.ClampMagnitude((mousePosition - transform.position),1f);

        transform.up = -(Vector2)directionInvert;
    }

    #endregion
}
