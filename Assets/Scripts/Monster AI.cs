using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public MonsterStats stats;
    public Transform[] patrolPoints;
    public Transform player;

    [Header("Audio")]
    public AudioClip detectSound;

    public enum MonsterState { Patrol, Chase, Attack, Lost }
    public MonsterState currentState = MonsterState.Patrol;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private int currentPatrolIndex = 0;
    private float lostTimer = 0f;
    private float attackTimer = 0f;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent.speed = stats.data.patrolSpeed;

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        animator.SetFloat("Speed", agent.velocity.magnitude, 0.1f, Time.deltaTime);

        switch (currentState)
        {
            case MonsterState.Patrol:
                HandlePatrol();
                break;
            case MonsterState.Chase:
                HandleChase();
                break;
            case MonsterState.Attack:
                HandleAttack();
                break;
            case MonsterState.Lost:
                HandleLost();
                break;
        }
    }

    void HandlePatrol()
    {
        agent.speed = stats.data.patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f && agent.hasPath)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = Random.Range(1f, 3f);
            }
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                GoToNextPatrolPoint();
            }
        }

        if (CanSeePlayer())
            ChangeState(MonsterState.Chase);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, patrolPoints.Length);
        } while (newIndex == currentPatrolIndex && patrolPoints.Length > 1);

        currentPatrolIndex = newIndex;
        agent.destination = patrolPoints[currentPatrolIndex].position;
    }

    void HandleChase()
    {
        agent.speed = stats.data.chaseSpeed;
        agent.destination = player.position;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= stats.data.attackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        if (!CanSeePlayer())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= stats.data.lostPlayerTime)
                ChangeState(MonsterState.Lost);
        }
        else
        {
            lostTimer = 0f;
        }
    }

    void HandleAttack()
    {
        agent.destination = transform.position;
        transform.LookAt(player);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > stats.data.attackRange + 0.5f)
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = 1.5f;
        }
    }

    void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(stats.data.attackDamage);
    }

    void HandleLost()
    {
        agent.speed = stats.data.patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChangeState(MonsterState.Patrol);
            GoToNextPatrolPoint();
        }
    }

    public void ChangeState(MonsterState newState)
    {
        if (currentState == newState) return;

        lostTimer = 0f;
        currentState = newState;

        switch (newState)
        {
            case MonsterState.Patrol:
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsLost", false);
                break;

            case MonsterState.Chase:
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsLost", false);

                if (audioSource != null && detectSound != null)
                    audioSource.PlayOneShot(detectSound);
                break;

            case MonsterState.Attack:
                animator.SetBool("IsAttacking", true);
                agent.destination = player.position;
                break;

            case MonsterState.Lost:
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsLost", true);
                agent.destination = player.position;
                break;
        }
    }

    bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        float range = stats.data.detectionRange;
        if (FlashlightToggle.IsFlashlightOn)
            range += stats.data.flashlightBonus;

        if (dist > range) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out hit, range))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }
}