using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Collection Points")]
    public int totalCollectionPoints = 1;
    private int completedPoints = 0;
    private bool canEscape = false;

    [Header("UI")]
    public TextMeshProUGUI escapeStatusText;
    public GameObject winPanel;
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;

        if (winPanel != null)
            winPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (escapeStatusText != null)
            escapeStatusText.text = "0/" + totalCollectionPoints + " completed";
    }

    public void OnCollectionPointCompleted()
    {
        completedPoints++;
        canEscape = completedPoints >= totalCollectionPoints;

        if (escapeStatusText != null)
        {
            if (canEscape)
                escapeStatusText.text = "Return to start to escape!";
            else
                escapeStatusText.text = completedPoints +
                    "/" + totalCollectionPoints + " completed";
        }
    }

    public bool CanEscape()
    {
        return canEscape;
    }

    public void WinGame()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}