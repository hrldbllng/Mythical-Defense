using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypingScript : MonoBehaviour
{
    public Text sentenceText; // Reference to the UI Text component to display the sentences
    public Button nextButton; // Reference to the UI Button for proceeding to the next sentence
    public Image backgroundImage; // Reference to the UI Image component for background picture
    public Sprite[] backgroundSprites; // Array of background pictures corresponding to each sentence
    public string[] sentences; // Array of sentences to display
    public int finalSceneIndex = 2; // Index of the final scene to load after displaying all sentences

    private int currentIndex = -1; // Index of the current sentence being displayed (-1 means not started yet)
    private bool isTyping = false; // Flag to indicate if the typing animation is in progress
    private Coroutine typingCoroutine; // Reference to the coroutine for typing animation

    void Start()
    {
        // Ensure the next button is always visible
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.onClick.AddListener(NextSentence);
        }

        // Start typing the first sentence
        NextSentence();
    }

    void StartTyping(string sentence)
    {
        if (!isTyping)
        {
            typingCoroutine = StartCoroutine(TypeSentence(sentence));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        sentenceText.text = ""; // Clear the text initially

        // Iterate through each character in the sentence
        foreach (char letter in sentence)
        {
            sentenceText.text += letter; // Add the current letter to the text
            yield return new WaitForSeconds(0.05f); // Adjust typing speed here
        }

        isTyping = false;
    }

    void NextSentence()
    {
        // Check if the typing animation is ongoing
        if (isTyping)
        {
            // If typing, complete the current sentence immediately
            SkipTyping();
            return;
        }

        currentIndex++; // Move to the next sentence index

        // Check if there are more sentences to display
        if (currentIndex < sentences.Length)
        {
            // Start typing the next sentence
            StartTyping(sentences[currentIndex]);

            // Change background picture if available
            if (backgroundImage != null && currentIndex < backgroundSprites.Length)
            {
                backgroundImage.sprite = backgroundSprites[currentIndex];
            }
        }
        else
        {
            // No more sentences, load the final scene
            LoadFinalScene();
        }
    }

    // Allow the user to skip typing and instantly display the full sentence by clicking the next button
    public void SkipTyping()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            sentenceText.text = sentences[currentIndex]; // Display the full sentence
            isTyping = false;
            nextButton.gameObject.SetActive(true); // Show the next button
        }
    }

    void LoadFinalScene()
    {
        if (finalSceneIndex >= 0 && finalSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(finalSceneIndex);
        }
        else
        {
            Debug.LogWarning("Invalid final scene index: " + finalSceneIndex);
        }
    }
}
