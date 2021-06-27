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

    [SerializeField] private GameObject pickUpPrefab;
    [Range(1, 100)]
    [SerializeField] private int pickUpCreationRate;

    [SerializeField] private LayerMask obstacleMask;

    private Player player;
    private Transform playerTransform;
    private ZombieMovement zombieMovement;

    private Transform cachedTransform;

    private Vector3 startPosition;
    private Vector3 patrolPosition;
    private int currentPatrolPoint;

    private float minDistance = 0.5f;

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

    //private void OnEnable()
    //{
    //    OnDied += OnDied_ReloadZombies;
    //}

    //private void OnDisable()
    //{
    //    OnDied -= OnDied_ReloadZombies;
    //}

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


    public void Reload()
    {
        transform.position = startPosition;

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

        if (distance < attackRadius)
        {
            SetState(State.Attack);
        }
        else if (distance < moveRadius)
        {
            //var direction = playerPos - transform.position;
            //Debug.DrawRay(transform.position, direction, Color.red);

            //var ray = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, obstacleMask);     ÍÅ ÐÀÁÎÒÀÅÒ!!!!

            //if (ray.collider != null)
            //{
            //    return;
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

        if (currentState == State.Return && Vector3.Distance(transform.position, startPosition) <= minDistance)
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
        else if (currentState == State.Chase)
        {
            Chase();
        }
        else if (currentState == State.Return)
        {
            Return();
        }
        else if (currentState == State.Patrol)
        {
            Patrol();
            patrolPosition = patrolPoints[currentPatrolPoint].position;
            zombieMovement.SetDestination(patrolPosition);
        }
    }

    protected override void UnitDie()
    {
        base.UnitDie();

        SetState(State.Die);
        SetActiveMovement(false);

        if (NeedCreatePickUp())
        {
            Instantiate(pickUpPrefab, transform.position, Quaternion.identity);
        }
    }

    private void Attack()
    {
        zombieMovement.enabled = false;

        if (Time.time - lastAttackTime > attackRate)
        {
            player.ApplyDamage(zombieDamage);

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

    private bool NeedCreatePickUp()
    {
        var randomNumber = Random.Range(1, 101);
        return pickUpCreationRate >= randomNumber;
    }

    #endregion


    #region Event handlers

    //private void OnDied_ReloadZombies()
    //{
    //    if (player.IsDead)
    //    {
    //        SetState(State.Patrol);
    //        UnitLive();
    //        transform.position = startPosition;
    //    }
    //}

    #endregion
}
