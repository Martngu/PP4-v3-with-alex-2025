using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPlayerAlive = true;

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

        // Find the BeatTracker in the scene and stop music
        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.StopMusic();
        }
    }

}
