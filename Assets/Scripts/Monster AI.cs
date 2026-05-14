using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public MonsterStats stats;
    public Transform[] patrolPoints;      // 把巡逻点拖进来
    public Transform player;             // 把Player拖进来

    // 状态枚举
    public enum MonsterState { Patrol, Chase, Attack, Lost }
    public MonsterState currentState = MonsterState.Patrol;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;  // 当前目标巡逻点
    private float lostTimer = 0f;        // 丢失玩家计时器
    private float attackTimer = 0f;      // 攻击冷却计时器

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.data.patrolSpeed;

        // 自动找到Player（也可以手动拖）
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        // 开始巡逻
        GoToNextPatrolPoint();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

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

    // ── PATROL ──────────────────────────────────────────
    void HandlePatrol()
    {
        agent.speed = stats.data.patrolSpeed;

        // 到达巡逻点后前往下一个
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();

        // 检测玩家
        if (CanSeePlayer())
            ChangeState(MonsterState.Chase);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // ── CHASE ───────────────────────────────────────────
    void HandleChase()
    {
        agent.speed = stats.data.chaseSpeed;
        agent.destination = player.position;

        // 够近就攻击
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= stats.data.attackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        // 看不到玩家就开始计时
        if (!CanSeePlayer())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= stats.data.lostPlayerTime)
                ChangeState(MonsterState.Lost);
        }
        else
        {
            lostTimer = 0f; // 看得到就重置计时
        }
    }

    // ── ATTACK ──────────────────────────────────────────
    void HandleAttack()
    {
        agent.destination = transform.position; // 停下来
        transform.LookAt(player);

        float dist = Vector3.Distance(transform.position, player.position);

        // 玩家跑远了就继续追
        if (dist > stats.data.attackRange + 0.5f)
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        // 攻击冷却结束就攻击
        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = 1.5f; // 1.5秒攻击一次
        }
    }

    void Attack()
    {
        // 找到玩家的血量脚本并扣血
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(stats.data.attackDamage);
    }

    // ── LOST ────────────────────────────────────────────
    void HandleLost()
    {
        agent.speed = stats.data.patrolSpeed;

        // 在玩家最后位置附近转一转
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // 搜索完毕，回到巡逻
            ChangeState(MonsterState.Patrol);
            GoToNextPatrolPoint();
        }
    }

    // ── 工具方法 ─────────────────────────────────────────
    void ChangeState(MonsterState newState)
    {
        lostTimer = 0f;
        currentState = newState;

        if (newState == MonsterState.Lost)
        {
            // 去玩家最后出现的位置搜索
            agent.destination = player.position;
        }
    }

    

    // 检测玩家是否可见
    bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // 计算实际检测范围（手电筒开着时更容易被发现）
        float range = stats.data.detectionRange;
        if (FlashlightToggle.IsFlashlightOn)
            range += stats.data.flashlightBonus;

        if (dist > range) return false;

        // Raycast 检测视线是否被墙挡住
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