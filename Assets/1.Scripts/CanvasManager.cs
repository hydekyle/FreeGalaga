using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public int score = 0;
    public Text scoreText, livesText;
    public Image levelBackground;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    public void AddScore (int value)
    {
        score += value;
        scoreText.text = score.ToString ();
    }

    public void SetBackground (Sprite sprite)
    {
        levelBackground.sprite = sprite;
    }

}
