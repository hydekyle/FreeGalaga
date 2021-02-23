using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverArea : MonoBehaviour
{
    float lastTimeEntered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<Enemy>().ID == "4C") return;
            EnemiesManager.Instance.EnemyTouchBottom(other.GetComponent<Enemy>());
            lastTimeEntered = Time.time;
        }
    }
}
