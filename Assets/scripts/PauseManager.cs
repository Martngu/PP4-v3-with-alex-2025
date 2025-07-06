using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject playerDeathUI;
    public GameObject levelCompleteUI;
    public Slider optionsVolumeSlider;

    private bool isPaused = false;
    private EventInstance musicInstance;
    private bool musicWasPlaying = false;
    private bool musicInstanceValid = false;

    void Start()
    {
        // Try BeatTracker (level 1)
        var beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            musicInstance = beatTracker.GetMusicInstance();
            musicInstanceValid = true;
        }
        else
        {
            // Try BeatTracker2 (level 2)
            var beatTracker2 = FindFirstObjectByType<BeatTracker2>();
            if (beatTracker2 != null)
            {
                // Access musicInstance via reflection (or add a public getter to BeatTracker2)
                var field = typeof(BeatTracker2).GetField("musicInstance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    musicInstance = (EventInstance)field.GetValue(beatTracker2);
                    musicInstanceValid = true;
                }
            }
        }

        if (!musicInstanceValid)
        {
            Debug.LogWarning("No BeatTracker or BeatTracker2 found. Music control disabled.");
        }

        float savedVolume = AudioSettingsManager.GetSavedVolume();
        optionsVolumeSlider.value = savedVolume;
        AudioSettingsManager.SetVolume(savedVolume);
        optionsVolumeSlider.onValueChanged.AddListener(OnOptionsVolumeChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);

        if (musicInstanceValid)
        {
            musicInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState == PLAYBACK_STATE.PLAYING)
            {
                musicInstance.setPaused(true);
                musicWasPlaying = true;
            }
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);

        if (musicInstanceValid && musicWasPlaying)
        {
            musicInstance.setPaused(false);
        }
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowDeathUI()
    {
        playerDeathUI.SetActive(true);
        Time.timeScale = 0f;
        if (musicInstanceValid)
        {
            musicInstance.setPaused(true);
        }
    }

    public void ShowLevelCompleteUI()
    {
        levelCompleteUI.SetActive(true);
        Time.timeScale = 0f;
        if (musicInstanceValid)
        {
            musicInstance.setPaused(true);
        }
    }

    public void OnOptionsVolumeChanged(float value)
    {
        AudioSettingsManager.SetVolume(value);
    }
}
