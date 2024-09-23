using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{
    public Slider audioSlider; // Reference to the UI Slider for controlling volume
    public Transform soundEffectsParent; // Reference to the parent GameObject containing sound effect AudioSources

    private void Start()
    {
        // Add listener to the slider's value changed event
        audioSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float volume)
    {
        // Clamp the volume value between 0 and 1
        float clampedVolume = Mathf.Clamp01(volume);

        // Iterate through all AudioSources under the parent GameObject and set their volume
        foreach (AudioSource audioSource in soundEffectsParent.GetComponentsInChildren<AudioSource>())
        {
            audioSource.volume = clampedVolume;
        }
    }
}
