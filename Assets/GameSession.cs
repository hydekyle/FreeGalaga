using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    private int gamesPlayed = 0;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
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
