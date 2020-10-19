using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    public GameObject bossPrefab;
    public List<Enemy> minions = new List<Enemy> ();
    public Stats stats;

    bool vulnerable = false;

    private void FixedUpdate ()
    {
        if (!vulnerable && IsAllMinionsDead ()) LoseVulnerability ();
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
    }

    void Die ()
    {
        CanvasManager.Instance.AddScore (1000);
        bossPrefab.SetActive (true);
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
