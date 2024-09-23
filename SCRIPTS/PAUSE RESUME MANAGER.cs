using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseResumeManager : MonoBehaviour
{
    public Button pauseButton;
    public List<Button> resumeButtons;
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }

        foreach (Button resumeButton in resumeButtons)
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        // You can add other update logic here
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Set the time scale to 0 to pause the game
        isPaused = true;

        // Show the pause panel or perform other pause actions
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Set the time scale back to 1 to resume the game
        isPaused = false;

        // Hide the pause panel or perform other resume actions
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
