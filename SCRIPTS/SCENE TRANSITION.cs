using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public int targetSceneIndex = 1; // Set the index of the target scene
    public GameObject loadingScreen; // Reference to the loading screen UI GameObject
    public Slider loadingSlider; // Reference to the loading screen slider
    public float loadingDelay = 2f; // Set the delay before loading the scene

    void Start()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false); // Hide the loading screen initially
        }
    }

    public void LoadTargetScene()
    {
        if (loadingScreen != null && loadingSlider != null)
        {
            StartCoroutine(LoadSceneWithLoadingScreen());
        }
    }

    IEnumerator LoadSceneWithLoadingScreen()
    {
        loadingScreen.SetActive(true); // Show the loading screen

        // Set the initial value of the loading slider to 0
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
        }

        // Add a delay before loading the target scene
        yield return new WaitForSeconds(loadingDelay);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneIndex);

        // Continue updating the loading screen until the scene is fully loaded
        while (!asyncOperation.isDone)
        {
            // Calculate the loading progress (0 to 1)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Update the loading slider value
            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            yield return null;
        }
    }
}
