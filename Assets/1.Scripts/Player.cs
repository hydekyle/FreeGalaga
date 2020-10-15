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
    float minPosX = -5f, maxPosX = 5f, minPosY = -4.5f, maxPosY = -2f;
    AudioSource audioSource;
    AudioClip shotAudioClip;
    Transform gunPoint;
    BoxCollider2D myCollider;
    bool hitActive = true;

    private void Start ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        myCollider = GetComponent<BoxCollider2D> ();
        gunPoint = transform.Find ("GunPoint");
        shotAudioClip = AudioManager.Instance.scriptableSounds.basicShot;
        audioSource = GetComponent<AudioSource> ();
        playerShots = EZObjectPool.CreateObjectPool (playerShot, "PlayerShots", 4, true, true, true);
    }

    void Update ()
    {
        transform.position += new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0) * Time.deltaTime * stats.movementVelocity;
        transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minPosX, maxPosX), Mathf.Clamp (transform.position.y, minPosY, maxPosY), 0);
    }

    public void Disparar ()
    {
        if (isShootAvailable () && playerShots.TryGetNextObject (gunPoint.transform.position, Quaternion.identity, out GameObject go))
        {
            go.GetComponent<Rigidbody2D> ().velocity = Vector2.up * stats.shootSpeed * 10;
            lastTimeShot = Time.time;
            audioSource.PlayOneShot (shotAudioClip);
        }
    }

    bool isShootAvailable ()
    {
        return Time.time > lastTimeShot + 0.4f / stats.shootCooldown;
    }

    void GetStrike ()
    {
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
        myCollider.enabled = false;
        var spriteRenderer = GetComponent<SpriteRenderer> ();
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds (time);
        spriteRenderer.color = Color.white;
        EnemiesManager.Instance.SetAnimationSpeed (lastSpeed);
        myCollider.enabled = true;
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy"))
        {
            other.GetComponent<Enemy> ().Erase ();
            GetStrike ();
        }
    }

}
