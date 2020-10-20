using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class Player : MonoBehaviour
{
    public GameObject shotPrefab;
    public Stats stats;
    EZObjectPool playerShots;
    float lastTimeShot;
    float minPosX = -3.8f, maxPosX = 3.8f, minPosY = -4.5f, maxPosY = -2f;
    AudioClip shotAudioClip;
    Transform gunPoint;
    public BoxCollider2D myCollider;
    public int playerLevel = 1;

    private void Start ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        myCollider = GetComponent<BoxCollider2D> ();
        gunPoint = transform.Find ("GunPoint");
        shotAudioClip = AudioManager.Instance.scriptableSounds.basicShot;
        SetBulletsPool (0);
    }

    void SetBulletsPool (int level)
    {
        if (level > GameManager.Instance.tablesEtc.disparosJugador.Count) return;
        playerShots?.ClearPool ();
        var newShotPrefab = GameManager.Instance.tablesEtc.disparosJugador [level];
        playerShots = EZObjectPool.CreateObjectPool (newShotPrefab, "PlayerShots" + level, 4, true, true, true);
    }

    void Update ()
    {
        transform.position += new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0) * Time.deltaTime * stats.movementVelocity;
        transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minPosX, maxPosX), Mathf.Clamp (transform.position.y, minPosY, maxPosY), 0);
    }

    public void Disparar ()
    {
        if (isShootAvailable () && playerShots.TryGetNextObject (GetGunPosition (), Quaternion.identity, out GameObject go))
        {
            go.GetComponent<Rigidbody2D> ().velocity = Vector2.up * stats.shootSpeed * 10;
            lastTimeShot = Time.time;
            AudioManager.Instance.PlayAudioClip (shotAudioClip);
        }
    }

    bool lastShootRight;
    Vector3 GetGunPosition ()
    {
        if (playerLevel == 4)
        {
            lastShootRight = !lastShootRight;
            return gunPoint.transform.position + (lastShootRight ? Vector3.right / 3 : Vector3.left / 3);
        }
        return gunPoint.transform.position;
    }

    bool isShootAvailable ()
    {
        return Time.time > lastTimeShot + 0.4f / stats.shootCooldown;
    }

    void GetStrike ()
    {
        GameManager.Instance.LoseLives (1, 2f);
    }

    public IEnumerator InmuneTime (float time)
    {
        var lastLevel = GameManager.Instance.activeLevelNumber;
        var lastSpeed = EnemiesManager.Instance.animationSpeed;
        EnemiesManager.Instance.SetAnimationSpeed (0);
        lastTimeShot = Time.time + time;
        var spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds (time);
        spriteRenderer.color = Color.white;
        if (lastLevel == GameManager.Instance.activeLevelNumber) EnemiesManager.Instance.SetAnimationSpeed (lastSpeed);
        myCollider.enabled = true;
    }

    public void LevelUp ()
    {
        AudioManager.Instance.PlayAudioClip (GameManager.Instance.tablesSounds.shipUpgrade);
        playerLevel++;
        stats.movementVelocity += 1;
        stats.shootSpeed += 1;
        stats.shootCooldown += 1;
        stats.damage += 1;

        SetBulletsPool (playerLevel - 1);

        try
        {
            GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.tablesEtc.navesJugador [playerLevel - 1];

        }
        catch
        { }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy"))
        {
            other.GetComponent<Enemy> ().Erase ();
            GetStrike ();
        }
        else if (other.CompareTag ("EnemyShot"))
        {
            other.gameObject.SetActive (false);
            GetStrike ();
            EnemiesManager.Instance.ReloadShootCooldown ();
        }
    }

}
