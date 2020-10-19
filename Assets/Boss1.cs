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
    public GameObject bulletPrefab;
    public GameObject bombPrefab;
    EZObjectPool bullets;
    EZObjectPool bombs;
    float nextTimeShoot, nextTimeBomb, nextTimeMoved;

    private void Awake ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        playerT = GameManager.Instance.player.transform;
        targetPos = GetScreenPos (ScreenPosition.BotMid);
        foreach (Transform t in transform.Find ("Cannons")) cannons.Add (t);
        bullets = EZObjectPool.CreateObjectPool (bulletPrefab, "Bullets Boss", 4, true, true, true);
        bombs = EZObjectPool.CreateObjectPool (bombPrefab, "Bombs Boss", 4, true, true, true);
    }

    private void Update ()
    {
        transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        if (Time.time > nextTimeShoot) Shoot ();
        else if (Time.time > nextTimeBomb) ThrowBomb ();
        else if (Time.time > nextTimeMoved) MoveToNewPosition ();
    }

    void Shoot ()
    {
        if (bullets.TryGetNextObject (GetRandomCannonPos (), Quaternion.identity, out GameObject enemyBullet))
        {
            enemyBullet.GetComponent<Rigidbody2D> ().velocity = Vector2.down * stats.shootSpeed;
            nextTimeShoot = Time.time + Random.Range (0.1f, 1f);
        }
    }

    void ThrowBomb ()
    {
        if (bombs.TryGetNextObject (transform.position, Quaternion.identity, out GameObject enemyBomb))
        {
            Vector2 bombDirection = Vector2.up * 4 + Vector2.right * Random.Range (-1.8f, 1.8f);
            enemyBomb.GetComponent<Rigidbody2D> ().AddForce (bombDirection, ForceMode2D.Impulse);
            nextTimeBomb = Time.time + Random.Range (0.5f, 1.5f);
        }
    }

    void MoveToNewPosition ()
    {
        targetPos = GetScreenPos (Random.Range (0, 9));
        nextTimeMoved = Time.time + Random.Range (4f, 9f);
    }

    Vector3 GetRandomCannonPos ()
    {
        return cannons [Random.Range (0, cannons.Count)].position;
    }

    Vector3 GetScreenPos (ScreenPosition screenPos)
    {
        Vector3 pos = Vector3.zero;

        switch (screenPos)
        {
            case ScreenPosition.TopLeft:
                pos = new Vector3 (-3f, 2f, 0);
                break;
            case ScreenPosition.TopMid:
                pos = new Vector3 (0, 2f, 0);
                break;
            case ScreenPosition.TopRight:
                pos = new Vector3 (3f, 2f, 0);
                break;

            case ScreenPosition.MidLeft:
                pos = new Vector3 (-3f, 1f, 0);
                break;
            case ScreenPosition.MidMid:
                pos = new Vector3 (0f, 1f, 0);
                break;
            case ScreenPosition.MidRight:
                pos = new Vector3 (3f, 1f, 0);
                break;

            case ScreenPosition.BotLeft:
                pos = new Vector3 (-3f, 0f, 0);
                break;
            case ScreenPosition.BotMid:
                pos = new Vector3 (0f, 0f, 0);
                break;
            case ScreenPosition.BotRight:
                pos = new Vector3 (3f, 0f, 0);
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
                pos = new Vector3 (-3f, 2f, 0);
                break;
            case 1:
                pos = new Vector3 (0, 2f, 0);
                break;
            case 2:
                pos = new Vector3 (3f, 2f, 0);
                break;

            case 3:
                pos = new Vector3 (-3f, 1f, 0);
                break;
            case 4:
                pos = new Vector3 (0f, 1f, 0);
                break;
            case 5:
                pos = new Vector3 (3f, 1f, 0);
                break;

            case 6:
                pos = new Vector3 (-3f, 0f, 0);
                break;
            case 7:
                pos = new Vector3 (0f, 0f, 0);
                break;
            case 8:
                pos = new Vector3 (3f, 0f, 0);
                break;

            default:
                pos = Vector3.zero;
                break;
        }

        return pos;
    }

    void GetStrike (int damage)
    {

    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("PlayerShot"))
        {
            other.gameObject.SetActive (false);
            GetStrike (GameManager.Instance.player.stats.damage);
        }
    }

}
