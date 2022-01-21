using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    public string serverURL;
    public User user;
    public GameConfiguration gameConfiguration;
    public ServerEndpoints serverEndpoints;
    public int score = 0;
    public bool isGameReady = false;
    private int gamesPlayed = 0;

    void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    async void Start()
    {
        if (!PlayerPrefs.HasKey("id")) SaveNewUserID();
        serverEndpoints = Helpers.GetServerEndpoints();
        var serverData = await NetworkManager.GetServerDataByUserID(PlayerPrefs.GetString("id"));
        user = serverData.userData;
        gameConfiguration = serverData.gameConfig;
        isGameReady = true;
    }

    void SaveNewUserID()
    {
        var newID = System.Guid.NewGuid().ToString();
        PlayerPrefs.SetString("id", newID);
    }

    public void GameFinished()
    {
        gamesPlayed++;
    }

    public bool IsFirstGame()
    {
        return gamesPlayed == 0;
    }
}
