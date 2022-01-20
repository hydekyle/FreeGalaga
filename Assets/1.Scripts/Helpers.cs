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

    public static async UniTask<User> GetUserData()
    {
        string id = PlayerPrefs.GetString("id");
        var userData = await NetworkManager.GetUserDataByID(id);
        return userData;
    }

    public static GameServer GetGameServer()
    {
        GameServer gameData = new GameServer();
        var serverURL = GameManager.Instance.serverURL;
        gameData.getHighScoresURL = serverURL + "/getScores";
        gameData.getUserDataURL = serverURL + "/getUserDataByID";
        gameData.updateScoreURL = serverURL + "/updateScore";
        gameData.getGameConfigURL = serverURL + "/getGameConfig";
        return gameData;
    }

    public static string GetEncryptedToken(string alias, int points)
    {
        // Sad Encryption to avoid sad trolls
        string token = "";
        token = (alias.Length * points + 7).ToString();
        return token;
    }
}