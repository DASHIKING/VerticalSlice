using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    private bool playerInside = false;

    void Update()
    {
        
    
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance != null && GameManager.Instance.CanEscape())
                GameManager.Instance.WinGame();
            else
                Debug.Log("还没完成收集点!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("EscapeZone 检测到: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("玩家进入逃离区域!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}