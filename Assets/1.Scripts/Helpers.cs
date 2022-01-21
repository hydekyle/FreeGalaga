using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Helpers
{
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

    public static async UniTask<GameDataResponse> GetGameData()
    {
        string id = PlayerPrefs.GetString("id");
        var userData = await NetworkManager.GetServerDataByUserID(id);
        return userData;
    }

    public static ServerEndpoints GetServerEndpoints()
    {
        ServerEndpoints gameServer = new ServerEndpoints();
        var serverURL = GameSession.Instance.serverURL;
        gameServer.getHighScoresURL = serverURL + "/getScores";
        gameServer.updateScoreURL = serverURL + "/updateScore";
        gameServer.getGameDataURL = serverURL + "/getGameData";
        return gameServer;
    }

    public static string GetEncryptedToken(string alias, int points)
    {
        // Sad Encryption to avoid sad trolls
        string token = "";
        token = (alias.Length * points + 7).ToString();
        return token;
    }

    public static GameConfiguration GetGameConfigOffline()
    {
        GameConfiguration gameConfig = new GameConfiguration();
        gameConfig.livesPerCredit = 1;
        gameConfig.playerMovementSpeed = 10;
        gameConfig.playerAttackSpeed = 10;
        gameConfig.storyLevelWaitTime = 0;
        gameConfig.miniBossHealth = 1000;
        gameConfig.finalBossHealth = 2000;
        GameManager.Instance.showStories = false;
        return gameConfig;
    }
}