using UnityEngine;

public class MonsterStats : MonoBehaviour
{
    [Header("Monster Data")]
    public MonsterData data;  // 把对应的数据文件拖进来

    [HideInInspector]
    public float currentHP;

    void Start()
    {
        currentHP = data.maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}