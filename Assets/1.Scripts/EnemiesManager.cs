using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;

    public int rows = 6, columns = 10;
    float animationSpeed = 1;

    public GameObject enemyPrefab;
    public Transform enemiesT;
    Vector3 defaultPosEnemies;
    Animator enemiesAnimator;
    List<Enemy> enemiesList = new List<Enemy> ();

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
        defaultPosEnemies = enemiesT.localPosition;
        enemiesAnimator = enemiesT.parent.GetComponent<Animator> ();
    }

    public void LoadEnemies ()
    {
        StopEnemies ();
        forwardSteps = 0;
        enemiesT.parent.position = Vector3.zero;
        enemiesT.localPosition = defaultPosEnemies;
        SpawnEnemies (GameManager.Instance.activeLevelNumber);
    }

    void SpawnEnemies (int levelNumber)
    {
        StartCoroutine (SpawnEnemies ());
    }

    IEnumerator SpawnEnemies ()
    {
        foreach (Transform t in enemiesT) Destroy (t.gameObject);

        for (var x = 0; x < columns; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                var enemyModel = GetEnemyModel (GameManager.Instance.GetLevelNumber ());
                var go = GameObject.Instantiate (enemyPrefab, Vector3.zero, Quaternion.Euler (0, 0, -90), enemiesT);
                go.GetComponent<SpriteRenderer> ().sprite = enemyModel.sprite;
                go.transform.localPosition = new Vector3 (x / 1.5f, y / 1.5f, 0);
                Enemy enemy = go.GetComponent<Enemy> ();
                enemy.stats = enemyModel.stats;
                go.SetActive (true);
                enemiesList.Add (enemy);
                enemiesleft++;
                yield return new WaitForSeconds (0.03f);
            }
        }
        enemiesAnimator.Play ("Enemies");
        enemiesAnimator.speed = animationSpeed;
        GameManager.Instance.gameIsActive = true;
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
        enemiesAnimator.Play ("Idle");
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
        enemiesleft--;
        if (enemiesleft % 20 == 0) SetAnimationSpeed (animationSpeed + 1);
        if (enemiesleft == 0) GameManager.Instance.LevelCompleted ();
    }

}
