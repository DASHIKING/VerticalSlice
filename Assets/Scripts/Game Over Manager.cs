using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI References")]
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;
    }

    public void ShowGameOver()
    {
        // 显示 Game Over 画面
        gameOverPanel.SetActive(true);

        // 暂停游戏
        Time.timeScale = 0f;

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        // 恢复时间
        Time.timeScale = 1f;

        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}