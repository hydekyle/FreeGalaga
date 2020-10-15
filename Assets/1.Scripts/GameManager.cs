using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public bool gameIsActive = false;
    public int activeLevelNumber = 0;
    public ScriptableLevels levelsTables;
    public int lives = 3;

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
        if (lives == 0) return;
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
        EnemiesManager.Instance.ClearAllEnemies ();
        Invoke ("LoadNextLevel", 2.5f);
    }

    public void GameOver ()
    {
        player.gameObject.SetActive (false);
        EnemiesManager.Instance.StopEnemies ();
        gameIsActive = false;
        LoseLives (lives);
        print ("Game Over");
    }

    private void Update ()
    {
        if (Input.GetKey (KeyCode.Mouse0) && gameIsActive) player.Disparar ();
        if (Input.GetKeyDown (KeyCode.Mouse0) && lives == 0) SceneManager.LoadScene (0);
        if (Input.GetKeyDown (KeyCode.Mouse1)) EnemiesManager.Instance.MakeCrazyEnemy ();
    }

    public void RestartLevel ()
    {
        player.gameObject.SetActive (true);
        EnemiesManager.Instance.Reset ();
    }

    public void LoseLives (int livesLost)
    {
        lives -= livesLost;
        CanvasManager.Instance.SetLivesNumber (lives);
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
