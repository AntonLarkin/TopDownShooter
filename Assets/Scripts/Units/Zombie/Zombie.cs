using UnityEngine;
using Pathfinding;
using Lean.Pool;

public class Zombie : BaseUnit
{
    #region Variables

    private enum State
    {
        Idle,
        Attack,
        Chase,
        Return,
        Die,
        Patrol
    }

    [SerializeField] private float moveRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float chaseRadius;
    [SerializeField] private float noticeRadius;
    [SerializeField] private float zombieDamage;
    [SerializeField] private float attackRate;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private bool isPatrolActive;

    [SerializeField] private GameObject pickUpPrefab;
    [Range(1, 100)]
    [SerializeField] private int pickUpCreationRate;

    [SerializeField] private LayerMask obstacleMask;

    private Player player;
    private Transform playerTransform;
    //private ZombieMovement zombieMovement;
    private AIPath aIPath;
    private AIDestinationSetter aIDestinationSetter;

    private Transform cachedTransform;

    private Transform startPositionTransform;
    private Transform patrolPosition;
    private int currentPatrolPoint;

    [Range(30, 360)]
    [SerializeField] private float visionAngle;

    private float minDistance;

    private float lastAttackTime;

    private State currentState;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        cachedTransform = transform;
        startPositionTransform = cachedTransform;

        player = FindObjectOfType<Player>();
        playerTransform = player.transform;

        aIPath = GetComponent<AIPath>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();

