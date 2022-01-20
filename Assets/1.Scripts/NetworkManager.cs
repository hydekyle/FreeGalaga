using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Security.Cryptography;
using System.Linq;
using Cysharp.Threading.Tasks;

public static class NetworkManager
{
    public static async UniTask<bool> UpdateUserData(string alias, int score)
    {
        bool isNewRecord = false;
        try
        {
            var token = Helpers.GetEncryptedToken(alias, score);
            string url = String.Concat(GameManager.Instance.gameServer.updateScoreURL, String.Format("?alias={0}&score={1}&token={2}&id={3}&avatar={4}", alias, score, token, PlayerPrefs.GetString("id"), GameManager.Instance.user.avatar));
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to server");
            isNewRecord = webRequest.downloadHandler.text == "1" ? true : false;
        }
        catch (Exception err) { Debug.LogWarning(err.Message); }
        return isNewRecord;
    }

    public static async UniTask<User> GetUserDataByID(string id)
    {
        User user = new User();
        try
        {
            var url = GameManager.Instance.gameServer.getUserDataURL + "?id=" + id;
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to database");
            if (webRequest.responseCode != 200) throw new Exception("User have not send any score yet");
            var userDB = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);
            user = new User
            {
                alias = PlayerPrefs.HasKey("alias") ? PlayerPrefs.GetString("alias") : userDB.alias,
                score = userDB.score,
                avatar = PlayerPrefs.HasKey("avatar") ? PlayerPrefs.GetInt("avatar") : userDB.avatar
            };
        }
        catch (Exception err)
        {
            // Generate a new user if we can't get userdata from db
            Debug.LogWarning(err.Message);
            user = new User
            {
                alias = PlayerPrefs.HasKey("alias") ? PlayerPrefs.GetString("alias") : "Player-" + PlayerPrefs.GetString("id").Substring(0, 4),
                score = PlayerPrefs.HasKey("score") ? PlayerPrefs.GetInt("score") : 0,
                avatar = PlayerPrefs.HasKey("avatar") ? PlayerPrefs.GetInt("avatar") : 1
            };
        }
        return user;
    }

    public static async UniTask<TopScore> GetHighScores()
    {
        TopScore topScore = new TopScore();
        try
        {
            var url = GameManager.Instance.gameServer.getHighScoresURL;
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to ranking");
            topScore = JsonUtility.FromJson<TopScore>(webRequest.downloadHandler.text);
        }
        catch (Exception err)
        {
            Debug.LogWarning(err.Message);
            GameManager.Instance.ResetGame();
        }
        return topScore;
    }

    public static async UniTask<GameConfiguration> GetGameConfiguration(GameServer gameServer)
    {
        GameConfiguration gameConfig = new GameConfiguration();
        try
        {
            var url = gameServer.getGameConfigURL;
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't load GameConfig from remote server. Loading default settings.");
            gameConfig = JsonUtility.FromJson<GameConfiguration>(webRequest.downloadHandler.text);
        }
        catch (Exception err)
        {
            Debug.LogWarning(err.Message);
            gameConfig.livesPerCredit = 3;
            gameConfig.playerMovementSpeed = 7;
            gameConfig.playerAttackSpeed = 7;
            gameConfig.storyLevelWaitTime = 0;
            gameConfig.miniBossHealth = 1000;
            gameConfig.finalBossHealth = 2000;
            GameManager.Instance.showStories = false;
        }
        return gameConfig;
    }

}
