using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using EZObjectPools;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public bool gameIsActive = false;
    public int activeLevelNumber = 0;
    public ScriptableEtc tablesEtc;
    public ScriptableLevels tablesLevels;
    public ScriptableSounds tablesSounds;
    public int lives = 3;

    public GameObject bulletEnemyPrefab;
    [HideInInspector]
    public EZObjectPool enemyBulletsPoolGreen, enemyBulletsPoolRed, enemyBulletsPoolFire;

    public float minPosX = -3.8f, maxPosX = 3.8f, minPosY = -4.5f, maxPosY = -2f;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        Initialize ();
        StartGame ();
    }

    void Initialize ()
    {
        enemyBulletsPoolGreen = EZObjectPool.CreateObjectPool (tablesEtc.disparosEnemigos [0], "Bullets Enemy Green", 9, false, true, true);
        enemyBulletsPoolRed = EZObjectPool.CreateObjectPool (tablesEtc.disparosEnemigos [1], "Bullets Enemy Red", 9, false, true, true);
        enemyBulletsPoolFire = EZObjectPool.CreateObjectPool (tablesEtc.disparosEnemigos [2], "Bullets Enemy Fire", 9, false, true, true);
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
            case 1:
                background = tablesLevels.backgroundLevel1;
                animationSpeed = tablesLevels.animSpeed1;
                break;
            case 2:
                background = tablesLevels.backgroundLevel2;
                animationSpeed = tablesLevels.animSpeed2;
                break;
            case 3:
                background = tablesLevels.backgroundLevel3;
                animationSpeed = tablesLevels.animSpeed3;
                break;
            default:
                background = tablesLevels.backgroundLevelFinal;
                animationSpeed = tablesLevels.animSpeedFinal;
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
        Invoke ("PlayerLevelUp", 1.2f);
        Invoke ("LoadNextLevel", 2.6f);
    }

    public void PlayerLevelUp ()
    {
        player.LevelUp ();
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
        if (Input.GetKeyDown (KeyCode.Mouse0) && lives == 0) SceneManager.LoadScene (0);
        //if (Input.GetKeyDown (KeyCode.Mouse1)) EnemiesManager.Instance.MakeCrazyEnemy ();
    }

    public void RestartLevel ()
    {
        player.gameObject.SetActive (true);
        EnemiesManager.Instance.Reset ();
    }

    public void LoseLives (int livesLost, float inmuneTime)
    {
        AudioManager.Instance.PlayAudioClip (GameManager.Instance.tablesSounds.playerDestroyed);
        lives -= livesLost;
        CanvasManager.Instance.SetLivesNumber (lives);
        if (lives > 0)
        {
            player.myCollider.enabled = false;
            StartCoroutine (player.InmuneTime (inmuneTime));
        }
        else GameOver ();
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
                color = tablesLevels.barColorLevel1;
                break;
            case 2:
                color = tablesLevels.barColorLevel2;
                break;
            case 3:
                color = tablesLevels.barColorLevel3;
                break;
            case 4:
                color = tablesLevels.barColorLevel4;
                break;
            default:
                color = tablesLevels.barColorLevelFinal;
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
