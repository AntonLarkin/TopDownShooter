using UnityEngine;

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
    [SerializeField] private float zombieDamage;
    [SerializeField] private float attackRate;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private bool isPatrolActive;

    private Player player;
    private Transform playerTransform;
    private ZombieMovement zombieMovement;

    private Transform cachedTransform;
    private Vector3 startPosition;
    private Vector3 patrolPosition;
    private int currentPatrolPoint;

    private float minDistance = 0.1f;
    private bool isZombieDead;

    private float lastAttackTime;

    private State currentState;

    #endregion


    #region Unity lifecycle

    private void Awake()
    {
        cachedTransform = transform;
        startPosition = cachedTransform.position;

        zombieMovement = GetComponent<ZombieMovement>();

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

    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerTransform = player.transform;
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
        if (player.IsDead || currentState == State.Die)
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, moveRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    private void CheckState()
    {
        var playerPos = playerTransform.position;
        var distance = Vector3.Distance(playerPos, cachedTransform.position);

        if (!isZombieDead)
        {
            if (distance < attackRadius)
            {
                SetState(State.Attack);
            }
            else if (distance < moveRadius)
            {
                SetState(State.Chase);
            }
            else if (distance > chaseRadius)
            {
                if (currentState == State.Chase)
                {
                    SetState(State.Return);
                }
            }
            else if (currentHealPoints <= 0)
            {
                SetState(State.Die);
            }

            if (currentState == State.Return && Vector3.Distance(transform.position, startPosition) <= minDistance)
            {
                SetState(isPatrolActive ? State.Patrol : State.Idle);
            }
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
                    zombieMovement.SetDestination(playerTransform.position);

                    break;
                }
            case State.Return:
                {
                    if (currentState == State.Chase)
                    {
                        SetActiveMovement(true);
                        zombieMovement.SetDestination(startPosition);
                    }
                    break;
                }
            case State.Patrol:
                {
                    SetActiveMovement(true);
                    patrolPosition = patrolPoints[currentPatrolPoint].position;
                    zombieMovement.SetDestination(patrolPosition);
                    break;
                }
        }

        currentState = newState;
    }

    private void SetActiveMovement(bool isActive)
    {
        zombieMovement.enabled = isActive;
    }

    private void UpdateCurrentState()
    {
        if (currentState == State.Attack)
        {
            Attack();
        }
        if(currentState == State.Chase)
        {
            Chase();
        }
        if(currentState == State.Return)
        {
            Return();
        }
        if(currentState == State.Die)
        {
            Die();
        }
        if(currentState == State.Patrol)
        {
            Patrol();
            patrolPosition = patrolPoints[currentPatrolPoint].position;
            zombieMovement.SetDestination(patrolPosition);
        }
    }

    protected override void UnitDie()
    {
        base.UnitDie();

        isZombieDead = true;
        SetActiveMovement(false);
        GetComponent<CircleCollider2D>().enabled = false;
    }

    private void Die()
    {
        UnitDie();
    }

    private void Attack()
    {
        zombieMovement.enabled = false;

        if (Time.time - lastAttackTime > attackRate)
        {
            player.TakeDamage(zombieDamage);
            animator.SetTrigger(shootTriggerName);
            lastAttackTime = Time.time;
        }
    }

    private void Chase()
    {
        zombieMovement.SetDestination(playerTransform.position);
    }

    private void Return()
    {
        if (Vector3.Distance(cachedTransform.position, startPosition) <= minDistance)
        {
            SetState(State.Idle);
        }
    }

    private void Patrol()
    {
        if (Vector3.Distance(cachedTransform.position, patrolPosition) <= minDistance)
        {
            SetNewPatrolPoint();
            zombieMovement.SetDestination(patrolPosition);
        }
    }

    private void SetNewPatrolPoint()
    {
        currentPatrolPoint++;

        if (currentPatrolPoint >= patrolPoints.Length)
        {
            currentPatrolPoint = 0;
        }

        patrolPosition = patrolPoints[currentPatrolPoint].position;
    }

    #endregion
}
