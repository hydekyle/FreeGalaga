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

    float nextTimeShoot, nextTimeShootFire, nextTimeBomb, nextTimeMoved, nextTimeDropBoost;
    SpriteRenderer spriteRenderer;
    int nextHealthToDropPoints;
    int nextHealthToDropShield;

    private void Awake ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        nextHealthToDropPoints = stats.health - 400;
        nextHealthToDropShield = stats.health / 2;
        nextTimeDropBoost = Time.time + 7f;
        spriteRenderer = GetComponent<SpriteRenderer> ();
        playerT = GameManager.Instance.player.transform;
        targetPos = GetScreenPos (ScreenPosition.BotMid);
        foreach (Transform t in transform.Find ("Cannons")) cannons.Add (t);
        nextTimeShoot = nextTimeShootFire = nextTimeBomb = Time.time + 3f;
    }

    void DropBoost ()
    {
        GameManager.Instance.DropPowerUp (transform.position, BoostType.AttackSpeed);
        nextTimeDropBoost = Time.time + 13f;
    }

    void DropShield ()
    {
        GameManager.Instance.DropPowerUp (transform.position, BoostType.Shield);
        nextHealthToDropShield = -1;
    }

    void DropPoints ()
    {
        GameManager.Instance.DropPowerUp (transform.position, BoostType.Points);
        nextHealthToDropPoints -= 400;
    }

    private void Update ()
    {
        spriteRenderer.color = Color.Lerp (spriteRenderer.color, Color.white, Time.deltaTime * 15);
        transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        if (Time.time > nextTimeShoot) Shoot ();
        else if (Time.time > nextTimeDropBoost) DropBoost ();
        else if (Time.time > nextTimeMoved) MoveToNewPosition ();
        //else if (Time.time > nextTimeBomb && stats.health < 2000) ThrowBomb ();
    }

    void Shoot ()
    {
        if (GameManager.Instance.enemyBulletsPoolRed.TryGetNextObject (GetRandomCannonPos (), Quaternion.identity, out GameObject enemyBullet))
        {
            enemyBullet.GetComponent<Rigidbody2D> ().velocity = Vector2.down * stats.shootSpeed;
            nextTimeShoot = Time.time + Random.Range (0.13f, 0.66f);
            if (Time.time > nextTimeShootFire) ShootFire ();
        }
    }

    void ShootFire ()
    {
        if (GameManager.Instance.enemyBulletsPoolFire.TryGetNextObject (cannons [4].transform.position, Quaternion.identity, out GameObject enemyFire))
        {
            var punteriaDesafinada = new Vector3 (Random.Range (-1.3f, 1.3f), Random.Range (-1f, 1f));
            var forceDirection = ((GameManager.Instance.player.transform.position + punteriaDesafinada) - cannons [4].transform.position).normalized;
            var rot_z = Mathf.Atan2 (forceDirection.y, forceDirection.x) * Mathf.Rad2Deg;
            enemyFire.transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90 * 3);
            enemyFire.GetComponent<Rigidbody2D> ().velocity = forceDirection * stats.shootSpeed * 1.50f;
            nextTimeShootFire = Time.time + Random.Range (0.66f, stats.health > 1000 ? 1.5f : 0.99f);
        }
    }

    void ThrowBomb ()
    {
        if (GameManager.Instance.enemyBombs.TryGetNextObject (cannons [4].transform.position, Quaternion.identity, out GameObject enemyBomb))
        {
            Vector2 bombDirection = Vector2.up * 3 + Vector2.right * Random.Range (-1.75f, 1.75f);
            var bombRB = enemyBomb.GetComponent<Rigidbody2D> ();
            bombRB.AddForce (bombDirection, ForceMode2D.Impulse);
            if (stats.health < 999) bombRB.gravityScale = 2;
            nextTimeBomb = Time.time + Random.Range (0.66f, 0.99f);
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
        var randomCannon = Random.Range (0, cannons.Count - 1);
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
        CanvasManager.Instance.AddScore (damage);
        if (stats.health <= 0) Die ();
        else
        {
            AudioManager.Instance.PlayBossDamaged ();
            spriteRenderer.color = Color.red;
            if (stats.health < nextHealthToDropPoints) DropPoints ();
            if (stats.health < nextHealthToDropShield) DropShield ();
            if (stats.health < 2000 && Time.time > nextTimeBomb) ThrowBomb ();
        }
    }

    void Die ()
    {
        CanvasManager.Instance.AddScore (11000);
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
