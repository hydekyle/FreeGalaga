using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using EZObjectPools;
using UnityEngine;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject settings;
    public Player player;
    public Material playerBulletMaterial;
    public bool gameIsActive = false;
    public int activeLevelNumber = 0;
    public ScriptableEtc tablesEtc;
    public ScriptableLevels tablesLevels;
    public ScriptableSounds tablesSounds;
    public int lives = 3;
    public GameObject bulletEnemyPrefab, bombPrefab;
    public GameObject boostShield, boostHealth, boostPoints, boostAttackspeed;
    public bool retryAvailable = false;

    [DllImport("__Internal")]
    public static extern void ShareScore(int puesto, int puntos);

    [DllImport("__Internal")]
    public static extern void Information();

    [DllImport("__Internal")]
    public static extern void Gameover();

    [HideInInspector]
    public EZObjectPool enemyBulletsPoolGreen, enemyBulletsPoolRed, enemyBulletsPoolFire, enemyBombs;

    public float minPosX = -3.8f, maxPosX = 3.8f, minPosY = -4.5f, maxPosY = -2f;
    public GameObject bigExplosion;

    public GameData gameData = new GameData();
    public GameConfig gameConfig = new GameConfig();

    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        enemyBulletsPoolGreen = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[0], "Bullets Enemy Green", 4, false, true, true);
        enemyBulletsPoolRed = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[1], "Bullets Enemy Red", 5, false, true, true);
        enemyBulletsPoolFire = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[2], "Bullets Enemy Fire", 4, false, true, true);
        enemyBombs = EZObjectPool.CreateObjectPool(bombPrefab, "Bombs Boss", 6, false, true, true);
    }

    private void Start()
    {
        //CheckGameLegality ();
        LoadGameConfig();
    }

    // private void CheckGameLegality ()
    // {
    //     StartCoroutine (NetworkManager.CheckLegality (isLegal =>
    //     {
    //         if (isLegal) LoadGameConfig ();
    //     }));
    // }

    private void LoadGameConfig()
    {
        if (!Application.isEditor)
        {
            var baseURL = Application.absoluteURL;
            gameData.getHighScoresURL = baseURL + "scores.php";
            gameData.getUserDataURL = baseURL + "userdata.php";
            gameData.sendScoreURL = baseURL + "updatescore.php";
            gameData.consumeIntentosURL = baseURL + "consumeintentos.php";
            gameData.getStoriesURL = baseURL + "textos.php";
        }
        else // JUST FOR UNITY EDITOR
        {
            gameData.getHighScoresURL = "https://hydekyle.ga/cyber_defense/scores.php";
            gameData.sendScoreURL = "https://hydekyle.ga/cyber_defense/updatescore.php";
            gameData.getUserDataURL = "https://hydekyle.ga/cyber_defense/userdata.php";
            gameData.consumeIntentosURL = "https://hydekyle.ga/cyber_defense/consumeintentos.php";
            gameData.getStoriesURL = "https://hydekyle.ga/cyber_defense/textos.php";
        }
        gameConfig = new GameConfig() // Cambiar esto si se desea cargar configuración adicional desde fuera del editor
        {
            lives_per_credit = 3
        };
        GetUserData();
    }

    public string alias = "";
    public int intentos;

    private void GetUserData()
    {
        if (!Application.isEditor) alias = HttpCookie.GetCookie("ALIAS");

        if (alias != "")
        {
            StartCoroutine(NetworkManager.GetUserData(alias, userData =>
          {
              gameData.userAlias = userData.alias;
              intentos = int.Parse(userData.intentos);
              CanvasManager.Instance.LoadUserData(userData);
          }));
        }
        else
        {
            Debug.LogWarning("No ALIAS founded!");
        }
    }

    public void StartGame()
    {
        player.GetComponent<SpriteRenderer>().enabled = true;
        SetLives(gameConfig.lives_per_credit);
        if (!Application.isEditor) WebGLInput.captureAllKeyboardInput = true;
        CanvasManager.Instance.usernameText.text = gameData.userAlias;
        AudioManager.Instance.StartMusic();
        LoadLevel(++activeLevelNumber);
    }

    public void SetAndroidControls(bool status)
    {
        CanvasManager.Instance.androidControls.gameObject.SetActive(status);
    }

    public void AddRandomPowerUps(List<Enemy> enemyList)
    {
        enemyList.Sort((a, b) => 1 - 2 * Random.Range(0, 2));
        enemyList[0].powerUp = BoostType.Shield;
        enemyList[1].powerUp = BoostType.AttackSpeed;
        enemyList[2].powerUp = BoostType.Points;
    }

    float lastTimePowerUpDropped;
    public void DropPowerUp(Vector3 dropPosition, BoostType boostType)
    {
        if (boostType == BoostType.None) return;
        bool hacePoco = lastTimePowerUpDropped + 1.3f > Time.time;
        float gravity = hacePoco ? 0.1f : 0.25f;
        gravity += Random.Range(0f, 0.2f);
        GameObject powerUp;
        switch (boostType)
        {
            case BoostType.Points:
                powerUp = Instantiate(boostPoints, dropPosition, Quaternion.identity);
                powerUp.GetComponent<Rigidbody2D>().gravityScale = gravity;
                break;
            case BoostType.Health:
                powerUp = Instantiate(boostHealth, dropPosition, Quaternion.identity);
                powerUp.GetComponent<Rigidbody2D>().gravityScale = gravity;
                break;
            case BoostType.AttackSpeed:
                powerUp = Instantiate(boostAttackspeed, dropPosition, Quaternion.identity);
                powerUp.GetComponent<Rigidbody2D>().gravityScale = gravity;
                break;
            case BoostType.Shield:
                powerUp = Instantiate(boostShield, dropPosition, Quaternion.identity);
                powerUp.GetComponent<Rigidbody2D>().gravityScale = gravity;
                break;
        }

        lastTimePowerUpDropped = Time.time;
    }

    public void ExplosionBigAtPosition(Vector3 explosionPosition, Color explosionColor)
    {
        AudioManager.Instance.PlayAudioEnemy(tablesSounds.explosionBig);
        var explosion = Instantiate(bigExplosion, explosionPosition, Quaternion.identity);
        explosion.GetComponent<SpriteRenderer>().color = explosionColor;
    }

    public void FinalBossKilled(Vector3 explosionPosition)
    {
        StartCoroutine(FinalBossExplosionRoutine(explosionPosition));
    }

    IEnumerator FinalBossExplosionRoutine(Vector3 explosionPosition)
    {
        AudioManager.Instance.PlayAudioEnemy(tablesSounds.explosionBig);
        Instantiate(bigExplosion, explosionPosition, Quaternion.identity);
        DropPowerUp(explosionPosition, BoostType.Points);
        yield return new WaitForSeconds(0.3f);
        for (var x = 0; x < 10; x++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            AudioManager.Instance.PlayAudioEnemy(tablesSounds.explosionBig);
            Instantiate(bigExplosion, explosionPosition + randomPos, Quaternion.identity);
            DropPowerUp(explosionPosition, BoostType.Points);
            yield return new WaitForSeconds(0.3f);
        }
        if (lives > 0) GameFinished();
    }

    public void LoadNextLevel()
    {
        if (lives == 0) return;
        LoadLevel(++activeLevelNumber);
    }

    void LoadLevel(int levelNumber)
    {
        Sprite background;
        float animationSpeed;
        Color colorTextUI;

        switch (levelNumber)
        {
            case 1:
                background = tablesLevels.backgroundLevel1;
                animationSpeed = tablesLevels.animSpeed1;
                colorTextUI = Color.black;
                break;
            case 2:
                background = tablesLevels.backgroundLevel2;
                animationSpeed = tablesLevels.animSpeed2;
                colorTextUI = Color.white;
                break;
            case 3:
                background = tablesLevels.backgroundLevel3;
                animationSpeed = tablesLevels.animSpeed3;
                colorTextUI = Color.black;
                break;
            case 4:
                background = tablesLevels.backgroundLevel3;
                animationSpeed = tablesLevels.animSpeed3;
                colorTextUI = Color.white;
                break;
            default:
                background = tablesLevels.backgroundLevelFinal;
                animationSpeed = tablesLevels.animSpeedFinal;
                colorTextUI = Color.yellow;
                break;
        }
        EnemiesManager.Instance.LoadEnemies();

        CanvasManager.Instance.SetBackground(background);
        CanvasManager.Instance.SetLevelNumber(levelNumber);
        CanvasManager.Instance.SetColorTextUI(colorTextUI);
        CanvasManager.Instance.SetBarColor(GetColorByLevel(levelNumber));
    }

    public bool showStories = true;

    public void LevelCompleted()
    {
        gameIsActive = false;
        EnemiesManager.Instance.ClearAllEnemies();
        Invoke("PlayerLevelUp", 1.2f);
        Invoke("PrepareNextLevel", 2.6f);

        player.lastTimeAttackBoosted += 2.6f; // Para evitar desperdiciar el power-up entre escenas
        player.shield.nextTimeShutDownShield += 2.6f;
    }

    void PrepareNextLevel()
    {
        if (showStories) ShowStory();
        else CanvasManager.Instance.BTN_StoryOK();
    }

    void ShowStory()
    {
        CanvasManager.Instance.ShowStory(activeLevelNumber);
    }

    public void PlayerLevelUp()
    {
        player.LevelUp();
    }

    public void GameOver(bool playMusic)
    {
        if (playMusic)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayAudioPlayer(AudioManager.Instance.scriptableSounds.gameOverSFX);
        }

        player.gameObject.SetActive(false);
        EnemiesManager.Instance.StopEnemies();
        gameIsActive = false;
        CanvasManager.Instance.SendScore(gameData.userAlias, CanvasManager.Instance.score);
    }

    private void Update()
    {
        Controles();
    }

    public void RestartLevel()
    {
        player.gameObject.SetActive(true);
        EnemiesManager.Instance.Reset();
    }

    void SetLives(int livesAmount)
    {
        CanvasManager.Instance.SetLivesNumber(livesAmount);
        lives = livesAmount;
    }

    public void LoseLives(int livesLost, float inmuneTime)
    {
        AudioManager.Instance.PlayPlayerDestroyed();
        lives -= livesLost;
        CanvasManager.Instance.SetLivesNumber(lives);
        if (lives > 0)
        {
            StartCoroutine(player.InmuneTime(inmuneTime));
        }
        else GameOver(true);
    }

    public void GainLives(int newLives)
    {
        lives += newLives;
        CanvasManager.Instance.SetLivesNumber(lives);
    }

    public int GetLevelNumber()
    {
        return activeLevelNumber;
    }

    Color GetColorByLevel(int levelNumber)
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

    public void DesactivateOnTime(GameObject go, float time)
    {
        StartCoroutine(_DesactivateOnTime(go, time));
    }

    IEnumerator _DesactivateOnTime(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(false);
    }

    public void GameFinished()
    {
        AddScoreByTime();
        Invoke("EndGame", 6.6f);
    }

    void AddScoreByTime()
    {
        var score = (int)(300f - Time.timeSinceLevelLoad);
        CanvasManager.Instance.AddScore(score);
    }

    IEnumerator ChangeLivesByPoints()
    {
        var totalLives = lives;
        yield return new WaitForSeconds(3f);
        for (var x = 0; x < totalLives; x++)
        {
            AudioManager.Instance.PlayAudioPlayer(tablesSounds.lifeUp);
            CanvasManager.Instance.SetLivesNumber(--lives);
            CanvasManager.Instance.AddScore(1000);
            yield return new WaitForSeconds(1f);
        }
        GameOver(false);
    }

    void EndGame()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayAudioPlayer(AudioManager.Instance.scriptableSounds.theWin);
        gameIsActive = false;
        StartCoroutine(ChangeLivesByPoints());
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void Controles()
    {
        if (Input.GetButtonDown("Reset")) ReloadScene();

        if (Input.GetButton("Shoot") || Input.GetButton("ShootPad"))
        {
            if (gameIsActive && lives > 0) player.Shoot();
        }
    }

}
