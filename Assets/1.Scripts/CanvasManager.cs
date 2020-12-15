using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public int score = 0;
    public Text scoreText, livesText, levelText, usernameText;
    public Image levelBackground;
    public Image barImage;
    public Transform highScoresWindow;
    public Transform androidControls;
    public Image boostIcon, shieldIcon;
    public Transform starsParent;
    public GameObject startMenu;
    public Sprite spriteStarON;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    private void Start ()
    {
        SetLivesNumber (GameManager.Instance.lives);
    }

    public void SetFillBoostIcon (float fillValue)
    {
        boostIcon.fillAmount = Mathf.MoveTowards (boostIcon.fillAmount, fillValue, Time.deltaTime * 3);
    }

    public void SetFillShieldIcon (float fillValue)
    {
        if (fillValue == 0f) shieldIcon.fillAmount = 0f;
        else shieldIcon.fillAmount = Mathf.MoveTowards (shieldIcon.fillAmount, fillValue, Time.deltaTime * 3);
    }

    public void SendScore (string alias, int score)
    {
        StartCoroutine (NetworkManager.SendHighScore (alias, score, onEnded =>
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
                userSlot.Find ("Username").GetComponent<Text> ().text = topUsers [x].alias;
                userSlot.Find ("Points").GetComponent<Text> ().text = topUsers [x].score;
                userSlot.Find ("Avatar").GetComponent<Image> ().sprite = GetSpriteAvatar (topUsers [x].avatar);
                if (topUsers [x].alias == GameManager.Instance.gameData.userAlias) userSlot.Find ("Username").GetComponent<Text> ().color = Color.yellow;

                GameManager.Instance.SetAndroidControls (false);
                highScoresWindow.gameObject.SetActive (true);
                Invoke ("MakeRetryAvailable", 1f);
            }
        }));
    }

    private void MakeRetryAvailable ()
    {
        GameManager.Instance.retryAvailable = true;
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

    public Button startButton;
    public GameObject bar;
    public Text textHighScore;
    public Text textAlias;
    public Image avatarHolder;
    public List<Sprite> spritesAvatar;

    Sprite GetSpriteAvatar (int avatarNumber)
    {
        var selected = Mathf.Clamp (avatarNumber, 1, spritesAvatar.Count + 1) - 1;
        return spritesAvatar [selected]; // El avatar 01 se corresponde con la posición 0
    }

    public Sprite GetSpriteAvatar (string avatarURL)
    {
        Sprite sprite = spritesAvatar [0];
        try
        {
            var avatarIndex = avatarURL.Split (new string []
            {
                "avatar_player_"
            }, StringSplitOptions.None) [1].Split ('.') [0];
            sprite = GetSpriteAvatar (int.Parse (avatarIndex));
        }
        catch
        {
            Debug.LogWarning ("No se pudo leer el avatar correctamente");
        }
        return sprite;
    }

    public void ShowPlayAvailable (User userData)
    {
        SetStars (int.Parse (userData.intentos));
        SetScore (int.Parse (userData.score));
        avatarHolder.sprite = GetSpriteAvatar (userData.avatar);

        textAlias.text = userData.alias;
        startMenu.SetActive (true);
    }

    public void BTN_Start ()
    {
        StartCoroutine (NetworkManager.ConsumeIntento (GameManager.Instance.alias, () =>
        {
            startMenu.SetActive (false);
            bar.SetActive (true);
            GameManager.Instance.StartGame ();
        }));
    }

    void SetScore (int amount)
    {
        textHighScore.text = amount.ToString ();
    }

    void SetStars (int amount)
    {
        var stars = Mathf.Clamp (amount, 0, 3);
        for (var x = 0; x < stars; x++)
        {
            starsParent.GetChild (x).GetComponent<Image> ().sprite = spriteStarON;
        }
        if (stars == 0) startButton.interactable = false;
    }

}
