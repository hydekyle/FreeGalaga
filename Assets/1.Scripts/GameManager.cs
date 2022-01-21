using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using EZObjectPools;
using UnityEngine;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void ShareScore(int puesto, int puntos);
    [DllImport("__Internal")]
    public static extern void Information();
    [DllImport("__Internal")]
    public static extern void Gameover();

    public static GameManager Instance;
    public GameObject settings;
    public Player player;
    public Material playerBulletMaterial;
    public int activeLevelNumber = 0;
    public ScriptableEtc tablesEtc;
    public ScriptableLevels tablesLevels;
    public ScriptableSounds tablesSounds;
    public int lives = 3;
    public GameObject bulletEnemyPrefab, bombPrefab;
    public GameObject boostShield, boostHealth, boostPoints, boostAttackspeed;
    public GameConfiguration gameConfiguration;
    public string serverURL;

    [Header("SETTINGS")]
    public GameObject bigExplosion;
    public float minPosX = -3.8f, maxPosX = 3.8f, minPosY = -4.5f, maxPosY = -2f;

    [HideInInspector]
    public EZObjectPool enemyBulletsPoolGreen, enemyBulletsPoolRed, enemyBulletsPoolFire, enemyBombs;
    [HideInInspector]
    public bool retryAvailable = false;
    [HideInInspector]
    public bool gameIsActive = false;
    [HideInInspector]
    public GameServer gameServer;
    public User user;
    float lastTimePowerUpDropped;

    void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
        enemyBulletsPoolGreen = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[0], "Bullets Enemy Green", 4, false, true, true);
        enemyBulletsPoolRed = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[1], "Bullets Enemy Red", 5, false, true, true);
        enemyBulletsPoolFire = EZObjectPool.CreateObjectPool(tablesEtc.disparosEnemigos[2], "Bullets Enemy Fire", 4, false, true, true);
        enemyBombs = EZObjectPool.CreateObjectPool(bombPrefab, "Bombs Boss", 6, false, true, true);
        // Get a unique ID if dont have one
        if (!PlayerPrefs.HasKey("id"))
        {
            var newID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("id", newID);
        }
    }

    async void Start()
    {
        gameServer = Helpers.GetGameServer();
        var serverData = await NetworkManager.GetGameDataByUserID(PlayerPrefs.GetString("id"));
        user = serverData.userData;
        gameConfiguration = serverData.gameConfig;
        CanvasManager.Instance.ShowStartMenu(user);
    }

    public void StartGame()
    {
        if (!Application.isEditor) WebGLInput.captureAllKeyboardInput = true;
        AudioManager.Instance.PlayMainThemeMusic();
        LoadLevel(++activeLevelNumber);
        SpawnPlayer();
        Settings.Instance.TouchpadEnabled();
    }

    void SpawnPlayer()
    {
        SetLives(gameConfiguration.livesPerCredit);
        player.stats.movementVelocity = GameManager.Instance.gameConfiguration.playerMovementSpeed / 4f;
        player.stats.shootCooldown = GameManager.Instance.gameConfiguration.playerAttackSpeed / 4f;
        player.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void AddRandomPowerUps(List<Enemy> enemyList)
    {
        enemyList.Sort((a, b) => 1 - 2 * Random.Range(0, 2));
        enemyList[0].powerUp = BoostType.Shield;
        enemyList[1].powerUp = BoostType.AttackSpeed;
        enemyList[2].powerUp = BoostType.Points;
    }

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
        player.transform.position = new Vector3(0, -4, 0);
        player.lastTimeShot = Time.time;
        Time.timeScale = 1f;
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
        Invoke("LoadNextLevel", 2.6f);

        player.lastTimeAttackBoosted += 2.6f; // Para evitar desperdiciar el power-up entre escenas
        player.shield.nextTimeShutDownShield += 2.6f;
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
        SendScore();
    }

    public async void SendScore()
    {
        var score = CanvasManager.Instance.score;
        GameSession.Instance.GameFinished();
        GameManager.Instance.SaveScore(score);
        var updateResponse = await NetworkManager.UpdateUserDataAndGetTopScore();
        CanvasManager.Instance.ShowHighScores(updateResponse.users);
    }

    public void SaveScore(int score)
    {
        user.score = score;
        if (PlayerPrefs.HasKey("score"))
        {
            var lastScore = PlayerPrefs.GetInt("score");
            if (lastScore < score) PlayerPrefs.SetInt("score", score);
        }
        else
        {
            PlayerPrefs.SetInt("score", score);
        }
    }

    public void SetLives(int livesAmount)
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

    public void ResetGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

}
