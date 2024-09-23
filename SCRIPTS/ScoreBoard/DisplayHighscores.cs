using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour 
{
    public Text[] rNames;
    public Text[] rScores;
    HighScores myScores;

    void Start()
    {
        for (int i = 0; i < rNames.Length; i++)
        {
            rNames[i].text = i + 1 + ". Fetching...";
        }

        // Ensure that the HighScores component is available
        myScores = GetComponent<HighScores>();
        if (myScores != null)
        {
            StartCoroutine("RefreshHighscores");
        }
        else
        {
            Debug.LogError("HighScores component not found!");
        }
    }


    public void SetScoresToMenu(PlayerScore[] highscoreList) //Assigns proper name and score for each text value
    {
        for (int i = 0; i < rNames.Length;i ++)
        {
            rNames[i].text = i + 1 + ". ";
            if (highscoreList.Length > i)
            {
                rScores[i].text = highscoreList[i].score.ToString();
                rNames[i].text = highscoreList[i].username;
            }
        }
    }
    IEnumerator RefreshHighscores() //Refreshes the scores every 30 seconds
    {
        while(true)
        {
            myScores.DownloadScores();
            yield return new WaitForSeconds(30);
        }
    }
}
