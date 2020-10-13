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
    float minPosX = -5f, maxPosX = 5f, minPosY = -4.5f, maxPosY = -2.5f;
    AudioSource audioSource;
    AudioClip shotAudioClip;
    Transform gunPoint;

    private void Start ()
    {
        Initialize ();
        gunPoint = transform.Find ("GunPoint");
    }

    void Initialize ()
    {
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

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy"))
        {
            other.gameObject.SetActive (false);
            GameManager.Instance.GameOver ();
        }
    }

}
