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
        if (chasingPlayer && GameManager.Instance.gameIsActive && EnemiesManager.Instance.animationSpeed > 0)
        {
            float velocity = Mathf.Clamp (Vector3.Distance (transform.position, playerT.position) + stats.movementVelocity / 4, 0.66f, 3.3f);
            var dir = (playerT.position - transform.position).normalized;
            var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.AngleAxis (angle, transform.forward), Time.deltaTime * velocity);
            transform.position = Vector3.MoveTowards (transform.position, playerT.position, Time.deltaTime * velocity);
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

    public void Die ()
    {
        CanvasManager.Instance.AddScore (100);
        Erase ();
    }

    public void Erase ()
    {
        alive = false;
        gameObject.SetActive (false);
        EnemiesManager.Instance.EnemyDestroyed (this);
    }

    public void ChasePlayer ()
    {
        int velocity = stats.movementVelocity;
        velocity = Mathf.Clamp (velocity * (int) EnemiesManager.Instance.animationSpeed, 1, 20);
        stats.movementVelocity = velocity;
        transform.parent = null;
        GetComponent<SpriteRenderer> ().sortingOrder = 1;
        chasingPlayer = true;
    }
}
