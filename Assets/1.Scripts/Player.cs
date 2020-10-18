using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class Player : MonoBehaviour
{
    public GameObject playerShot;
    public Stats stats;
    EZObjectPool playerShots;
    float lastTimeShot;
    float minPosX = -3.8f, maxPosX = 3.8f, minPosY = -4.5f, maxPosY = -2f;
    AudioClip shotAudioClip;
    Transform gunPoint;
    BoxCollider2D myCollider;

    private void Start ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        myCollider = GetComponent<BoxCollider2D> ();
        gunPoint = transform.Find ("GunPoint");
        shotAudioClip = AudioManager.Instance.scriptableSounds.basicShot;
        playerShots = EZObjectPool.CreateObjectPool (playerShot, "PlayerShots", 4, true, true, true);
    }

    float myVelocity;

    void Update ()
    {
        // var targetVelocity = Input.GetKey (KeyCode.Mouse0) ? stats.movementVelocity / 1.8f : stats.movementVelocity;
        // myVelocity = Mathf.Lerp (myVelocity, targetVelocity, Time.deltaTime * 10);
        transform.position += new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0) * Time.deltaTime * stats.movementVelocity;
        transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minPosX, maxPosX), Mathf.Clamp (transform.position.y, minPosY, maxPosY), 0);
    }

    public void Disparar ()
    {
        if (isShootAvailable () && playerShots.TryGetNextObject (gunPoint.transform.position, Quaternion.identity, out GameObject go))
        {
            go.GetComponent<Rigidbody2D> ().velocity = Vector2.up * stats.shootSpeed * 10;
            lastTimeShot = Time.time;
            AudioManager.Instance.PlayAudioClip (shotAudioClip);
        }
    }

    bool isShootAvailable ()
    {
        return Time.time > lastTimeShot + 0.4f / stats.shootCooldown;
    }

    void GetStrike ()
    {
        myCollider.enabled = false;
        GameManager.Instance.LoseLives (1);
        if (GameManager.Instance.lives > 0)
        {
            StartCoroutine (InmuneTime (1.5f));
        }
        else GameManager.Instance.GameOver ();
    }

    IEnumerator InmuneTime (float time)
    {
        var lastSpeed = EnemiesManager.Instance.animationSpeed;
        EnemiesManager.Instance.SetAnimationSpeed (0);
        lastTimeShot = Time.time + time;
        var spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds (time);
        spriteRenderer.color = Color.white;
        EnemiesManager.Instance.SetAnimationSpeed (lastSpeed);
        myCollider.enabled = true;
    }

    public int playerLevel = 0;

    public void LevelUp ()
    {
        playerLevel++;
        stats.movementVelocity += 1;
        stats.shootSpeed += 1;
        stats.shootCooldown += 1;
        try
        {
            GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.tablesEtc.navesJugador [playerLevel];
        }
        catch
        {
            Debug.Log ("El jugador sube de nivel pero no se ha asignado un Sprite para la nueva nave.");
        }
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
        }
    }

}
