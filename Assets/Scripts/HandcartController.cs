using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HandcartController : MonoBehaviour
{
    [Header("Items")]
    public List<InteractableItem> itemsInCart = new List<InteractableItem>();

    [Header("UI")]
    public TextMeshProUGUI cartValueText;

    private float totalValue = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        UpdateCartValue();
        UpdateUI();
    }

    void UpdateCartValue()
    {
        totalValue = 0f;
        List<InteractableItem> toRemove = new List<InteractableItem>();

        foreach (InteractableItem item in itemsInCart)
        {
            if (item == null)
                toRemove.Add(item);
            else
                totalValue += item.value;
        }

        foreach (InteractableItem item in toRemove)
            itemsInCart.Remove(item);
    }

    public void AddItem(InteractableItem item)
    {
        if (!itemsInCart.Contains(item))
        {
            itemsInCart.Add(item);

            // 缩小到原来的 1/3
            item.transform.localScale *= 0.33f;

            // 设为子对象跟着车走
            item.transform.SetParent(transform);

            // 根据已有物品数量错开位置
            int count = itemsInCart.Count - 1;
            float offsetX = (count % 3 - 1) * 0.3f;
            float offsetZ = (count / 3) * 0.3f;
            item.transform.localPosition = new Vector3(offsetX, 1f, offsetZ);
            item.transform.localRotation = Quaternion.identity;

            // 禁用物理防止掉落
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                itemRb.isKinematic = true;
                itemRb.useGravity = false;
                itemRb.velocity = Vector3.zero;
                itemRb.angularVelocity = Vector3.zero;
            }

            // 禁用 Outline
            Outline outline = item.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }

    public float GetTotalValue()
    {
        return totalValue;
    }

    public void SubmitItems(float requiredValue)
    {
        float remaining = requiredValue;
        List<InteractableItem> toRemove = new List<InteractableItem>();

        foreach (InteractableItem item in itemsInCart)
        {
            if (remaining <= 0) break;
            remaining -= item.value;
            toRemove.Add(item);
        }

        foreach (InteractableItem item in toRemove)
        {
            itemsInCart.Remove(item);
            Destroy(item.gameObject);
        }

        UpdateCartValue();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (cartValueText != null)
            cartValueText.text = "Cart: $" + totalValue.ToString("0");
    }
}