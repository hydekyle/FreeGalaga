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
        GameManager.Instance.LoseLives (1);
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

    void ReloadEnemies ()
    {
        var levelNumber = GameManager.Instance.activeLevelNumber;
        var go = Instantiate (GetLevelPrefab (levelNumber), Vector3.zero, Quaternion.identity, enemiesT);
        var kills = go.transform.childCount - enemiesleft;
        kills++;
        for (var x = 0; x < kills; x++) go.transform.GetChild (x).gameObject.SetActive (false); // Remove killed ones
        go.transform.localPosition = Vector3.zero;
        enemiesAnimator.Play ("Enemies");
        enemiesAnimator.enabled = true;
        SetAnimationSpeed (animationSpeed);
        GameManager.Instance.gameIsActive = true;
        List<Enemy> newEnemiesList = new List<Enemy> ();
        foreach (Transform t in go.transform)
        {
            if (t.gameObject.activeSelf) newEnemiesList.Add (t.GetComponent<Enemy> ());
        }
        enemiesList = newEnemiesList;
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
        var lista = enemiesList.FindAll (e => e.alive == true);
        if (lista.Count == 0) return;

        var enemy = lista [Random.Range (0, lista.Count)];
        enemy.SetBehavior (enemy.defaultBehavior);
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

}
