using UnityEngine;
using FMODUnity;

public static class AudioSettingsManager
{
    private static FMOD.Studio.Bus musicBus;
    private static bool initialized = false;
    private const string volumePrefKey = "MusicVolume";

    // Initialize the FMOD bus once
    public static void Init()
    {
        if (!initialized)
        {
            musicBus = RuntimeManager.GetBus("bus:/Music");
            initialized = true;
        }
    }

    // Get saved volume from PlayerPrefs, default 1f
    public static float GetSavedVolume()
    {
        Init();
        return PlayerPrefs.GetFloat(volumePrefKey, 1f);
    }

    // Set volume on FMOD bus and save immediately
    public static void SetVolume(float value)
    {
        Init();
        if (musicBus.isValid())
        {
            musicBus.setVolume(value);
        }
        else
        {
            Debug.LogWarning("FMOD music bus not valid yet!");
        }

        PlayerPrefs.SetFloat(volumePrefKey, value);
        PlayerPrefs.Save();
    }
}
