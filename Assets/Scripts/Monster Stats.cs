using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [Header("Monster Data")]
    public MonsterData data;

    [HideInInspector]
    public float currentHP;

    private MonsterAI ai;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHP = data.maxHP;
        ai = GetComponent<MonsterAI>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 播放死亡动画
        if (animator != null)
            animator.SetTrigger("Die");

        // 关闭AI和NavMesh
        if (ai != null)
            ai.enabled = false;

        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // 3秒后消失，2分钟后复活
        StartCoroutine(DeathRoutine());
    }

    System.Collections.IEnumerator DeathRoutine()
    {
        // 等3秒后隐藏
        yield return new WaitForSeconds(3f);
        HideMonster();

        // 再等117秒（共2分钟）后复活
        yield return new WaitForSeconds(117f);
        Respawn();
    }

    void HideMonster()
    {
        // 隐藏所有视觉部件但保留GameObject
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }

    void Respawn()
    {
        isDead = false;
        currentHP = data.maxHP;

        // 传送到随机巡逻点
        if (ai != null && ai.patrolPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, ai.patrolPoints.Length);
            transform.position = ai.patrolPoints[randomIndex].position;
        }

        // 重新显示
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        // 重新启用AI
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        if (ai != null)
        {
            ai.enabled = true;
            ai.ChangeState(MonsterAI.MonsterState.Patrol);
        }
    }
}