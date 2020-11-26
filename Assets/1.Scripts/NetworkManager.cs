using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public static class NetworkManager
{
    public static IEnumerator GetGameConfig (Action<GameConfig> gameConfig)
    {
        var gameConfigURL = "http://hydekyle.ga/galaga/gameconfig.json";
        using (UnityWebRequest webRequest = UnityWebRequest.Get (gameConfigURL))
        {
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError)
            {
                GameConfig config = JsonUtility.FromJson<GameConfig> (webRequest.downloadHandler.text);
                gameConfig (config);
            }
            else
            {
                Debug.LogWarning ("Error al leer el json de configuración, cargando default");
                gameConfig (new GameConfig ()
                {
                    credits_per_player = 3,
                        lives_per_credit = 3,
                        highScoresURL = "",
                        sendScoreURL = ""
                });
            }
        }
    }

    public static IEnumerator SendHighScore (string username, int points, Action<bool> onEnded)
    {
        var sendScoreURL = GameManager.Instance.gameData.sendScoreURL;
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
        var highScoresURL = GameManager.Instance.gameData.highScoresURL;
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
                        var storedUserValues = rawUser.Split ('·');
                        users.Add (new User
                        {
                            username = storedUserValues [0],
                                points = storedUserValues [1],
                                avatar = storedUserValues [2]
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
