using UnityEngine;
using UnityEngine.UI;

public class TimeWatch : MonoBehaviour
{
    public Text timeText; // Reference to the UI Text component to display the time
    public float elapsedTime { get; private set; } // Elapsed time since the scene started
    private bool timerStarted = false; // Flag to check if the timer has started
    private bool timerPaused = false; // Flag to check if the timer is paused

    public MoneyManager moneyManager; // Reference to the MoneyManager script

    void Update()
    {
        if (IsTowerPresentWithTag("Towers"))
        {
            StartTimer();
        }

        if (timerStarted && !timerPaused)
        {
            UpdateTime();
        }
    }

    void UpdateTime()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimeText();
    }

    void UpdateTimeText()
    {
        // Format the elapsed time as minutes and seconds only
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Update the UI Text component
        if (timeText != null)
        {
            timeText.text = timeString;
        }
    }

    void StartTimer()
    {
        timerStarted = true;
    }

    public bool IsTimerStarted()
    {
        return timerStarted;
    }

    bool IsTowerPresentWithTag(string tag)
    {
        // Check if a tower with the specified tag is present in the scene
        GameObject tower = GameObject.FindGameObjectWithTag(tag);
        return tower != null;
    }

    // Method to get the elapsed time
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    // Method to reset the timer
    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    // Method to pause the timer
    public void PauseTimer()
    {
        timerPaused = true;
    }

    // Method to resume the timer
    public void ResumeTimer()
    {
        timerPaused = false;
    }

    // Method to get the elapsed time as score
    public int GetElapsedTimeAsScore()
    {
        // Return the elapsed time as score + current money
        return Mathf.FloorToInt(elapsedTime) + moneyManager.currentMoney;
    }
}
