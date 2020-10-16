using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Stats stats;
    [HideInInspector]
    public string ID;
    [HideInInspector]
    public Transform playerT;
    [HideInInspector]
    public EnemyBehavior activeBehavior = EnemyBehavior.None;
    [HideInInspector]
    public bool alive = true;
    Vector3 targetPos;
    float lastTimeShot;

    private void Start ()
    {
        playerT = GameManager.Instance.player.transform;
        ID = transform.name.Split (' ') [1].Substring (0, 2);
    }

    private void Update ()
    {
        if (GameManager.Instance.gameIsActive && EnemiesManager.Instance.animationSpeed > 0)
        {
            if (activeBehavior == EnemyBehavior.Kamikaze) ChasePlayer ();
            else if (activeBehavior == EnemyBehavior.PointAndShoot) PointAndShot ();
        }
    }

    void Shoot ()
    {
        if (GameManager.Instance.enemyBulletsPool.TryGetNextObject (transform.position, Quaternion.identity, out GameObject bullet))
        {
            bullet.GetComponent<Rigidbody2D> ().velocity = Vector3.down * stats.damage;
            lastTimeShot = Time.time;
        }
    }

    bool isShootAvailable ()
    {
        return Time.time > lastTimeShot + Random.Range (2.8f, 6.66f) / stats.shootCooldown;
    }

    void PointAndShot ()
    {
        var distanceToTarget = Vector3.Distance (transform.position, targetPos);
        if (Mathf.Approximately (distanceToTarget, 0f) && isShootAvailable ())
        {
            Shoot ();
        }
        else
        {
            transform.position = Vector3.MoveTowards (transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        }
    }

    void ChasePlayer ()
    {
        float velocity = Mathf.Clamp (Vector3.Distance (transform.position, playerT.position) + stats.movementVelocity / 4f, 0.55f, 3f);
        var dir = (playerT.position - transform.position).normalized;
        var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.AngleAxis (angle, transform.forward), Time.deltaTime * velocity);
        transform.position = Vector3.MoveTowards (transform.position, playerT.position, Time.deltaTime * velocity);
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

    public void SetBehavior (EnemyBehavior enemyBehavior)
    {
        switch (enemyBehavior)
        {
            case EnemyBehavior.Shooter:
                BehaviorShooter ();
                break;
            case EnemyBehavior.PointAndShoot:
                BehaviorPointAndShoot ();
                break;
            case EnemyBehavior.Kamikaze:
                BehaviorChasePlayer ();
                break;
            default:
                Debug.Log ("Sin behavior");
                break;
        }
    }

    void BehaviorChasePlayer ()
    {
        int velocity = stats.movementVelocity;
        velocity = Mathf.Clamp (velocity * (int) EnemiesManager.Instance.animationSpeed, 1, 20);
        stats.movementVelocity = velocity;
        transform.parent = null;
        GetComponent<SpriteRenderer> ().sortingOrder = 1;
        activeBehavior = EnemyBehavior.Kamikaze;
    }

    public void BehaviorPointAndShoot ()
    {
        transform.parent = null;
        targetPos = playerT.position + Vector3.up * Random.Range (0f, 4f) + Vector3.right * Random.Range (-1.5f, 1.5f);
        targetPos = new Vector3 (
            Mathf.Clamp (targetPos.x, GameManager.Instance.minPosX, GameManager.Instance.maxPosX),
            Mathf.Clamp (targetPos.y, playerT.position.y + 1, transform.position.y),
            0
        );
        activeBehavior = EnemyBehavior.PointAndShoot;
    }

    public void BehaviorShooter ()
    {

    }

}
