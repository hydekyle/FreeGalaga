using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    float nextTimeShutDownShield;
    Player player;

    private void Start ()
    {
        player = GameManager.Instance.player;
    }

    private void Update ()
    {
        transform.position = Vector3.MoveTowards (transform.position, player.transform.position - Vector3.up * 0.5f, Time.deltaTime * 9f);
        if (gameObject.activeSelf && Time.time > nextTimeShutDownShield) DesactivateShield ();
    }

    public void ActivateShield ()
    {
        gameObject.SetActive (true);
        nextTimeShutDownShield = Time.time + 16f;
    }

    public void DesactivateShield ()
    {
        gameObject.SetActive (false);
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("EnemyShot"))
        {
            other.gameObject.SetActive (false);
            nextTimeShutDownShield -= 2f;
        }
    }
}
