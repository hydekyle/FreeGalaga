using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Image image;
    public Sprite historySprite;
    bool loading;
    public Image titleImage;

    public GameObject historyGO;

    private void Start()
    {
        Invoke("StartGame", 0.1f);
    }

    public void BTN_Play()
    {
        StartGame();
    }

    void StartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

}
