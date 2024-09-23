using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverTransition : MonoBehaviour
{
    public int targetSceneIndex = 1; // Set the index of the target scene
    public GameObject loadingScreen; // Reference to the loading screen UI GameObject
    public Slider loadingSlider; // Reference to the loading screen slider
    public float loadingDelay = 2f; // Set the delay before loading the scene
    public InputField usernameInputField; // Reference to the InputField for the username

    void Start()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false); // Hide the loading screen initially
        }
    }

    public void LoadTargetScene()
    {
        // Check if the input field is assigned and contains text
        if (usernameInputField != null && !string.IsNullOrEmpty(usernameInputField.text))
        {
            if (loadingScreen != null && loadingSlider != null)
            {
                StartCoroutine(LoadSceneWithLoadingScreen());
            }
        }
        else
        {
            Debug.LogWarning("Username is empty. Cannot load target scene.");
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
