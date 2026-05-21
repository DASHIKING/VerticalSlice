using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity = 100f;
    public float darkDecreaseRate = 1f;      // 黑暗中每秒下降
    public float chaseDecreaseRate = 2f;     // 被追逐时每秒下降
    public float lightRecoveryRate = 1f;     // 开灯时每秒上升

    [Header("UI")]
    public Slider sanitySlider;              // 进度条
    public TextMeshProUGUI sanityText;       // 数字显示

    private PlayerHealth playerHealth;
    private bool isDead = false;

    void Start()
    {
        currentSanity = maxSanity;
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();
    }

    void Update()
    {
        if (isDead) return;

        UpdateSanity();
        UpdateUI();
    }
    void UpdateSanity()
    {
        bool flashlightOn = FlashlightToggle.IsFlashlightOn;
        bool isBeingChased = IsBeingChased();

        if (isBeingChased)
        {
            currentSanity -= chaseDecreaseRate * Time.deltaTime;
        }
        else if (flashlightOn)
        {
            currentSanity += lightRecoveryRate * Time.deltaTime;
        }
        else
        {
            currentSanity -= darkDecreaseRate * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        if (currentSanity <= 0f)
        {
            isDead = true;
            if (playerHealth != null)
                playerHealth.TakeDamage(9999f);
        }
    }
    

    bool IsBeingChased()
    {
        // 找场景里所有怪物，检查是否有怪物在追玩家
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
        foreach (MonsterAI monster in monsters)
        {
            if (monster.currentState == MonsterAI.MonsterState.Chase ||
                monster.currentState == MonsterAI.MonsterState.Attack)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateUI()
    {
        if (sanitySlider != null)
            sanitySlider.value = currentSanity / maxSanity;

        if (sanityText != null)
            sanityText.text = "Sanity: " + currentSanity.ToString("0");
    }
}