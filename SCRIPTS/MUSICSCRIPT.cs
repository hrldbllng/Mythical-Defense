using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; // Reference to the AudioSource component for music.
    public Slider musicSlider; // Reference to the Slider component for music volume.

    void Start()
    {
        // Load music volume from player preferences or set a default value.
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSlider.value = savedVolume;
        SetMusicVolume(savedVolume);

        // Set the music to loop.
        musicSource.loop = true;
    }

    void Update()
    {
        // Check if the music has ended and restart it.
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        // Set music volume and save it to player preferences.
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
