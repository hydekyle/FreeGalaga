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
    public static async UniTask<TopScore> UpdateUserDataAndGetTopScore()
    {
        TopScore topScore = new TopScore();
        try
        {
            var id = PlayerPrefs.GetString("id");
            var alias = GameSession.Instance.user.alias;
            var avatar = GameSession.Instance.user.avatar;
            var score = GameManager.Instance.score;
            var token = Helpers.GetEncryptedToken(alias, score);
            string url = String.Concat(GameSession.Instance.serverEndpoints.updateScoreURL, String.Format("?alias={0}&score={1}&token={2}&id={3}&avatar={4}", alias, score, token, id, avatar));
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to server");
            topScore = JsonUtility.FromJson<TopScore>(webRequest.downloadHandler.text);
        }
        catch (Exception err) { Debug.LogWarning(err.Message); }
        return topScore;
    }

    public static async UniTask<GameDataResponse> GetServerDataByUserID(string id)
    {
        GameDataResponse gameDataResponse = new GameDataResponse();
        try
        {
            var url = GameSession.Instance.serverEndpoints.getGameDataURL + "?id=" + id;
            var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to database");
            gameDataResponse = JsonUtility.FromJson<GameDataResponse>(webRequest.downloadHandler.text);
            if (gameDataResponse.userData.alias == null) throw new Exception("User have not send any score yet");
            var userDB = gameDataResponse.userData;
            gameDataResponse.userData = new User
            {
                alias = PlayerPrefs.HasKey("alias") ? PlayerPrefs.GetString("alias") : userDB.alias,
                score = userDB.score,
                avatar = PlayerPrefs.HasKey("avatar") ? PlayerPrefs.GetInt("avatar") : userDB.avatar
            };
        }
        catch (Exception err)
        {
            // Load new local User and GameData
            Debug.LogWarning(err.Message);
            var aliasDefault = "Player-" + PlayerPrefs.GetString("id").Substring(0, 4);
            gameDataResponse.userData = new User
            {
                alias = PlayerPrefs.HasKey("alias") ? PlayerPrefs.GetString("alias") : aliasDefault,
                score = PlayerPrefs.HasKey("score") ? PlayerPrefs.GetInt("score") : 0,
                avatar = PlayerPrefs.HasKey("avatar") ? PlayerPrefs.GetInt("avatar") : 1
            };
        }
        if (gameDataResponse.gameConfig.playerMovementSpeed == 0) gameDataResponse.gameConfig = Helpers.GetGameConfigOffline();
        return gameDataResponse;
    }

    // public static async UniTask<GameConfiguration> GetGameConfiguration(GameServer gameServer)
    // {
    //     GameConfiguration gameConfig = new GameConfiguration();
    //     try
    //     {
    //         var url = gameServer.getGameDataURL;
    //         var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
    //         if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't load GameConfig from remote server. Loading default settings.");
    //         gameConfig = JsonUtility.FromJson<GameConfiguration>(webRequest.downloadHandler.text);
    //     }
    //     catch (Exception err)
    //     {
    //         Debug.LogWarning(err.Message);

    //     }
    //     return gameConfig;
    // }

    // // We fetch high scores as response when player send score to avoid x2 calls
    // public static async UniTask<TopScore> GetHighScores()
    // {
    //     TopScore topScore = new TopScore();
    //     try
    //     {
    //         var url = GameManager.Instance.gameServer.getHighScoresURL;
    //         var webRequest = await UnityWebRequest.Get(url).SendWebRequest();
    //         if (webRequest.result == UnityWebRequest.Result.ConnectionError) throw new Exception("Can't connect to ranking");
    //         topScore = JsonUtility.FromJson<TopScore>(webRequest.downloadHandler.text);
    //     }
    //     catch (Exception err)
    //     {
    //         Debug.LogWarning(err.Message);
    //         GameManager.Instance.ResetGame();
    //     }
    //     return topScore;
    // }

}
