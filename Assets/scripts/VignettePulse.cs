using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignettePulse : MonoBehaviour
{
    private Vignette vignette;
    public float pulseIntensity = 0.5f; // Peak intensity during pulse
    public float pulseDuration = 0.2f;  // How quickly the pulse fades out

    private float baseIntensity = 0.2f; // The resting vignette intensity

    void Start()
    {
        // Find the Post Process Volume
        Volume volume = GetComponent<Volume>();
        if (volume != null && volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = baseIntensity;
        }
        else
        {
            Debug.LogError("Vignette not found in the Post Process Volume.");
        }
    }

    private void OnEnable()
    {
        BeatTracker.OnBeat += TriggerVignettePulse;
    }

    private void OnDisable()
    {
        BeatTracker.OnBeat -= TriggerVignettePulse;
    }

    private void TriggerVignettePulse()
    {
        if (vignette != null)
        {
            StopAllCoroutines();
            StartCoroutine(PulseVignette());
        }
    }

    private IEnumerator PulseVignette()
    {
        float elapsed = 0f;
        vignette.intensity.value = pulseIntensity;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(pulseIntensity, baseIntensity, elapsed / pulseDuration);
            yield return null;
        }

        vignette.intensity.value = baseIntensity;
    }
}
