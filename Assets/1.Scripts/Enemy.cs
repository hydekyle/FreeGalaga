using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class Enemy : MonoBehaviour
{
    public BoostType powerUp = BoostType.None;
    public int points = 100;
    public EnemyBehavior defaultBehavior = EnemyBehavior.Kamikaze;
    public BulletType bulletType = BulletType.GreenBullet;
    public bool shootAtStart = false;
    public Stats stats;

    [HideInInspector]
    public EnemyBehavior activeBehavior = EnemyBehavior.None;
    [HideInInspector]
    public Transform playerT;

    [HideInInspector]
    public bool alive = true;
    Vector3 targetPos;
    float lastTimeShot;
    SpriteRenderer spriteRenderer;

    EZObjectPool myBulletPool;

    public string ID;

    public bool isLittleOne;

    private void Start()
    {
        Initialize();
        if (shootAtStart)
        {
            GetShootOnCooldown();
            activeBehavior = EnemyBehavior.Shooter;
        }
    }

    float GetRandomCooldown()
    {
        return Random.Range(2f / stats.shootCooldown, Mathf.Clamp(10 - stats.shootSpeed, 1.1f, 10f));
    }

    void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerT = GameManager.Instance.player.transform;
        ID = transform.name.Split(' ')[1].Substring(0, 2);

        switch (bulletType)
        {
            case BulletType.FireBullet:
                myBulletPool = GameManager.Instance.enemyBulletsPoolFire;
                break;
            case BulletType.RedBullet:
                myBulletPool = GameManager.Instance.enemyBulletsPoolRed;
                break;
            default:
                myBulletPool = GameManager.Instance.enemyBulletsPoolGreen;
                break;
        }
    }

    private void Update()
    {
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 20);
        if (GameManager.Instance.gameIsActive && EnemiesManager.Instance.animationSpeed > 0)
        {
            if (activeBehavior == EnemyBehavior.Kamikaze) ChasePlayer(false);
            else if (activeBehavior == EnemyBehavior.PointAndShoot) PointAndShot();
            else if (activeBehavior == EnemyBehavior.Shooter && IsShootAvailable()) Shoot();
            else if (activeBehavior == EnemyBehavior.Leader) ChasePlayer(true);
        }
    }

    public void GetShootOnCooldown()
    {
        lastTimeShot = Time.time + GetRandomCooldown();
    }

    void Shoot()
    {
        if (myBulletPool.TryGetNextObject(transform.position, Quaternion.identity, out GameObject bullet))
        {
            switch (bulletType)
            {
                case BulletType.GreenBullet:
                    AudioManager.Instance.PlayEnemyShotSlow();
                    break;
                case BulletType.RedBullet:
                    AudioManager.Instance.PlayEnemyShot();
                    break;
                case BulletType.FireBullet:
                    AudioManager.Instance.PlayEnemyShotFire();
                    break;
            }
            bullet.GetComponent<Rigidbody2D>().velocity = Vector3.down * stats.shootSpeed;
            GetShootOnCooldown();
        }
    }

    bool IsShootAvailable()
    {
        return Time.time > lastTimeShot;
    }

    void PointAndShot()
    {
        var distanceToTarget = Vector3.Distance(transform.position, targetPos);
        if (Mathf.Approximately(distanceToTarget, 0f) && IsShootAvailable())
        {
            Shoot();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * stats.movementVelocity);
        }
    }

    bool falling = false;

    void ChasePlayer(bool isLeader)
    {
        var distance = Vector3.Distance(transform.position, playerT.position);
        float velocity = Mathf.Clamp(distance + stats.movementVelocity / 4f, 0.55f, 3f);
        var dir = (playerT.position - transform.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (!isLeader && transform.position.y + 0.33f < playerT.transform.position.y) falling = true;
        if (falling) dir = new Vector2(dir.x, -dir.y);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, transform.forward), Time.deltaTime * velocity);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime * velocity);
    }

    void GetStrike(int strikeForce)
    {
        if (!alive) return;
        stats.health -= strikeForce;
        if (stats.health <= 0) DestroyedByPlayer();
        else
        {
            spriteRenderer.color = Color.red;
            if (isLittleOne) AudioManager.Instance.PlayEnemyDamagedLow();
            else AudioManager.Instance.PlayEnemyDamaged();

        }
    }

    public void DestroyedByPlayer()
    {
        GameManager.Instance.DropPowerUp(transform.position, powerUp);
        GameManager.Instance.AddScore(points);
        Die();
    }

    public void Die()
    {
        if (defaultBehavior == EnemyBehavior.Leader)
        {
            List<Enemy> tempList = new List<Enemy>();
            foreach (Transform t in transform)
            {
                if (t.gameObject.activeSelf) tempList.Add(t.GetComponent<Enemy>());
            }
            foreach (Enemy enemy in tempList)
            {
                enemy.transform.SetParent(null);
                enemy.SetBehavior(enemy.defaultBehavior);
                EnemiesManager.Instance.AddEnemy(enemy);
            }
        }
        alive = false;
        gameObject.SetActive(false);
        EnemiesManager.Instance.EnemyDestroyed(this);
    }

    public void SetBehavior(EnemyBehavior enemyBehavior)
    {
        switch (enemyBehavior)
        {
            case EnemyBehavior.Leader:
                BehaviorLead();
                break;
            case EnemyBehavior.Shooter:
                BehaviorShooter();
                break;
            case EnemyBehavior.PointAndShoot:
                BehaviorPointAndShoot();
                break;
            case EnemyBehavior.Kamikaze:
                BehaviorChasePlayer();
                break;
            default:
                Debug.Log("Sin behavior");
                break;
        }
    }

    void Unparent()
    {
        transform.parent = null;
        spriteRenderer.sortingOrder = 1;
    }

    void BehaviorLead()
    {
        transform.SetParent(null);
        activeBehavior = EnemyBehavior.Leader;
    }

    void BehaviorChasePlayer()
    {
        float velocity = stats.movementVelocity;
        velocity = Mathf.Clamp(velocity * EnemiesManager.Instance.animationSpeed, 1f, 20f);
        stats.movementVelocity = velocity;
        activeBehavior = EnemyBehavior.Kamikaze;
        Unparent();
    }

    public void BehaviorPointAndShoot()
    {
        targetPos = playerT.position + Vector3.up * Random.Range(1.5f, 5f) + Vector3.right * Random.Range(-1.5f, 1.5f);
        targetPos = new Vector3(
            Mathf.Clamp(targetPos.x, GameManager.Instance.minPosX, GameManager.Instance.maxPosX),
            Mathf.Clamp(targetPos.y + 1.5f, playerT.position.y + 5f, transform.position.y),
            0
        );
        activeBehavior = EnemyBehavior.PointAndShoot;
        lastTimeShot = 0f;
        Unparent();
    }

    public void BehaviorShooter()
    {
        activeBehavior = EnemyBehavior.Shooter;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerShot"))
        {
            other.gameObject.SetActive(false);
            GetStrike(GameManager.Instance.player.stats.damage);
        }
    }

}
