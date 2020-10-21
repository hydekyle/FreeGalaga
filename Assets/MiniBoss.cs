using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{
    public Transform boss;
    public List<Enemy> minions = new List<Enemy> ();
    public Stats stats;
    public Vector3 leftPos, rightPos;
    SpriteRenderer spriteRenderer;
    Vector3 targetPos = Vector3.up;

    bool vulnerable = false;
    float startPingPongTime;
    float nextTimePosChange;

    private void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    private void FixedUpdate ()
    {
        if (!vulnerable && IsAllMinionsDead ()) LoseVulnerability ();
    }

    void SwitchTargetPos ()
    {
        targetPos = targetPos == leftPos ? rightPos : leftPos;
        nextTimePosChange = Time.time + Random.Range (2.5f, 5f);
    }

    private void Update ()
    {
        spriteRenderer.color = Color.Lerp (spriteRenderer.color, Color.white, Time.deltaTime * 15);
        if (vulnerable)
        {
            if (nextTimePosChange < Time.time) SwitchTargetPos ();
            transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        }
    }

    void ThrowBomb ()
    {
        if (GameManager.Instance.enemyBombs.TryGetNextObject (transform.position, Quaternion.identity, out GameObject enemyBomb))
        {
            AudioManager.Instance.PlayEnemyShieldImpact ();
            Vector2 bombDirection = Vector2.up * 4 + Vector2.right * Random.Range (-1.8f, 1.8f);
            enemyBomb.GetComponent<Rigidbody2D> ().AddForce (bombDirection, ForceMode2D.Impulse);
        }
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
        EnemiesManager.Instance.StopEnemies ();
        GetComponent<Animator> ().enabled = false;
        transform.SetParent (null);
        vulnerable = true;
        //StartCoroutine (MyRoutine ());
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
        else
        {
            AudioManager.Instance.PlayBossDamaged ();
            spriteRenderer.color = Color.red;
        }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("PlayerShot"))
        {
            if (vulnerable) GetStrike (GameManager.Instance.player.stats.damage);

            other.gameObject.SetActive (false);
            ThrowBomb ();
        }
    }

}
