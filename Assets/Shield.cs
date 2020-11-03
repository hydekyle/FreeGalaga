using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    float nextTimeShutDownShield;
    Player player;
    SpriteRenderer spriteRenderer;

    private void Start ()
    {
        player = GameManager.Instance.player;
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    private void Update ()
    {
        transform.position = Vector3.MoveTowards (transform.position, player.transform.position - Vector3.up * 0.5f, Time.deltaTime * 9f);
        spriteRenderer.color = Color.Lerp (spriteRenderer.color, Color.white, Time.deltaTime * 6.66f);
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
        transform.position = new Vector3 (0, -6, 0);
    }

    void GetStrike ()
    {
        spriteRenderer.color = Color.red;
        nextTimeShutDownShield -= 2f;
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("EnemyShot"))
        {
            other.gameObject.SetActive (false);
            GetStrike ();
        }
        else if (other.CompareTag ("Enemy"))
        {
            other.GetComponent<Enemy> ().Die ();
            GetStrike ();
        }
    }
}
