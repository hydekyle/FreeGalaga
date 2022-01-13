using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Security.Cryptography;
using System.Linq;

public static class NetworkManager
{
    public static IEnumerator SendHighScore(string alias, int score, Action<bool> onEnded)
    {
        var token = GetEncriptedToken(alias, score);
        var sendScoreURL = GameManager.Instance.gameData.sendScoreURL;
        string sendScoreFinalURL = String.Concat(sendScoreURL, String.Format("?alias={0}&score={1}&token={2}&id={3}&avatar={4}", alias, score, token, PlayerPrefs.GetString("id"), GameManager.Instance.user.avatar));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(sendScoreFinalURL))
        {
            bool isNewRecord = false;
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError)
                isNewRecord = webRequest.downloadHandler.text == "1" ? true : false;
            onEnded(isNewRecord);
        }
    }

    public static IEnumerator GetUserData(string id, Action<User> userData)
    {
        var getUserDataURL = GameManager.Instance.gameData.getUserDataURL + "?id=" + id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(getUserDataURL))
        {
            yield return webRequest.SendWebRequest();
            try
            {
                if (webRequest.result != UnityWebRequest.Result.ConnectionError)
                {
                    if (webRequest.responseCode != 200) throw new Exception("User have not send any score yet");
                    var storedUserValues = webRequest.downloadHandler.text.Split('|');
                    userData(new User
                    {
                        id = storedUserValues[0],
                        alias = storedUserValues[1],
                        score = storedUserValues[2],
                        avatar = int.Parse(storedUserValues[3])
                    });
                }
                else
                {
                    throw new Exception("Can't connect to database");
                }
            }
            catch (Exception err)
            {
                // Generate a new user if we can't get userdata from db
                Debug.LogWarning(err.Message);
                string newID = PlayerPrefs.GetString("id");
                userData(new User
                {
                    id = newID,
                    alias = PlayerPrefs.HasKey("alias") ? PlayerPrefs.GetString("alias") : "Player-" + newID.Substring(0, 4),
                    score = PlayerPrefs.HasKey("score") ? PlayerPrefs.GetInt("score").ToString() : "0",
                    avatar = PlayerPrefs.HasKey("avatar") ? PlayerPrefs.GetInt("avatar") : 1
                });
            }
        }
    }

    public static IEnumerator GetHighScores(Action<List<User>> topUsers)
    {
        var highScoresURL = GameManager.Instance.gameData.getHighScoresURL;
        List<User> users = new List<User>();
        using (UnityWebRequest webRequest = UnityWebRequest.Get(highScoresURL))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError)
            {
                foreach (var rawUser in webRequest.downloadHandler.text.Split('|'))
                {
                    try
                    {
                        var storedUserValues = rawUser.Split('·');
                        users.Add(new User
                        {
                            alias = storedUserValues[0],
                            score = storedUserValues[1],
                            avatar = int.Parse(storedUserValues[2])
                        });
                    }
                    catch
                    { }
                }
            }
            else
            {
                Debug.LogWarning("Can't connect to ranking");
                GameManager.Instance.ResetGame();
            }
            topUsers(users);
        }
    }

    public static IEnumerator GetGameConfiguration(Action<GameConfiguration> gameConfiguration)
    {
        var storiesURL = GameManager.Instance.gameData.gameDataURL;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(storiesURL))
        {
            GameConfiguration gameConfig = new GameConfiguration();
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError)
            {
                var fetched_data = webRequest.downloadHandler.text.Split('|').ToList();
                gameConfig.livesPerCredit = int.Parse(fetched_data[0]);
                gameConfig.playerMovementSpeed = int.Parse(fetched_data[1]);
                gameConfig.playerAttackSpeed = int.Parse(fetched_data[2]);
                gameConfig.storyLevelWaitTime = int.Parse(fetched_data[3]);
                gameConfig.miniBossHealth = int.Parse(fetched_data[4]);
                gameConfig.finalBossHealth = int.Parse(fetched_data[5]);
                fetched_data.RemoveRange(0, 6);
                var storiesList = new List<string>();
                foreach (string story in fetched_data) storiesList.Add(story);
                gameConfig.stories = storiesList;
            }
            else
            {
                Debug.LogWarning("Can't get GameConfig from remote server. Default settings loaded.");
                gameConfig.livesPerCredit = 3;
                gameConfig.playerMovementSpeed = 7;
                gameConfig.playerAttackSpeed = 7;
                gameConfig.storyLevelWaitTime = 0;
                gameConfig.miniBossHealth = 1000;
                gameConfig.finalBossHealth = 2000;
                gameConfig.stories = Helpers.GetDebugStories();
                GameManager.Instance.showStories = false;
            }
            gameConfiguration(gameConfig);
        }
    }

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF32Encoding ue = new System.Text.UTF32Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    public static string GetEncriptedToken(string alias, int points)
    {
        string token = "";
        token = (alias.Length * points + 7).ToString();
        return token;
    }

}
