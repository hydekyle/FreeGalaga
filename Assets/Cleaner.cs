using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaner : MonoBehaviour
{
    // Se encarga de desactivar los disparos que han salido fuera del área de juego
    private void OnTriggerExit2D (Collider2D other)
    {
        if (other.CompareTag ("EnemyShot") || other.CompareTag ("PlayerShot")) other.gameObject.SetActive (false);
    }
}
