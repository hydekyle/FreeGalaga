using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Stats stats;
    public Transform playerT;
    public bool chasingPlayer = false;
    public bool alive = true;

    private void Start ()
    {
        playerT = GameManager.Instance.player.transform;
    }

    private void Update ()
    {
        if (chasingPlayer && GameManager.Instance.gameIsActive)
        {
            var dir = (playerT.position - transform.position).normalized;
            var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.AngleAxis (angle, transform.forward), Time.deltaTime * stats.movementVelocity);
            transform.position = Vector3.MoveTowards (transform.position, playerT.position, Time.deltaTime * stats.movementVelocity);
        }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("PlayerShot"))
        {
            other.gameObject.SetActive (false);
            GetStrike (GameManager.Instance.player.stats.damage);
        }
    }

    void GetStrike (int strikeForce)
    {
        stats.health -= strikeForce;
        if (stats.health <= 0) Die ();
    }

    void Die ()
    {
        alive = false;
        CanvasManager.Instance.AddScore (100);
        gameObject.SetActive (false);
        EnemiesManager.Instance.EnemyDestroyed (this);
    }

    public void ChasePlayer ()
    {
        int velocity = stats.movementVelocity * Mathf.FloorToInt (Vector3.Distance (transform.position, playerT.position)) / 10;
        velocity = Mathf.Clamp (velocity, 1, 6);
        stats.movementVelocity = velocity;
        transform.parent = null;
        GetComponent<SpriteRenderer> ().sortingOrder = 1;
        chasingPlayer = true;
    }
}
