using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelSelectPanel;
    public GameObject optionsPanel;
    public Slider volumeSlider;

    private bool isInitializingSlider = false;

    private void Start()
    {
        float savedVolume = AudioSettingsManager.GetSavedVolume();

        // Prevent triggering slider change callback when setting initial value
        isInitializingSlider = true;
        volumeSlider.value = savedVolume;
        AudioSettingsManager.SetVolume(savedVolume);
        isInitializingSlider = false;

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackFromOptions()
    {
        AudioSettingsManager.SetVolume(volumeSlider.value);
        ShowMainMenu();
    }

    public void OnVolumeSliderChanged(float value)
    {
        if (isInitializingSlider) return;

        AudioSettingsManager.SetVolume(value);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
