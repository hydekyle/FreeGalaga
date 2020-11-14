using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public static class NetworkManager
{
    public static string highScoresURL = "https://hydekyle.ga/scores.php";
    public static string sendScoreURL = "https://hydekyle.ga/updatescore.php";

    public static IEnumerator SetHighScore (string username, int points, Action<bool> onEnded)
    {
        string sendScoreFinalURL = String.Concat (sendScoreURL, String.Format ("/?username={0}&points={1}", username, points));
        using (UnityWebRequest webRequest = UnityWebRequest.Get (sendScoreFinalURL))
        {
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError)
            {
                bool isNewRecord = webRequest.downloadHandler.text == "1" ? true : false;
                onEnded (isNewRecord);
            }
        }
    }

    public static IEnumerator GetHighScores (Action<List<User>> topUsers)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get (highScoresURL))
        {
            List<User> users = new List<User> ();
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError)
            {
                foreach (var rawUser in webRequest.downloadHandler.text.Split ('|'))
                {
                    try
                    {
                        users.Add (new User
                        {
                            username = rawUser.Split (':') [0],
                                points = rawUser.Split (':') [1]
                        });
                    }
                    catch
                    { }
                }
                topUsers (users);
            }
            else Debug.LogWarning ("Error GetHighScore: " + webRequest.error);
        }
    }

}
