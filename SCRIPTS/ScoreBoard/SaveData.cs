using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    public Text myTimeText; // Change the name to reflect time
    public InputField myName;
    private TimeWatch timeWatch; // Reference to the TimeWatch script

    void Start()
    {
        // Find and store reference to the TimeWatch script in the scene
        timeWatch = FindObjectOfType<TimeWatch>();
    }

    void Update()
    {
        // Update the time text to display the elapsed time
        myTimeText.text = $"SCORE: {timeWatch.GetElapsedTimeAsScore()}";
    }

    public void SendScore()
    {
        // Get the elapsed time as a string
        int elapsedTime = timeWatch.GetElapsedTimeAsScore();

        

        // Upload the time along with the player's name to an external service
        HighScores.UploadScore(myName.text, elapsedTime);
    }

    private int ConvertElapsedTimeToInt(string elapsedTime)
    {
        // Split the elapsed time string into minutes and seconds
        string[] timeComponents = elapsedTime.Split(':');
        int minutes = int.Parse(timeComponents[0]);
        int seconds = int.Parse(timeComponents[1]);

        // Convert the time to seconds
        int totalSeconds = minutes * 60 + seconds;
        return totalSeconds;
    }
}
