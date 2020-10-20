using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public int score = 0;
    public Text scoreText, livesText, levelText;
    public Image levelBackground;
    public Image barImage;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        SetLivesNumber (GameManager.Instance.lives);
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

    public void SetLevelNumber (int levelNumber)
    {
        levelText.text = levelNumber.ToString ();
    }

    public void SetLivesNumber (int lives)
    {
        livesText.text = lives.ToString ();
    }

    public void SetBarColor (Color color)
    {
        barImage.color = color;
    }

}
