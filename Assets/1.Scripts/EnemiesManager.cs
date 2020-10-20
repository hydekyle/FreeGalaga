using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;

    public float animationSpeed = 1;

    public GameObject enemyPrefab;
    public GameObject explosionPrefab;
    public Transform enemiesT;
    Vector3 defaultPosEnemies;
    Animator enemiesAnimator;
    List<Enemy> enemiesList = new List<Enemy> ();
    EZObjectPool explosionsPool;
    public AudioSource enemiesAudio;

    int forwardSteps = 0; // Veces que los enemigos han avanzado
    int enemiesleft = 0;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        Initialize ();
    }

    public void AddEnemy (Enemy enemy)
    {
        enemiesList.Add (enemy);
    }

    public void ReloadShootCooldown ()
    {
        foreach (var enemy in enemiesList) enemy?.GetShootOnCooldown ();
    }

    void Initialize ()
    {
        defaultPosEnemies = enemiesT.localPosition;
        enemiesAnimator = enemiesT.parent.GetComponent<Animator> ();
        explosionsPool = EZObjectPool.CreateObjectPool (explosionPrefab, "Explosiones", 4, true, true, true);
    }

    public void Reset ()
    {
        forwardSteps = 0;
        enemiesT.parent.position = Vector3.zero;
        enemiesT.localPosition = defaultPosEnemies;
        SetAnimationSpeed (GetLevelSpeed (GameManager.Instance.activeLevelNumber));
    }

    public void EnemyTouchBottom ()
    {
        RestartPositions ();
        GameManager.Instance.LoseLives (1, 0.5f);
    }

    public void RestartPositions ()
    {
        StopEnemies ();
        Reset ();
        ReloadEnemies ();
    }

    public void ClearAllEnemies ()
    {
        if (enemiesT.childCount > 0)
            foreach (Transform t in enemiesT) Destroy (t.gameObject); // Destruye cualquier nivel activo

        foreach (Enemy enemy in enemiesList) Destroy (enemy.gameObject); // Destruye cada enemigo
    }

    public void LoadEnemies ()
    {
        StopEnemies ();
        Reset ();
        LoadEnemies (GameManager.Instance.activeLevelNumber);
    }

    void LoadEnemies (int levelNumber)
    {
        var go = Instantiate (GetLevelPrefab (levelNumber), Vector3.zero, Quaternion.identity, enemiesT);
        go.transform.localPosition = Vector3.zero;
        enemiesleft = go.transform.childCount;
        enemiesAnimator.Play (GetAnimationName ());
        enemiesAnimator.enabled = true;
        SetAnimationSpeed (animationSpeed);
        List<Enemy> newEnemiesList = new List<Enemy> ();
        foreach (Transform t in go.transform)
        {
            if (t.TryGetComponent (out Enemy newEnemy)) newEnemiesList.Add (newEnemy);
        }
        enemiesleft += newEnemiesList.FindAll (e => e.defaultBehavior == EnemyBehavior.Leader).Count * 4;
        enemiesList = newEnemiesList;
        GameManager.Instance.gameIsActive = true;
    }

    void ReloadEnemies ()
    {
        List<Enemy> newEnemies = new List<Enemy> ();
        var enemiesAlive = enemiesList.FindAll (enemy => enemy.alive);
        print (enemiesAlive.Count);

        var levelNumber = GameManager.Instance.activeLevelNumber;
        var go = Instantiate (GetLevelPrefab (levelNumber), Vector3.zero, Quaternion.identity);

        foreach (Transform t in go.transform) t.gameObject.SetActive (false); // Desactivar todos del nuevo GO

        foreach (Enemy enemy in enemiesAlive)
        {
            if (enemy.ID == "4C") return;
            var newEnemyT = go.transform.Find (enemy.transform.name);
            newEnemyT.gameObject.SetActive (true); // Activar sólo los que quedaron vivos
            newEnemies.Add (newEnemyT.GetComponent<Enemy> ());
        }

        ClearAllEnemies ();

        go.transform.SetParent (enemiesT);

        enemiesList = newEnemies;

        go.transform.localPosition = Vector3.zero;
        enemiesAnimator.Play (GetAnimationName ());
        enemiesAnimator.enabled = true;
        SetAnimationSpeed (animationSpeed);
        GameManager.Instance.gameIsActive = true;
    }

    GameObject GetLevelPrefab (int levelNumber)
    {
        GameObject levelPrefab;
        switch (levelNumber)
        {
            case 1:
                levelPrefab = GameManager.Instance.tablesLevels.prefabLevel1;
                break;
            case 2:
                levelPrefab = GameManager.Instance.tablesLevels.prefabLevel2;
                break;
            case 3:
                levelPrefab = GameManager.Instance.tablesLevels.prefabLevel3;
                break;
            case 4:
                levelPrefab = GameManager.Instance.tablesLevels.prefabLevel4;
                break;
            default:
                levelPrefab = GameManager.Instance.tablesLevels.prefabLevelFinal;
                break;
        }

        return levelPrefab;
    }

    float GetLevelSpeed (int levelNumber)
    {
        float value;
        switch (levelNumber)
        {
            case 1:
                value = GameManager.Instance.tablesLevels.animSpeed1;
                break;
            case 2:
                value = GameManager.Instance.tablesLevels.animSpeed2;
                break;
            case 3:
                value = GameManager.Instance.tablesLevels.animSpeed3;
                break;
            case 4:
                value = GameManager.Instance.tablesLevels.animSpeed4;
                break;
            default:
                value = GameManager.Instance.tablesLevels.animSpeedFinal;
                break;
        }
        return value;
    }

    public void StepForward ()
    {
        StartCoroutine (MoveForwardEnemies ());
    }

    IEnumerator MoveForwardEnemies ()
    {
        float actualSpeed = animationSpeed;
        enemiesAnimator.enabled = false;
        forwardSteps++;
        float v = 0f;
        Vector3 targetPos = Vector3.down * forwardSteps / 3f;
        while (v < 1f)
        {
            transform.position = Vector3.Lerp (transform.position, targetPos, v / 10);
            v += Time.deltaTime * actualSpeed;
            yield return new WaitForEndOfFrame ();
        }
        transform.position = targetPos;
        enemiesAnimator.enabled = true;
    }

    public void StopEnemies ()
    {
        enemiesAnimator.speed = 0;
        StopAllCoroutines ();
    }

    int crazyEnemiesTotal = 0;
    public void MakeCrazyEnemy ()
    {
        var lista = enemiesList.FindAll (e => e.alive == true);
        lista = lista.FindAll (e => e.activeBehavior == EnemyBehavior.None);
        if (lista.Count == 0) return;

        var enemy = lista [Random.Range (0, lista.Count)];
        enemy.SetBehavior (enemy.defaultBehavior);

        // Esta parte solo es para el nivel 4
        if (GameManager.Instance.activeLevelNumber == 4 && crazyEnemiesTotal % 2 == 0)
        {
            var laLista = enemiesList.FindAll (leader => leader.defaultBehavior == EnemyBehavior.Leader)?.FindAll (alive => alive.alive)?.FindAll (isShooter => isShooter.activeBehavior == EnemyBehavior.Shooter);
            if (laLista.Count > 0) laLista [Random.Range (0, laLista.Count)].SetBehavior (EnemyBehavior.Leader);
        }
        crazyEnemiesTotal++;
    }

    public void SetAnimationSpeed (float speed)
    {
        animationSpeed = speed;
        enemiesAnimator.speed = animationSpeed;
    }

    bool IsFinalLevel ()
    {
        return GameManager.Instance.activeLevelNumber == 5;
    }

    public void EnemyDestroyed (Enemy enemy)
    {
        if (explosionsPool.TryGetNextObject (enemy.transform.position, Quaternion.identity, out GameObject explosionGO))
        {
            GameManager.Instance.DesactivateOnTime (explosionGO, 0.06f);
        }
        AudioManager.Instance.PlayAudioClip (GameManager.Instance.tablesSounds.explosionLow);

        if (!IsFinalLevel ())
        {
            enemiesleft--;
            if (enemiesleft % 3 == 0) MakeCrazyEnemy ();
            if (enemiesleft % 10 == 0) SetAnimationSpeed (animationSpeed + 0.5f);
            if (enemiesleft == 0) GameManager.Instance.LevelCompleted ();
            else if (enemiesleft == 1) SetAnimationSpeed (animationSpeed * 2);
        }
    }

    string GetAnimationName ()
    {
        var level = GameManager.Instance.activeLevelNumber;
        string animName = level != 3 ? "Enemies" : "EnemiesVariant";
        return animName;
    }

}
