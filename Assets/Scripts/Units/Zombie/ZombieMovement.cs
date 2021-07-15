using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private float speed;
    [SerializeField] private Transform zombieTransform;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moveSpeedName;

    private Transform cachedTransform;
    private Rigidbody2D rb;
    private Vector3 direction;

    private AIPath aIPath;
    private AIDestinationSetter aIDestinationSetter;

    private float minDistance;

    #endregion


    #region Properties

    public float MinDistance => minDistance;

    #endregion

    #region Unity lifecycle

    private void Awake()
    {
        cachedTransform = zombieTransform;
        rb = GetComponent<Rigidbody2D>();

        aIPath = GetComponent<AIPath>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();

        minDistance = aIPath.endReachedDistance;
        aIPath.maxSpeed = speed;
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
        SetMoveAnimation(0f);
    }

    private void Update()
    {
        RotateZombie();
    }

    #endregion


    #region Public methods

    public void StartMoving(bool isActive)
    {
        aIPath.enabled = isActive;

        animator.SetFloat(moveSpeedName, isActive ? aIPath.velocity.magnitude : 0);
    }

    public void SetTargetPosition(Transform target)
    {
        aIDestinationSetter.target = target;
    }

    #endregion


    #region Private methods

    private void SetMoveAnimation(float magnitude)
    {
        animator.SetFloat(moveSpeedName, magnitude);
    }

    private void RotateZombie()
    {
        direction = Vector3.ClampMagnitude((aIDestinationSetter.target.position - cachedTransform.position), 1f);

        cachedTransform.up = -(Vector2)direction;
    }


    #endregion
}
