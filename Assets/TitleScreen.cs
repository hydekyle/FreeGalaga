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

    public void BTN_Play ()
    {
        if (!loading)
        {
            image.sprite = historySprite;
            Invoke ("StartGame", 4.5f);
            loading = true;
        }
    }

    void StartGame ()
    {
        SceneManager.LoadScene ("MainLevel");
    }

}
