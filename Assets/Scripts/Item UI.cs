using UnityEngine;
using TMPro;

public class ItemUI : MonoBehaviour
{
    public static ItemUI Instance; // 单例，方便其他脚本调用

    [Header("UI References")]
    public GameObject itemInfoPanel;     // 刚才创建的 Panel
    public TextMeshProUGUI itemNameText; // ItemNameText
    public TextMeshProUGUI itemValueText;// ItemValueText

    void Awake()
    {
        Instance = this;
    }

    // 显示物品信息
    public void ShowItemInfo(string name, float value)
    {
        itemInfoPanel.SetActive(true);
        itemNameText.text = name;
        itemValueText.text = "Value: $" + value.ToString("0");
    }

    // 隐藏物品信息
    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }
}