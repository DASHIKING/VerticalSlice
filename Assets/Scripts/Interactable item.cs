using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Item Properties")]
    public float weight = 1f;    // 重量，影响移动速度
    public float value = 100f;   // 价值，显示在UI上
    public string itemName = "Item"; // 物品名字

    [HideInInspector]
    public bool isBeingHeld = false; // 是否正在被持有，其他脚本会用到
}