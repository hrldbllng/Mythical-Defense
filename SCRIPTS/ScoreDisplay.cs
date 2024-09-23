using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Linq;
using System.Text;

public class ScoreDisplay : MonoBehaviour
{
    public Text rankTextPrefab;
    public Text nameTextPrefab;
    public Text scoreTextPrefab;
    public Transform rankListParent;
    public Transform nameListParent;
    public Transform scoreListParent;

    // Interval for checking for updates (in seconds)
    public float updateInterval = 10f;

    DatabaseReference dbRef;

    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
                return;
            }

            // Firebase has been successfully initialized, get the root reference
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            // Start periodic updates
            StartCoroutine(PeriodicUpdateScores());
        });
    }

    IEnumerator PeriodicUpdateScores()
    {
        while (true)
        {
            LoadAllScores();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void LoadAllScores()
    {
        Debug.Log("Loading scores...");
        dbRef.Child("scores").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading scores: " + task.Exception.Message);
                return;
            }

            DataSnapshot snapshot = task.Result;
            if (snapshot == null || !snapshot.Exists)
            {
                Debug.LogError("Snapshot is null or does not exist.");
                return;
            }

            Dictionary<string, int> allScores = new Dictionary<string, int>();

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string username = childSnapshot.Key;
                // Check if the "score" child exists before accessing its value
                if (childSnapshot.Child("score").Exists)
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    allScores.Add(username, score);
                }
                else
                {
                    Debug.LogWarning("Score data not found for user: " + username);
                }
            }

            DisplayScores(allScores);
        });
    }

    public void DisplayScores(Dictionary<string, int> scores)
    {
        List<KeyValuePair<string, int>> sortedScores = scores.ToList();
        sortedScores.Sort((x, y) => y.Value.CompareTo(x.Value));

        rankTextPrefab.text = "";
        nameTextPrefab.text = "";
        scoreTextPrefab.text = "";

        StringBuilder rankBuilder = new StringBuilder();
        StringBuilder nameBuilder = new StringBuilder();
        StringBuilder scoreBuilder = new StringBuilder();

        foreach (var kvp in sortedScores)
        {
            rankBuilder.AppendLine((sortedScores.IndexOf(kvp) + 1).ToString());
            nameBuilder.AppendLine(kvp.Key);
            scoreBuilder.AppendLine(kvp.Value.ToString());
        }

        rankTextPrefab.text = rankBuilder.ToString();
        nameTextPrefab.text = nameBuilder.ToString();
        scoreTextPrefab.text = scoreBuilder.ToString();
    }

    void ClearDisplay()
    {
        foreach (Transform child in rankListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in nameListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in scoreListParent)
        {
            Destroy(child.gameObject);
        }
    }

    void AddScoreToDisplay(int rank, string name, int score)
    {
        InstantiateText(rankTextPrefab, rankListParent, rank.ToString());
        InstantiateText(nameTextPrefab, nameListParent, name);
        InstantiateText(scoreTextPrefab, scoreListParent, score.ToString());
    }

    void InstantiateText(Text prefab, Transform parent, string value)
    {
        Text text = Instantiate(prefab, parent);
        text.text = value;
    }
}
