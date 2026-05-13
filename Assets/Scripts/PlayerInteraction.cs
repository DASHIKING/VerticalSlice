using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 4f;

    [Header("Hold Settings")]
    public float holdDistance = 2.5f;    // 物品悬浮在玩家前方多远
    public float holdSpeed = 15f;        // 物品跟随速度基础值

    private Camera cam;
    private InteractableItem currentLookedAt;
    private Outline currentOutline;

    private InteractableItem heldItem;   // 当前持有的物品
    private Rigidbody heldRb;           // 持有物品的 Rigidbody

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        DetectItem();
        HandleHolding();

        // 按下鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
                TryPickUp();
            else
                DropItem();
        }
    }

    void DetectItem()
    {
        // 已经拿着东西就不再检测高亮
        if (heldItem != null)
        {
            HideOutline();
            currentLookedAt = null;
            return;
        }

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null)
            {
                if (item != currentLookedAt)
                {
                    HideOutline();
                    currentLookedAt = item;
                    currentOutline = item.GetComponent<Outline>();
                    if (currentOutline != null)
                        currentOutline.enabled = true;
                }
                return;
            }
        }

        HideOutline();
        currentLookedAt = null;
    }

    void TryPickUp()
    {
        if (currentLookedAt == null) return;

        // 检查距离够不够近
        float dist = Vector3.Distance(transform.position, currentLookedAt.transform.position);
        if (dist > interactRange) return;

        // 拿起物品
        heldItem = currentLookedAt;
        heldRb = heldItem.GetComponent<Rigidbody>();

        // 关闭重力，让物品悬浮
        heldRb.useGravity = false;
        heldRb.drag = 10f;

        heldItem.isBeingHeld = true;
        HideOutline();

        ItemUI.Instance.ShowItemInfo(heldItem.itemName, heldItem.value);
    }

    void HandleHolding()
    {
        if (heldItem == null) return;

        // 计算目标位置：摄像机前方 holdDistance 处
        Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;

        // 根据重量计算跟随速度（越重越慢）
        float speed = holdSpeed / heldItem.weight;
        speed = Mathf.Clamp(speed, 3f, 20f); // 限制最小和最大速度

        // 用 MovePosition 移动，保留物理碰撞
        heldRb.MovePosition(Vector3.Lerp(heldRb.position, targetPos, speed * Time.deltaTime));

        // 同时把旋转慢慢归零，物品不会乱转
        heldRb.MoveRotation(Quaternion.Lerp(heldRb.rotation, Quaternion.identity, 5f * Time.deltaTime));
    }

    void DropItem()
    {
        if (heldItem == null) return;

        // 恢复重力和阻力
        heldRb.useGravity = true;
        heldRb.drag = 3f;

        heldItem.isBeingHeld = false;
        heldItem = null;
        heldRb = null;

        ItemUI.Instance.HideItemInfo();
    }

    void HideOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}