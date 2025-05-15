using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public GameObject gamePanel, losePanel, winPanel;
    [SerializeField] EventTrigger playAgainButton;
    [SerializeField] EventTrigger backMenuButton;
    [SerializeField] EventTrigger CheckPointButton;
    [SerializeField] EventTrigger nextLevelButton;
    private Player player;

    [Header("Alertas UI")]
    [SerializeField] private TextMeshProUGUI enemyAlertText; // Nombre único
    void Start()
    {
        gamePanel.SetActive(true);
        losePanel.SetActive(false);
        winPanel.SetActive(false);

        player = FindObjectOfType<Player>();
    }

    public void ShowEnemyAlert(string message)
    {
 
        if (enemyAlertText != null)
            enemyAlertText.text = message;
    }

    public void HideEnemyAlert()
    {
        if (enemyAlertText != null)
            enemyAlertText.text = "";
    }
    public void Onlose()
    {
        gamePanel.SetActive(false);
        losePanel.SetActive(true);
        winPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Onwin()
    {
        gamePanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Time.timeScale = 1;
        player.ResetPlayerCollisions();
    }
    public void BackTomenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
        player.ResetPlayerCollisions();
    }
    public void RespawnAtCheckpoint()
    {
        Time.timeScale = 1f;
        losePanel.SetActive(false);
        gamePanel.SetActive(true);
        player.RespawnAtCheckpoint(); // Llama a la función de respawn en el script del jugador
    }
}
