using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Security.Cryptography;

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
        var token = GetEncriptedToken (alias, score);
        var sendScoreURL = GameManager.Instance.gameData.sendScoreURL;
        string sendScoreFinalURL = String.Concat (sendScoreURL, String.Format ("?alias={0}&score={1}&token={2}", alias, score, token));
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
        var token = GetEncriptedToken (alias, GameManager.Instance.intentos);
        var consumeIntentoURL = GameManager.Instance.gameData.consumeIntentosURL + "?alias=" + alias + "&token=" + token;
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

    public static IEnumerator GetStories (Action<List<string>> stories)
    {
        var storiesURL = GameManager.Instance.gameData.getStoriesURL;
        using (UnityWebRequest request = UnityWebRequest.Get (storiesURL))
        {
            var storiesList = new List<string> ();
            yield return request.SendWebRequest ();
            if (!request.isNetworkError)
            {
                var fetched_stories = request.downloadHandler.text.Split ('|');
                foreach (string story in fetched_stories) storiesList.Add (story);
                stories (storiesList);
            }
            else
            {
                Debug.LogWarning ("No se ha podido conectar con el servidor remoto.");
            }
        }
    }

    public static IEnumerator CheckLegality (Action<bool> isLegal)
    {
        var legalityURL = "https://hydekyle.ga/galaga/legal.txt";
        using (UnityWebRequest webRequest = UnityWebRequest.Get (legalityURL))
        {
            yield return webRequest.SendWebRequest ();
            if (!webRequest.isNetworkError && webRequest.downloadHandler.text.Substring (0, 1) == "*") isLegal (false);
            else isLegal (true);
        }
    }

    public static string Md5Sum (string strToEncrypt)
    {
        System.Text.UTF32Encoding ue = new System.Text.UTF32Encoding ();
        byte [] bytes = ue.GetBytes (strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
        byte [] hashBytes = md5.ComputeHash (bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString (hashBytes [i], 16).PadLeft (2, '0');
        }

        return hashString.PadLeft (32, '0');
    }

    public static string GetEncriptedToken (string alias, int points)
    {
        string token = "";
        token = (alias.Length * points + 7).ToString ();
        return token;
    }

}
