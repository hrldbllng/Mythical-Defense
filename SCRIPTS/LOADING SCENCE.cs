using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public int mainGameSceneIndex = 1; // Set the index of your main game scene
    public float delayBeforeLoading = 2f;
    public Slider loadingSlider;

    void Start()
    {
        // Set the initial value of the slider to 0
        loadingSlider.value = 0f;

        // Invoke the LoadMainGameScene method after a delay
        Invoke("LoadMainGameScene", delayBeforeLoading);
    }

    void LoadMainGameScene()
    {
        // Start loading the main game scene asynchronously
        StartCoroutine(LoadMainGameSceneAsync());
    }

    IEnumerator LoadMainGameSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mainGameSceneIndex);

        // Continue updating the slider until the scene is fully loaded
        while (!asyncOperation.isDone)
        {
            // Calculate the loading progress (0 to 1)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Update the slider value
            loadingSlider.value = progress;

            yield return null;
        }
    }
}
