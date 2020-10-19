using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverArea : MonoBehaviour
{
    float lastTimeEntered;

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy") && Time.time > lastTimeEntered + 1)
        {
            EnemiesManager.Instance.EnemyTouchBottom ();
            lastTimeEntered = Time.time;
        }
    }
}
