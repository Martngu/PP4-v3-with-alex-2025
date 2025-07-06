using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPlayerAlive = true;

    [Header("UI References")]
    public GameObject deathPanel;
    public PauseMenu pauseMenu;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
        Debug.Log("Player has died. Stopping all enemies.");

        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.StopMusic();
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        if (pauseMenu != null)
        {
            pauseMenu.ShowDeathUI();
        }

        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {
        if (pauseMenu != null)
        {
            pauseMenu.ShowLevelCompleteUI();
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.CompleteLevel(); 
        }
    }
}
