using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    public Transform boss;
    public List<Enemy> minions = new List<Enemy> ();
    public Stats stats;
    public Vector3 leftPos, rightPos;

    bool vulnerable = false;
    float startPingPongTime;

    private void FixedUpdate ()
    {
        if (!vulnerable && IsAllMinionsDead ()) LoseVulnerability ();
    }

    private void Update ()
    {
        if (vulnerable) transform.position = Vector3.Lerp (transform.position, Vector3.zero, Time.deltaTime * stats.movementVelocity);
    }

    bool IsAllMinionsDead ()
    {
        foreach (var enemy in minions)
        {
            if (enemy.alive) return false;
        }
        return true;
    }

    void LoseVulnerability ()
    {
        vulnerable = true;
        EnemiesManager.Instance.StopEnemies ();
        GetComponent<Animator> ().StopPlayback ();
    }

    void Die ()
    {
        CanvasManager.Instance.AddScore (1000);
        boss.gameObject.SetActive (true);
        StopAllCoroutines ();
        gameObject.SetActive (false);
    }

    public void GetStrike (int strikeDamage)
    {
        stats.health -= strikeDamage;
        if (stats.health <= 0) Die ();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("PlayerShot"))
        {
            other.gameObject.SetActive (false);
            if (vulnerable) GetStrike (GameManager.Instance.player.stats.damage);
        }
    }

}
