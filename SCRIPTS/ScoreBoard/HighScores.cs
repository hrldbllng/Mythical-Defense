using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class HighScores : MonoBehaviour
{
    const string privateCode = "ebe833cabe3348be808c3a8f150222629a3b8cdd2dd98d6c1249e372b23597b3ef2ab8a7903e0c710fedd02aaddf62d67899cc9dcebd3054827f7c187609c255c07e075790aa989f291f312542cdd5c66a98c2b98fe86eb9fe8355f62fd5367f3ed8fe84b429300667f2ff617d6d0b31fcea933eb5c5f9aefc477b6735bb03ef";  //Key to Upload New Info
    const string publicCode = "d67dce3f7f50dd9bb2ff3dba307f066b11eb4db1af9683b281701e2a58b104c7";   //Key to download
    const string webURL = "https://danqzq.itch.io/leaderboard-creator"; // Use HTTP protocol


    public PlayerScore[] scoreList;
    DisplayHighscores myDisplay;

    static HighScores instance; //Required for STATIC usability
    void Awake()
    {
        instance = this; //Sets Static Instance
        myDisplay = GetComponent<DisplayHighscores>();
    }
    
    public static void UploadScore(string username, int score)  //CALLED when Uploading new Score to WEBSITE
    {//STATIC to call from other scripts easily
        instance.StartCoroutine(instance.DatabaseUpload(username,score)); //Calls Instance
    }

    IEnumerator DatabaseUpload(string username, int score)
    {
        // Create the URL for uploading score
        string url = webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score;

        // Set security protocol to allow insecure connections
        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

        // Create a UnityWebRequest object
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // Send the request
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload Successful");
                DownloadScores();
            }
            else
            {
                Debug.LogError("Error uploading: " + www.error);
            }
        }
    }


    public void DownloadScores()
    {
        StartCoroutine("DatabaseDownload");
    }
    IEnumerator DatabaseDownload()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(webURL + publicCode + "/pipe/0/10"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                OrganizeInfo(www.downloadHandler.text);
                myDisplay.SetScoresToMenu(scoreList);
            }
            else
            {
                Debug.LogWarning("Error uploading: " + www.error);
            }
        }
    }


    void OrganizeInfo(string rawData)
    {
        string[] entries = rawData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        scoreList = new PlayerScore[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            if (entryInfo.Length >= 2) // Check if the entry has at least two elements
            {
                string username = entryInfo[0];
                int score;
                if (int.TryParse(entryInfo[1], out score)) // Use TryParse to handle invalid score formats
                {
                    scoreList[i] = new PlayerScore(username, score);
                    print(scoreList[i].username + ": " + scoreList[i].score);
                }
                else
                {
                    Debug.LogWarning("Invalid score format: " + entryInfo[1]);
                }
            }
            else
            {
                Debug.LogWarning("Invalid entry format: " + entries[i]);
            }
        }
    }

}

public struct PlayerScore //Creates place to store the variables for the name and score of each player
{
    public string username;
    public int score;

    public PlayerScore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}