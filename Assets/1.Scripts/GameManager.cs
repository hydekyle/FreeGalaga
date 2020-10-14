using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public bool gameIsActive = false;
    public int activeLevelNumber = 0;
    public ScriptableLevels levelsTables;

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
        CanvasManager.Instance.SetLevelNumber (levelNumber);
        CanvasManager.Instance.SetBarColor (GetColorByLevel (levelNumber));
    }

    public void LevelCompleted ()
    {
        gameIsActive = false;
        Invoke ("LoadNextLevel", 1f);
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

    Color GetColorByLevel (int levelNumber)
    {
        Color color;
        switch (levelNumber)
        {
            case 1:
                color = levelsTables.barColorLevel1;
                break;
            case 2:
                color = levelsTables.barColorLevel2;
                break;
            case 3:
                color = levelsTables.barColorLevel3;
                break;
            case 4:
                color = levelsTables.barColorLevel4;
                break;
            default:
                color = levelsTables.barColorLevelFinal;
                break;
        }
        color.a = 1;
        return color;
    }

    public void DesactivateOnTime (GameObject go, float time)
    {
        StartCoroutine (_DesactivateOnTime (go, time));
    }

    IEnumerator _DesactivateOnTime (GameObject go, float time)
    {
        yield return new WaitForSeconds (time);
        go.SetActive (false);
    }

}