        minDistance = aIPath.endReachedDistance;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            isPatrolActive = true;
            currentPatrolPoint = 0;
        }
        else
        {
            isPatrolActive = false;
            currentPatrolPoint = -1;
        }

        SetState(isPatrolActive ? State.Patrol : State.Idle);

    }

    private void OnEnable()
    {
        player.OnDied += Player_OnDied;
    }

    private void OnDisable()
    {
        player.OnDied -= Player_OnDied;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageInitiator = collision.GetComponent<DamageInitiator>();

        if (damageInitiator != null)
        {
            ApplyDamage(damageInitiator.Damage);
        }
    }

    private void Update()
    {
        if (currentState == State.Die)
        {
            return;
        }

        CheckState();
        UpdateCurrentState();
        
    }

    #endregion


    #region Private methods

    private void OnDrawGizmos()
    {
        var direction = -unitTransform.up;
        var rotate = Quaternion.AngleAxis(visionAngle / 2, Vector3.forward);
        var rotate2 = Quaternion.AngleAxis(-visionAngle / 2, Vector3.forward);

        var leftRay = rotate * direction;
        var rightRay = rotate2 * direction;
        Gizmos.DrawRay(unitTransform.position, leftRay * moveRadius);
        Gizmos.DrawRay(unitTransform.position, rightRay * moveRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, moveRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, noticeRadius);
    }

    private void CheckState()
    {
        var playerPos = playerTransform.position;
        var distance = Vector3.Distance(playerPos, cachedTransform.position);

        if (distance < attackRadius && IsPlayerFound() && !player.IsDead)
        {
            SetState(State.Attack);
        }
        else if ((distance < moveRadius || distance < noticeRadius) && distance > attackRadius && !player.IsDead && (IsPlayerFound() || distance < noticeRadius))
        {
            //var direction = playerPos - transform.position;
            //Debug.DrawRay(transform.position, direction, Color.red);

            //var ray = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, obstacleMask);

            //if (ray.collider !=null)
            //{
            //    if (patrolPoints.Length > 0)
            //    {
            //        SetState(State.Patrol);
            //        return;
            //    }
            //    else
            //    {
            //        SetState(State.Return);
            //        return;
            //    }
            //}

            SetState(State.Chase);

        }
        else if (distance > chaseRadius)
        {
            if (currentState == State.Chase)
            {
                SetState(State.Return);
            }
        }

        if (currentState == State.Return && Vector3.Distance(transform.position, startPositionTransform.position) <= minDistance)
        {
            SetState(isPatrolActive ? State.Patrol : State.Idle);
        }

    }

    private void SetState(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                {
                    SetActiveMovement(false);
                    break;
                }

            case State.Attack:
                {
                    SetActiveMovement(true);
                    break;
                }
            case State.Chase:
                {
                    SetActiveMovement(true);
                    SetTarget(playerTransform);

                    break;
                }
            case State.Return:
                {
                    if (currentState == State.Chase || currentState == State.Attack)
                    {
                        SetActiveMovement(true);
                        SetTarget(startPositionTransform);
                    }
                    break;
                }
            case State.Patrol:
                {
                    SetActiveMovement(true);
                    patrolPosition = patrolPoints[currentPatrolPoint];
                    SetTarget(patrolPosition);

                    break;
                }
        }

        currentState = newState;
    }

    private void SetActiveMovement(bool isActive)
    {
        aIPath.enabled = isActive;

        animator.SetFloat(moveSpeedName, isActive ? aIPath.velocity.magnitude : 0);
    }

    private void SetTarget(Transform target)
    {
        aIDestinationSetter.target = target;
    }

    private void UpdateCurrentState()
    {
        if (currentState == State.Attack)
        {
            Attack();
        }
        else if (currentState == State.Chase)
        {
            Chase();
            Move();
        }
        else if (currentState == State.Return)
        {
            Return();
            Move();
        }
        else if (currentState == State.Patrol)
        {
            Patrol();
            Move();
            patrolPosition = patrolPoints[currentPatrolPoint];
            SetTarget(patrolPosition);
        }
    }

    protected override void UnitDie()
    {
        base.UnitDie();

        SetState(State.Die);
        SetActiveMovement(false);

        if (NeedCreatePickUp())
        {
            LeanPool.Spawn(pickUpPrefab, transform.position, Quaternion.identity);
        }
    }

    private void Attack()
    {
        aIPath.enabled = false;

        if (Time.time - lastAttackTime > attackRate)
        {
            player.ApplyDamage(zombieDamage);

            animator.SetTrigger(shootTriggerName);
            lastAttackTime = Time.time;

        }
    }

    private void Chase()
    {
        SetTarget(playerTransform);
    }

    private void Return()
    {
        if (Vector3.Distance(cachedTransform.position, startPositionTransform.position) <= minDistance)
        {
            SetState(State.Idle);
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(cachedTransform.position, patrolPosition.position) <= minDistance)
        {
            SetNewPatrolPoint();
            SetTarget(patrolPosition);
            Move();
        }
    }

    private void Move()
    {
        animator.SetFloat(moveSpeedName, aIPath.velocity.magnitude);

    }

    private void SetNewPatrolPoint()
    {
        currentPatrolPoint++;

        if (currentPatrolPoint >= patrolPoints.Length)
        {
            currentPatrolPoint = 0;
        }

        patrolPosition = patrolPoints[currentPatrolPoint];
    }

    private bool NeedCreatePickUp()
    {
        var randomNumber = Random.Range(1, 101);
        return pickUpCreationRate >= randomNumber;
    }

    private bool IsPlayerFound()
    {
        var directionToPlayer = player.transform.position - cachedTransform.position;
        var halfAngleVision = Vector3.Angle(directionToPlayer, -unitTransform.up);

        if(halfAngleVision > visionAngle / 2)
        {
            return false;
        }

        RaycastHit2D rHit = Physics2D.Raycast(cachedTransform.position, directionToPlayer, moveRadius,obstacleMask);
        if (rHit.collider != null)
        {
            return false;
        }

        Debug.DrawRay(transform.position, directionToPlayer * moveRadius, Color.magenta);
        return true;
    }

    #endregion


    #region Event handlers

    private void Player_OnDied()
    {
        if (currentState == State.Die)
        {
            UnitLive();
        }

        ReloadHealPoints();

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            isPatrolActive = true;
            currentPatrolPoint = 0;

        }
        else
        {
            isPatrolActive = false;
            currentPatrolPoint = -1;
            SetTarget(startPositionTransform);
        }

        SetState(isPatrolActive ? State.Patrol : State.Return);


    }

    #endregion
}
