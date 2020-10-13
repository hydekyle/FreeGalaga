using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;

    public int rows = 6, columns = 10;
    public float animationSpeed = 1;

    public GameObject enemyPrefab;
    public Transform enemiesTransformParent;
    Animator enemiesAnimator;
    List<Enemy> enemiesList = new List<Enemy> ();

    int forwardSteps = 0; // Veces que los enemigos han avanzado

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        enemiesAnimator = enemiesTransformParent.parent.GetComponent<Animator> ();
        StartCoroutine (SpawnEnemies ());
    }

    IEnumerator SpawnEnemies ()
    {
        for (var x = 0; x < columns; x++)
        {
            for (var y = 0; y < rows; y++)
            {
                var go = GameObject.Instantiate (enemyPrefab, Vector3.zero, Quaternion.Euler (0, 0, -90), enemiesTransformParent);
                go.transform.localPosition = new Vector3 (x / 1.5f, y / 1.5f, 0);
                enemiesList.Add (go.GetComponent<Enemy> ());
                yield return new WaitForSeconds (0.01f);
            }
        }
        enemiesAnimator.Play ("Enemies");
        enemiesAnimator.speed = animationSpeed;
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

}
