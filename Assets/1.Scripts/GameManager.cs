using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public bool activeGame = false;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        StartGame ();
    }

    public void StartGame ()
    {
        activeGame = true;
        AudioManager.Instance.StartMusic ();
    }

    public void GameOver ()
    {
        player.gameObject.SetActive (false);
        EnemiesManager.Instance.StopEnemies ();
        activeGame = false;
    }

    private void Update ()
    {
        if (!activeGame && Input.GetKeyDown (KeyCode.Mouse0)) RestartLevel ();
        if (Input.GetKeyDown (KeyCode.F1)) EnemiesManager.Instance.MakeCrazyEnemy ();
    }

    public void RestartLevel ()
    {
        SceneManager.LoadScene (0);
    }

}
