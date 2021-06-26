using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moveSpeedName;

    private Transform cachedTransform;
    private Rigidbody2D rb;
    private Vector3 direction;

    private Vector3 destinationPoint;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;     //если зомби становится больше 3 ОДНОГО типа, то выдает ошибку (?)
        SetMoveAnimation(0f);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RotateZombie();
        MoveZombie();
    }

    #endregion


    #region Public methods

    public void SetDestination(Vector3 position)
    {
        destinationPoint = position;
    }

    #endregion


    #region Private methods

    private void MoveZombie()
    {
        rb.velocity = direction * speed;
        SetMoveAnimation(direction.magnitude);
    }

    private void SetMoveAnimation(float magnitude)
    {
        animator.SetFloat(moveSpeedName, magnitude);
    }

    private void RotateZombie()
    {
        direction = Vector3.ClampMagnitude((destinationPoint - cachedTransform.position),1f);

        cachedTransform.up = -(Vector2)direction;
    }

    #endregion
}
