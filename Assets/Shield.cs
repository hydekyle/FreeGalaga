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
        spriteRenderer.color = Color.Lerp (spriteRenderer.color, Color.white, Time.deltaTime * 6.66f);
        if (gameObject.activeSelf && Time.time > nextTimeShutDownShield) DesactivateShield ();
    }

    public void ActivateShield ()
    {
        gameObject.SetActive (true);
        nextTimeShutDownShield = Time.time + 15f;
    }

    public void DesactivateShield ()
    {
        gameObject.SetActive (false);
    }

    void GetStrike ()
    {
        spriteRenderer.color = Color.red;
        nextTimeShutDownShield -= 1.5f;
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
