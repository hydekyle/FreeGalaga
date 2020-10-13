using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverArea : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy")) GameManager.Instance.GameOver ();
    }
}
