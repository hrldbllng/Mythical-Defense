using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using UnityEngine.UI;

[Serializable]
public class ScoreData
{
    public int score;
}

public class DataSaver : MonoBehaviour
{
    public InputField usernameInputField; // Reference to the InputField for the username
    public GameObject emptyUsernameIndicator; // Reference to the GameObject to show when the username is empty
    public ScoreData scoreData;
    DatabaseReference dbRef;

    

    private void Awake()
    {
        Debug.Log("DataSaver Awake"); // Debug log to indicate script activation
       

        // Initialize FirebaseDatabase reference
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }



    public void SaveScore()
{
    // Check if the usernameInputField is assigned
    if (usernameInputField == null)
    {
        Debug.LogError("usernameInputField is not assigned!");
        return;
    }

    // Get the username from the input field
    string username = usernameInputField.text.Trim(); // Trim to remove leading and trailing spaces

        // Check if the username is empty after trimming
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is empty!");
            // Show the emptyUsernameIndicator GameObject
            if (emptyUsernameIndicator != null)
            {
                emptyUsernameIndicator.SetActive(true);
            }
            return;
        }

        // Hide the emptyUsernameIndicator GameObject if it's currently active
        if (emptyUsernameIndicator != null && emptyUsernameIndicator.activeSelf)
        {
            emptyUsernameIndicator.SetActive(false);
        }

        // Get the score from TimeWatch component
        TimeWatch timeWatch = FindObjectOfType<TimeWatch>();
    if (timeWatch == null)
    {
        Debug.LogError("TimeWatch component not found!");
        return;
    }

    int score = timeWatch.GetElapsedTimeAsScore(); // Get the score

    // Convert score to JSON
    string json = JsonUtility.ToJson(new ScoreData { score = score });

    // Save the score in the database under the user's name
    dbRef.Child("scores").Child(username).SetRawJsonValueAsync(json);
}





    public bool IsInitialized()
    {
        return true; // Assuming DataSaver is always initialized
    }
}
