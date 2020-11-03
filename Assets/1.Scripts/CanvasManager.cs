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
    public Transform highScoresWindow;
    public int testScore;

    public void SendScore (string username, int points)
    {
        StartCoroutine (NetworkManager.SetHighScore (username, points, onEnded =>
        {
            print (onEnded);
            if (onEnded) ShowHighScores ();
        }));
    }

    public void ShowHighScores ()
    {
        StartCoroutine (NetworkManager.GetHighScores (topUsers =>
        {
            Transform content = highScoresWindow.Find ("Scroll View").Find ("Viewport").Find ("Content");
            for (var x = 0; x < topUsers.Count; x++)
            {
                var userSlot = content.GetChild (x);
                userSlot.Find ("Username").GetComponent<Text> ().text = topUsers [x].username;
                userSlot.Find ("Points").GetComponent<Text> ().text = topUsers [x].points;
                //userSlot.gameObject.SetActive(true);

            }
            highScoresWindow.gameObject.SetActive (true);
        }));
    }

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        SetLivesNumber (GameManager.Instance.lives);
        SendScore ("hydekyle", testScore);
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

    public void SetColorTextUI (Color color)
    {
        foreach (Transform t in levelText.transform.parent) t.GetComponent<Text> ().color = color;
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
