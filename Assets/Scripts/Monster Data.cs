using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Basic Stats")]
    public string monsterName = "Monster";
    public float maxHP = 100f;
    public float attackDamage = 10f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float flashlightBonus = 8f;
    public float attackRange = 1.8f;

    [Header("State Timers")]
    public float lostPlayerTime = 5f;
}