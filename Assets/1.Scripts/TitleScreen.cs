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

    public void BTN_Play ()
    {
        StartGame ();
        // if (!loading)
        // {
        //     image.sprite = historySprite;
        //     loading = true;
        //     historyGO.SetActive (true);
        // }
    }

    public void BTN_PlayNow ()
    {
        StartGame ();
    }

    void StartGame ()
    {
        titleImage.color = new Color (255f, 255f, 255f, 0.5f);
        SceneManager.LoadScene ("MainLevel");
    }

}
