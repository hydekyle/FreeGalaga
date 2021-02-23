using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float nextTimeShutDownShield;
    Player player;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        player = GameManager.Instance.player;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, Time.deltaTime * 6.66f);
        if (gameObject.activeSelf && Time.time > nextTimeShutDownShield) DesactivateShield();
        else
        {
            float fillValue = Mathf.Clamp((nextTimeShutDownShield - Time.time) * 2 / 10f, 0f, 1f);
            CanvasManager.Instance.SetFillShieldIcon(fillValue);
        }
    }

    public void ActivateShield()
    {
        gameObject.SetActive(true);
        nextTimeShutDownShield = Time.time + 15f;
    }

    public void DesactivateShield()
    {
        CanvasManager.Instance.SetFillShieldIcon(0f);
        gameObject.SetActive(false);
    }

    void GetStrike()
    {
        spriteRenderer.color = Color.red;
        nextTimeShutDownShield -= 1.5f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyShot"))
        {
            other.gameObject.SetActive(false);
            GetStrike();
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().DestroyedByPlayer();
            GetStrike();
        }
    }
}
