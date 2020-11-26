using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class Boss1 : MonoBehaviour
{
    public Stats stats;
    public Transform playerT;
    Vector3 targetPos;
    List<Transform> cannons = new List<Transform> ();

    float nextTimeShoot, nextTimeBomb, nextTimeMoved, nextTimeDropBoost;
    SpriteRenderer spriteRenderer;

    private void Awake ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        nextTimeDropBoost = Time.time + 6f;
        spriteRenderer = GetComponent<SpriteRenderer> ();
        playerT = GameManager.Instance.player.transform;
        targetPos = GetScreenPos (ScreenPosition.BotMid);
        foreach (Transform t in transform.Find ("Cannons")) cannons.Add (t);
        nextTimeShoot = nextTimeBomb = Time.time + 2f;
    }

    void DropBoost ()
    {
        GameManager.Instance.DropPowerUp (transform.position, BoostType.AttackSpeed);
        nextTimeDropBoost = Time.time + 12f;
    }

    private void Update ()
    {
        spriteRenderer.color = Color.Lerp (spriteRenderer.color, Color.white, Time.deltaTime * 15);
        transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        if (Time.time > nextTimeShoot) Shoot ();
        else if (Time.time > nextTimeDropBoost) DropBoost ();
        else if (Time.time > nextTimeBomb) ThrowBomb ();
        else if (Time.time > nextTimeMoved) MoveToNewPosition ();
    }

    void Shoot ()
    {
        if (GameManager.Instance.enemyBulletsPoolGreen.TryGetNextObject (GetRandomCannonPos (), Quaternion.identity, out GameObject enemyBullet))
        {
            enemyBullet.GetComponent<Rigidbody2D> ().velocity = Vector2.down * stats.shootSpeed;
            nextTimeShoot = Time.time + Random.Range (0.1f, 1f);
        }
    }

    void ThrowBomb ()
    {
        if (GameManager.Instance.enemyBombs.TryGetNextObject (transform.position, Quaternion.identity, out GameObject enemyBomb))
        {
            Vector2 bombDirection = Vector2.up * 4 + Vector2.right * Random.Range (-1.6f, 1.6f);
            enemyBomb.GetComponent<Rigidbody2D> ().AddForce (bombDirection, ForceMode2D.Impulse);
            nextTimeBomb = Time.time + Random.Range (0.2f, 0.9f);
        }
    }

    void MoveToNewPosition ()
    {
        targetPos = GetScreenPos (Random.Range (0, 9));
        nextTimeMoved = Time.time + Random.Range (4f, 9f);
    }

    int lastCannonUsed = -1;

    Vector3 GetRandomCannonPos ()
    {
        var randomCannon = Random.Range (0, cannons.Count);
        if (randomCannon == lastCannonUsed)
        {
            randomCannon = randomCannon == cannons.Count - 1 ? 0 : randomCannon + 1;
        }
        lastCannonUsed = randomCannon;
        return cannons [randomCannon].position;
    }

    void GetStrike (int damage)
    {
        stats.health -= damage;
        if (stats.health <= 0) Die ();
        else
        {
            AudioManager.Instance.PlayBossDamaged ();
            spriteRenderer.color = Color.red;
        }
    }

    void Die ()
    {
        CanvasManager.Instance.AddScore (10000);
        GameManager.Instance.FinalBossKilled (transform.position);
        gameObject.SetActive (false);
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("PlayerShot"))
        {
            other.gameObject.SetActive (false);
            GetStrike (GameManager.Instance.player.stats.damage);
        }
    }

    Vector3 GetScreenPos (ScreenPosition screenPos)
    {
        Vector3 pos = Vector3.zero;

        switch (screenPos)
        {
            case ScreenPosition.TopLeft:
                pos = new Vector3 (-1.5f, 2f, 0);
                break;
            case ScreenPosition.TopMid:
                pos = new Vector3 (0, 2f, 0);
                break;
            case ScreenPosition.TopRight:
                pos = new Vector3 (1.5f, 2f, 0);
                break;

            case ScreenPosition.MidLeft:
                pos = new Vector3 (-1.5f, 1f, 0);
                break;
            case ScreenPosition.MidMid:
                pos = new Vector3 (0f, 1f, 0);
                break;
            case ScreenPosition.MidRight:
                pos = new Vector3 (1.5f, 1f, 0);
                break;

            case ScreenPosition.BotLeft:
                pos = new Vector3 (-1.5f, 0f, 0);
                break;
            case ScreenPosition.BotMid:
                pos = new Vector3 (0f, 0f, 0);
                break;
            case ScreenPosition.BotRight:
                pos = new Vector3 (1.5f, 0f, 0);
                break;

            default:
                pos = Vector3.zero;
                break;
        }

        return pos;
    }

    Vector3 GetScreenPos (int screenPos)
    {
        Vector3 pos = Vector3.zero;

        switch (screenPos)
        {
            case 0:
                pos = new Vector3 (-1.5f, 2f, 0);
                break;
            case 1:
                pos = new Vector3 (0, 2f, 0);
                break;
            case 2:
                pos = new Vector3 (1.5f, 2f, 0);
                break;

            case 3:
                pos = new Vector3 (-1.5f, 1f, 0);
                break;
            case 4:
                pos = new Vector3 (0f, 1f, 0);
                break;
            case 5:
                pos = new Vector3 (1.5f, 1f, 0);
                break;

            case 6:
                pos = new Vector3 (-1.5f, 0f, 0);
                break;
            case 7:
                pos = new Vector3 (0f, 0f, 0);
                break;
            case 8:
                pos = new Vector3 (1.5f, 0f, 0);
                break;

            default:
                pos = Vector3.zero;
                break;
        }

        return pos;
    }

}
