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
    List<Transform> levels = new List<Transform> ();
    EZObjectPool explosionsPool;
    public AudioSource enemiesAudio;

    public ScriptableEnemies enemiesTable;

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

    void Initialize ()
    {
        defaultPosEnemies = enemiesT.localPosition;
        enemiesAnimator = enemiesT.parent.GetComponent<Animator> ();
        explosionsPool = EZObjectPool.CreateObjectPool (explosionPrefab, "Explosiones", 4, true, true, true);
        foreach (Transform t in enemiesT) levels.Add (t);
    }

    Transform GetLevelTransform (int levelNumber)
    {
        return levels [levelNumber - 1];
    }

    public void Reset ()
    {
        forwardSteps = 0;
        enemiesT.parent.position = Vector3.zero;
        enemiesT.localPosition = defaultPosEnemies;
        SetAnimationSpeed (GetLevelSpeed (GameManager.Instance.activeLevelNumber));
    }

    public void ClearAllEnemies ()
    {
        if (enemiesT.childCount > 0)
            foreach (Transform t in enemiesT) Destroy (t.gameObject);

        foreach (Enemy enemy in enemiesList) Destroy (enemy.gameObject);
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
        enemiesAnimator.Play ("Enemies");
        enemiesAnimator.enabled = true;
        SetAnimationSpeed (animationSpeed);
        GameManager.Instance.gameIsActive = true;
        List<Enemy> newEnemiesList = new List<Enemy> ();
        foreach (Transform t in go.transform) newEnemiesList.Add (t.GetComponent<Enemy> ());
        enemiesList = newEnemiesList;

    }

    private EnemyModel GetEnemyModel (int levelNumber)
    {
        EnemyModel enemyModel;
        switch (levelNumber)
        {
            case 1:
                enemyModel = enemiesTable.level1 [Random.Range (0, enemiesTable.level1.Count)];
                break;
            case 2:
                enemyModel = enemiesTable.level2 [Random.Range (0, enemiesTable.level1.Count)];
                break;
            default:
                enemyModel = enemiesTable.level1 [Random.Range (0, enemiesTable.level1.Count)];
                break;
        }

        return enemyModel;
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

    public void MakeCrazyEnemy ()
    {
        var lista = enemiesList.FindAll (enemy => enemy.alive == true);
        if (lista.Count == 0) return;
        lista [Random.Range (0, lista.Count)].ChasePlayer ();
    }

    public void SetAnimationSpeed (float speed)
    {
        animationSpeed = speed;
        enemiesAnimator.speed = animationSpeed;
    }

    public void EnemyDestroyed (Enemy enemy)
    {
        if (explosionsPool.TryGetNextObject (enemy.transform.position, Quaternion.identity, out GameObject explosionGO))
        {
            GameManager.Instance.DesactivateOnTime (explosionGO, 0.06f);
        }
        AudioManager.Instance.PlayAudioClip (GameManager.Instance.tablesSounds.explosionLow);
        enemiesleft--;
        if (enemiesleft % 15 == 0) SetAnimationSpeed (animationSpeed + 1);
        if (enemiesleft == 0) GameManager.Instance.LevelCompleted ();
        else if (enemiesleft == 1) SetAnimationSpeed (animationSpeed * 2);
    }

}
