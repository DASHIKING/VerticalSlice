using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();

        if (currentHP <= 0)
            Die();
    }

    void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + currentHP.ToString("0") + " / " + maxHP.ToString("0");
    }

    void Die()
    {
        // 调用 Game Over 画面
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
    }
}