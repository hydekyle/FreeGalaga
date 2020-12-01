using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public static class NetworkManager
{
    public static IEnumerator GetGameConfig (string gameConfigURL, Action<GameConfig> gameConfig)
    {
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
                var newGameConfig = new GameConfig ()
                {
                    lives_per_credit = 3
                };
                gameConfig (newGameConfig);
            }
        }
    }

    public static IEnumerator SendHighScore (string alias, int score, Action<bool> onEnded)
    {
        var sendScoreURL = GameManager.Instance.gameData.sendScoreURL;
        string sendScoreFinalURL = String.Concat (sendScoreURL, String.Format ("/?alias={0}&score={1}", alias, score));
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

    public static IEnumerator ConsumeIntento (string alias, Action onConsumed)
    {
        var consumeIntentoURL = GameManager.Instance.gameData.consumeIntentosURL + "?alias=" + alias;
        using (UnityWebRequest webRequest = UnityWebRequest.Get (consumeIntentoURL))
        {
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError)
            {
                if (webRequest.downloadHandler.text == "OK")
                {
                    onConsumed ();
                }
            }
        }
    }

    public static IEnumerator GetUserData (string alias, Action<User> userData)
    {
        var getUserDataURL = GameManager.Instance.gameData.getUserDataURL + "?alias=" + alias;
        using (UnityWebRequest webRequest = UnityWebRequest.Get (getUserDataURL))
        {
            User user = new User ();
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError)
            {
                foreach (var rawUser in webRequest.downloadHandler.text.Split ('|'))
                {
                    try
                    {
                        var storedUserValues = rawUser.Split ('·');
                        user = new User
                        {
                            alias = storedUserValues [0],
                            score = storedUserValues [1],
                            avatar = storedUserValues [2],
                            intentos = storedUserValues [3]
                        };
                    }
                    catch
                    { }
                }
            }
            userData (user);
        }
    }

    public static IEnumerator GetHighScores (Action<List<User>> topUsers)
    {
        var highScoresURL = GameManager.Instance.gameData.getHighScoresURL;
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
                        var newUser = new User
                        {
                            alias = storedUserValues [0],
                            score = storedUserValues [1],
                            avatar = storedUserValues [2]
                        };
                        users.Add (newUser);
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
