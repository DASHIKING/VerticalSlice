using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 4f;

    [Header("Hold Settings")]
    public float holdDistance = 2.5f;
    public float holdSpeed = 15f;

    [Header("Handcart Settings")]
    public float cartDetectRadius = 2.5f;  // 靠近多远可以推车
    public float cartFollowSpeed = 8f;     // 推车跟随速度
    public float cartFollowDistance = 1.5f; // 车跟在玩家前方多远

    private Camera cam;
    private InteractableItem currentLookedAt;
    private Outline currentOutline;
    private InteractableItem heldItem;
    private Rigidbody heldRb;

    private HandcartController nearbyCart;
    private HandcartController grabbedCart;  // 当前推着的车
    private Rigidbody cartRb;
    private bool isPushingCart = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        DetectNearbyCart();
        DetectItem();
        HandleHolding();
        HandleCartPushing();

        // 鼠标左键：拿起/放下物品
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
                TryPickUp();
            else
                TryDropOrAddToCart();
        }

        // E 键：开始/停止推车
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPushingCart)
                TryStartPushingCart();
            else
                StopPushingCart();
        }
    }

    // ── 检测附近推车 ─────────────────────────────────────
    void DetectNearbyCart()
    {
        Collider[] cols = Physics.OverlapSphere(
            transform.position, cartDetectRadius);

        nearbyCart = null;
        foreach (Collider col in cols)
        {
            

            HandcartController cart = col.GetComponent<HandcartController>();
            if (cart == null)
                cart = col.GetComponentInParent<HandcartController>();
            if (cart != null)
            {
                nearbyCart = cart;
                
                break;
            }
        }
    }

    // ── E 键推车 ─────────────────────────────────────────
    void TryStartPushingCart()
    {
        if (nearbyCart == null) return;
        if (heldItem != null) return;

        grabbedCart = nearbyCart;
        cartRb = grabbedCart.GetComponent<Rigidbody>();
        isPushingCart = true;
        cartRb.drag = 10f;
        Debug.Log("开始推车!");
    }

    void StopPushingCart()
    {
        if (cartRb != null)
            cartRb.drag = 3f;

        grabbedCart = null;
        cartRb = null;
        isPushingCart = false;
        Debug.Log("停止推车!");
    }

    void HandleCartPushing()
    {
        if (!isPushingCart || cartRb == null) return;

        Vector3 targetPos = transform.position + 
            transform.forward * cartFollowDistance;
    
        targetPos.y = cartRb.position.y;

        cartRb.MovePosition(Vector3.Lerp(
            cartRb.position, targetPos, cartFollowSpeed * Time.deltaTime));

        // 加180度让车正面朝向玩家
        Quaternion targetRot = Quaternion.Euler(
            0, transform.eulerAngles.y + 90f, 0);
        cartRb.MoveRotation(Quaternion.Lerp(
            cartRb.rotation, targetRot, 5f * Time.deltaTime));
    }

    // ── 物品检测高亮 ─────────────────────────────────────
    void DetectItem()
    {
        if (heldItem != null || isPushingCart)
        {
            HideOutline();
            currentLookedAt = null;
            return;
        }

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2));
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

    // ── 拿起物品 ─────────────────────────────────────────
    void TryPickUp()
    {
        if (currentLookedAt == null) return;
        if (isPushingCart) return;

        float dist = Vector3.Distance(
            transform.position, currentLookedAt.transform.position);
        if (dist > interactRange) return;

        heldItem = currentLookedAt;
        heldRb = heldItem.GetComponent<Rigidbody>();
        heldRb.useGravity = false;
        heldRb.drag = 10f;
        heldItem.isBeingHeld = true;
        HideOutline();

        ItemUI.Instance.ShowItemInfo(heldItem.itemName, heldItem.value);
    }

    // ── 放下物品或放入车 ─────────────────────────────────
    void TryDropOrAddToCart()
    {
        if (heldItem == null) return;

        if (nearbyCart != null)
            AddItemToCart();
        else
            DropItem();
    }

    void AddItemToCart()
    {
        if (heldItem == null || nearbyCart == null) return;

        heldRb.useGravity = true;
        heldRb.drag = 3f;
        heldItem.isBeingHeld = false;

        nearbyCart.AddItem(heldItem);

        ItemUI.Instance.HideItemInfo();
        heldItem = null;
        heldRb = null;
    }

    void DropItem()
    {
        if (heldItem == null) return;

        heldRb.useGravity = true;
        heldRb.drag = 3f;
        heldItem.isBeingHeld = false;
        heldItem = null;
        heldRb = null;

        ItemUI.Instance.HideItemInfo();
    }

    // ── 持有物品跟随 ─────────────────────────────────────
    void HandleHolding()
    {
        if (heldItem == null) return;

        Vector3 targetPos = cam.transform.position +
            cam.transform.forward * holdDistance;
        float speed = holdSpeed / heldItem.weight;
        speed = Mathf.Clamp(speed, 3f, 20f);

        heldRb.MovePosition(Vector3.Lerp(
            heldRb.position, targetPos, speed * Time.deltaTime));
        heldRb.MoveRotation(Quaternion.Lerp(
            heldRb.rotation, Quaternion.identity, 5f * Time.deltaTime));
    }

    void HideOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }
}