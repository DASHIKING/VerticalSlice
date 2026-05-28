using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity = 100f;
    public float darkDecreaseRate = 1f;
    public float chaseDecreaseRate = 2f;
    public float lightRecoveryRate = 1f;

    [Header("UI")]
    public Slider sanitySlider;
    public TextMeshProUGUI sanityText;

    [Header("Distortion")]
    public Material sanityDistortionMat;
    public float maxDistortionIntensity = 0.02f;
    public float distortionThreshold = 20f;

    private PlayerHealth playerHealth;
    private bool isDead = false;

    void Start()
    {
        currentSanity = maxSanity;
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();

        if (sanityDistortionMat != null)
            sanityDistortionMat.SetFloat("_Intensity", 0f);
    }

    void Update()
    {
        if (isDead) return;
        UpdateSanity();
        UpdateDistortion();
        UpdateUI();
    }

    void UpdateSanity()
    {
        bool flashlightOn = FlashlightToggle.IsFlashlightOn;
        bool isBeingChased = IsBeingChased();

        if (isBeingChased)
            currentSanity -= chaseDecreaseRate * Time.deltaTime;
        else if (flashlightOn)
            currentSanity += lightRecoveryRate * Time.deltaTime;
        else
            currentSanity -= darkDecreaseRate * Time.deltaTime;

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        if (currentSanity <= 0f)
        {
            isDead = true;
            if (playerHealth != null)
                playerHealth.TakeDamage(9999f);
        }
    }

    void UpdateDistortion()
    {
        if (sanityDistortionMat == null) return;

        if (currentSanity <= distortionThreshold)
        {
            float t = 1f - (currentSanity / distortionThreshold);
            float intensity = Mathf.Lerp(0f, maxDistortionIntensity, t);
            sanityDistortionMat.SetFloat("_Intensity", intensity);
        }
        else
        {
            sanityDistortionMat.SetFloat("_Intensity", 0f);
        }
    }

    bool IsBeingChased()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
        foreach (MonsterAI monster in monsters)
        {
            if (monster.currentState == MonsterAI.MonsterState.Chase ||
                monster.currentState == MonsterAI.MonsterState.Attack)
                return true;
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