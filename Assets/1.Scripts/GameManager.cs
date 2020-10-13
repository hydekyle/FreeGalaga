using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public bool gameIsActive = false;
    public int activeLevelNumber = 0;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        StartGame ();
    }

    public void StartGame ()
    {
        AudioManager.Instance.StartMusic ();
        LoadLevel (++activeLevelNumber);
    }

    public void LoadNextLevel ()
    {
        LoadLevel (++activeLevelNumber);
    }

    public void GameOver ()
    {
        player.gameObject.SetActive (false);
        EnemiesManager.Instance.StopEnemies ();
        gameIsActive = false;
    }

    private void Update ()
    {
        if (Input.GetKey (KeyCode.Mouse0) && gameIsActive) player.Disparar ();
        if (Input.GetKeyDown (KeyCode.Mouse0) && !player.gameObject.activeSelf) RestartLevel ();
        if (Input.GetKeyDown (KeyCode.Mouse1)) EnemiesManager.Instance.MakeCrazyEnemy ();
    }

    public void RestartLevel ()
    {
        player.gameObject.SetActive (true);
        LoadLevel (activeLevelNumber);
    }

    public int GetLevelNumber ()
    {
        return activeLevelNumber;
    }

    public ScriptableLevels levelsTables;

    void LoadLevel (int levelNumber)
    {
        Sprite background;
        float animationSpeed;

        switch (levelNumber)
        {
            case 2:
                background = levelsTables.backgroundLevel2;
                animationSpeed = levelsTables.animSpeed2;
                break;
            default:
                background = levelsTables.backgroundLevel1;
                animationSpeed = levelsTables.animSpeed1;
                break;
        }

        CanvasManager.Instance.SetBackground (background);
        EnemiesManager.Instance.LoadEnemies ();
        EnemiesManager.Instance.SetAnimationSpeed (animationSpeed);
    }

    public void LevelCompleted ()
    {
        gameIsActive = false;
        Invoke ("LoadNextLevel", 1f);
    }

}
