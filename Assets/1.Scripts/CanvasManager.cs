using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public int score = 0;
    public Text scoreText, livesText, levelText, usernameText;
    public Image levelBackground;
    public Image barImage;
    public Transform highScoresWindow;
    public Transform androidControls;

    public void SendScore (string username, int points)
    {
        StartCoroutine (NetworkManager.SetHighScore (username, points, onEnded =>
        {
            ShowHighScores ();
        }));
    }

    public void ShowHighScores ()
    {
        StartCoroutine (NetworkManager.GetHighScores (topUsers =>
        {
            Transform content = highScoresWindow.Find ("Leader Board").Find ("Scroll View").Find ("Viewport").Find ("Content");
            for (var x = 0; x < topUsers.Count; x++)
            {
                var userSlot = content.GetChild (x);
                userSlot.Find ("Username").GetComponent<Text> ().text = topUsers [x].username;
                userSlot.Find ("Points").GetComponent<Text> ().text = topUsers [x].points;
                if (topUsers [x].username == DataManager.Instance.username) userSlot.Find ("Username").GetComponent<Text> ().color = Color.yellow;
                //userSlot.gameObject.SetActive(true);

            }
            GameManager.Instance.SetAndroidControles (false);
            highScoresWindow.gameObject.SetActive (true);
            Invoke ("MakeRetryAvailable", 1f);
        }));
    }

    private void MakeRetryAvailable ()
    {
        GameManager.Instance.retryAvailable = true;
    }

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

    public void SetColorTextUI (Color color)
    {
        usernameText.color = color;
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
