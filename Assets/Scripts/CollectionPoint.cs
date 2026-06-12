using UnityEngine;
using TMPro;

public class CollectionPoint : MonoBehaviour
{
    [Header("Settings")]
    public float requiredValue = 500f;
    public bool isCompleted = false;

    [Header("UI")]
    public TextMeshProUGUI requiredValueText;
    public TextMeshProUGUI statusText;

    [Header("Visual")]
    public Renderer floorRenderer;

    [Header("Pillar")]
    
    public GameObject glowPillar;  // 把绿色柱子拖进来

    private HandcartController cartInside = null;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        
        if (isCompleted) return;

        if (cartInside != null)
        {
            float cartValue = cartInside.GetTotalValue();
            float difference = cartValue - requiredValue;

            if (difference >= 0)
            {
                statusText.text = "Right click to submit!";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "Need $" +
                    Mathf.Abs(difference).ToString("0") + " more";
                statusText.color = Color.red;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("右键被按下, cartInside: " + cartInside);
                TrySubmit();
            }
        }
        else
        {
            statusText.text = "Push cart here";
            statusText.color = Color.yellow;
        }
    }

    void TrySubmit()
    {
        Debug.Log("TrySubmit 被调用, cartInside: " + cartInside + 
            ", cartValue: " + cartInside?.GetTotalValue() + 
            ", required: " + requiredValue);
        
        if (cartInside == null) return;

        float cartValue = cartInside.GetTotalValue();
        if (cartValue >= requiredValue)
        {
            cartInside.SubmitItems(requiredValue);
            Complete();
        }
        else
        {
            statusText.text = "Not enough value!";
            statusText.color = Color.red;
        }
    }

    void Complete()
    {
        isCompleted = true;
        statusText.text = "Completed!";
        statusText.color = Color.green;

        if (floorRenderer != null)
            floorRenderer.material.color = Color.green;

        // 隐藏绿色柱子
        if (glowPillar != null)
            glowPillar.SetActive(false);

        GameManager.Instance?.OnCollectionPointCompleted();
    }

    void UpdateUI()
    {
        if (requiredValueText != null)
            requiredValueText.text = "Required: $" +
                requiredValue.ToString("0");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发器检测到: " + other.gameObject.name + " Tag: " + other.tag);
    
        if (other.CompareTag("Handcart"))
        {
            cartInside = other.GetComponent<HandcartController>();
            if (cartInside == null)
                cartInside = other.GetComponentInParent<HandcartController>();
            Debug.Log("手推车进入收集点!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Handcart"))
        {
            cartInside = null;
            statusText.text = "Push cart here";
            statusText.color = Color.yellow;
        }
    }
}