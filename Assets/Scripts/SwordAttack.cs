using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 0.8f;

    private float cooldownTimer = 0f;
    private Camera cam;
    private PlayerInteraction interaction;

    void Start()
    {
        cam = Camera.main;
        interaction = GetComponent<PlayerInteraction>();
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    // Visual Script 调用这个方法触发攻击
    public void TryAttack()
    {
        if (cooldownTimer > 0f) return;
        if (interaction != null && interaction.IsHoldingItem()) return;

        cooldownTimer = attackCooldown;

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            MonsterStats monster = hit.collider.GetComponent<MonsterStats>();
            if (monster != null)
                monster.TakeDamage(attackDamage);
        }
    }

    // Visual Script 调用这个判断是否显示剑
    public bool ShouldShowSword()
    {
        if (interaction == null) return true;
        return !interaction.IsHoldingItem();
    }
}