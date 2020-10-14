using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;

    float animationSpeed = 1;

    public GameObject enemyPrefab;
    public GameObject explosionPrefab;
    public Transform enemiesT;
    Vector3 defaultPosEnemies;
    Animator enemiesAnimator;
    List<Enemy> enemiesList = new List<Enemy> ();
    List<Transform> levels = new List<Transform> ();
    EZObjectPool explosionsPool;

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

    void Reset ()
    {
        forwardSteps = 0;
        enemiesT.parent.position = Vector3.zero;
        enemiesT.localPosition = defaultPosEnemies;
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
                levelPrefab = GameManager.Instance.levelsTables.prefabLevel1;
                break;
            case 2:
                levelPrefab = GameManager.Instance.levelsTables.prefabLevel2;
                break;
            case 3:
                levelPrefab = GameManager.Instance.levelsTables.prefabLevel3;
                break;
            case 4:
                levelPrefab = GameManager.Instance.levelsTables.prefabLevel4;
                break;
            default:
                levelPrefab = GameManager.Instance.levelsTables.prefabLevel5;
                break;
        }

        return levelPrefab;
    }

    public void StepForward ()
    {
        StartCoroutine (MoveForwardEnemies ());
    }

    IEnumerator MoveForwardEnemies ()
    {
        forwardSteps++;
        float v = 0f;
        Vector3 targetPos = Vector3.down * forwardSteps / 4f;
        while (v < 1f)
        {
            transform.position = Vector3.Lerp (transform.position, targetPos, v);
            v += Time.deltaTime * animationSpeed;
            yield return new WaitForEndOfFrame ();
        }
    }

    public void StopEnemies ()
    {
        enemiesAnimator.speed = 0;
        StopAllCoroutines ();
    }

    public void MakeCrazyEnemy ()
    {
        var lista = enemiesList.FindAll (enemy => enemy.alive == true);
        lista [Random.Range (0, lista.Count)].ChasePlayer ();
    }

    public void SetAnimationSpeed (float speed)
    {
        animationSpeed = speed;
        enemiesAnimator.speed = animationSpeed;
    }

    public void EnemyDestroyed (Enemy enemy)
    {
        if (explosionsPool.TryGetNextObject (enemy.transform.position, Quaternion.identity, out GameObject explosion))
        {
            StartCoroutine (DesactivateOnTime (explosion, 0.06f));
        }
        enemiesleft--;
        if (enemiesleft % 20 == 0) SetAnimationSpeed (animationSpeed + 1);
        if (enemiesleft == 0) GameManager.Instance.LevelCompleted ();
    }

    public IEnumerator DesactivateOnTime (GameObject go, float time)
    {
        yield return new WaitForSeconds (time);
        go.SetActive (false);
    }

}
