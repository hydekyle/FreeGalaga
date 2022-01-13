using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public int score = 0;
    int myHighScore, myRankPosition = 11;
    public Text scoreText, livesText, levelText, usernameText;
    public Image levelBackground;
    public Image barImage;
    public Transform highScoresWindow;
    public Transform androidControls;
    public Image boostIcon, shieldIcon;
    public Transform starsParent;
    public GameObject startMenu;
    public Sprite spriteStarON;
    public GameObject bar;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textAlias;
    public Image avatarHolder;
    public List<Sprite> spritesAvatar;

    public GameObject storiesUI;
    public TextMeshProUGUI informationText;
    public TextMeshProUGUI storyText;
    public GameObject storyImage, mainStoryButton;
    public RectTransform loadingBlackScreen, courtineScreen;
    public Button informationButton;

    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        SetLivesNumber(GameManager.Instance.lives);
    }

    public void SetFillBoostIcon(float fillValue)
    {
        boostIcon.fillAmount = Mathf.MoveTowards(boostIcon.fillAmount, fillValue, Time.deltaTime * 3);
    }

    public void SetFillShieldIcon(float fillValue)
    {
        if (fillValue == 0f) shieldIcon.fillAmount = 0f;
        else shieldIcon.fillAmount = Mathf.MoveTowards(shieldIcon.fillAmount, fillValue, Time.deltaTime * 3);
    }

    public void SendScore(string alias, int score)
    {
        GameSession.Instance.GameFinished();
        GameManager.Instance.SaveScore(score);
        StartCoroutine(NetworkManager.SendHighScore(alias, score, onEnded =>
        {
            myHighScore = score; // Cache for sharing on FB
            ShowHighScores();
        }));
    }

    public void ShowHighScores()
    {
        StartCoroutine(NetworkManager.GetHighScores(topUsers =>
      {
          Transform content = highScoresWindow.Find("Leader Board").Find("Scroll View").Find("Viewport").Find("Content");
          for (var x = 0; x < topUsers.Count; x++)
          {
              var userSlot = content.GetChild(x);
              userSlot.Find("Username").GetComponent<TextMeshProUGUI>().text = topUsers[x].alias;
              userSlot.Find("Points").GetComponent<TextMeshProUGUI>().text = topUsers[x].score;
              userSlot.Find("Avatar").GetComponent<Image>().sprite = GetSpriteAvatar(topUsers[x].avatar);
              if (topUsers[x].alias == GameManager.Instance.gameData.userAlias)
              {
                  userSlot.Find("Username").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                  myRankPosition = x + 1; // Recachear si estoy en el top (puntuación puede ser diferente)
                  myHighScore = int.Parse(topUsers[x].score);
              }
              GameManager.Instance.SetAndroidControls(false);
              highScoresWindow.gameObject.SetActive(true);
              Invoke("MakeRetryAvailable", 1f);
          }
      }));
    }

    public void BTN_SendScoreFB()
    {
        GameManager.ShareScore(myRankPosition, myHighScore);
    }

    private void MakeRetryAvailable()
    {
        GameManager.Instance.retryAvailable = true;
    }

    public void LoseScore(int value)
    {
        score = score - value > 0 ? score - value : 0;
        scoreText.text = score.ToString();
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }

    public void SetBackground(Sprite sprite)
    {
        levelBackground.sprite = sprite;
    }

    public void SetColorTextUI(Color color)
    {
        usernameText.color = color;
        foreach (Transform t in levelText.transform.parent) t.GetComponent<Text>().color = color;
    }

    public void SetLevelNumber(int levelNumber)
    {
        levelText.text = levelNumber.ToString();
    }

    public void SetLivesNumber(int lives)
    {
        livesText.text = lives.ToString();
    }

    public void SetBarColor(Color color)
    {
        barImage.color = color;
    }

    Sprite GetSpriteAvatar(int avatarNumber)
    {
        var selected = Mathf.Clamp(avatarNumber, 1, spritesAvatar.Count + 1) - 1;
        return spritesAvatar[selected];
    }

    public void LoadUserDataAndShowMenu(User userData)
    {
        StartCoroutine(NetworkManager.GetGameConfiguration(gameConfig =>
        {
            informationText.text = gameConfig.stories[gameConfig.stories.Count - 1];
            GameManager.Instance.gameConfiguration = gameConfig;
            loadingBlackScreen.gameObject.SetActive(false);
            SetUserDataAndStartMenu(userData);
        }));
    }

    private void SetUserDataAndStartMenu(User userData)
    {
        //SetStars(int.Parse(userData.intentos));
        SetScoreUI(int.Parse(userData.score));
        avatarHolder.sprite = GetSpriteAvatar(userData.avatar);
        textAlias.text = userData.alias;
        startMenu.SetActive(true);
        var menuScreen = startMenu.GetComponent<RectTransform>();
        var topY = courtineScreen.transform.localPosition;
        courtineScreen.DOAnchorPosY(300f, 1f, false).SetEase(Ease.OutBack).onComplete = new TweenCallback(() =>
        {
            courtineScreen.DOAnchorPosY(1100f, 2f, false).SetEase(Ease.OutBack);
        });
        menuScreen.anchoredPosition = Vector2.down * 600;
        menuScreen.DOAnchorPosY(-44f, 2f, false).SetEase(Ease.InOutExpo);
        //iTween.MoveTo(startMenu, iTween.Hash("x", 0, "y", 0, "transition", "spring"));
    }

    public void BTN_Start()
    {

        StartGame();
    }

    private void StartGame()
    {
        startMenu.SetActive(false);
        bar.SetActive(true);
        if (GameSession.Instance.IsFirstGame()) ShowMainStory(); // Mostrar solo la primera historia la primera partida
        else
        {
            FadeOutMainStory();
            BTN_Next();
        }
    }

    void ShowMainStory()
    {
        storyText.text = GameManager.Instance.gameConfiguration.stories[0];
        storiesUI.SetActive(true);
    }

    public void ShowLevelStory(int number)
    {
        try
        {
            storyText.text = GameManager.Instance.gameConfiguration.stories[number + 1];
            storiesUI.SetActive(true);
        }
        catch
        { }
        Invoke(nameof(BTN_Next), GameManager.Instance.gameConfiguration.storyLevelWaitTime / 10f);
    }

    void DisableStoryButton()
    {
        mainStoryButton.gameObject.SetActive(false);
    }

    public void BTN_CloseMainStory()
    {
        FadeOutMainStory();
        ShowLevelStory(0);
    }

    IEnumerator WaitSeconds(float seconds, Action onEnded)
    {
        yield return new WaitForSeconds(seconds);
        onEnded();
    }

    private void FadeOutMainStory()
    {
        storyImage.SetActive(false);
        mainStoryButton.SetActive(false);
        mainStoryButton.transform.parent.Find("Button_Next").GetComponent<Button>().interactable = true;
    }

    public void ShowGameover()
    {
        ShowLevelStory(GameManager.Instance.gameConfiguration.stories.Count);
    }

    public void BTN_Next()
    {
        storiesUI.SetActive(false);
        var levelNumber = GameManager.Instance.activeLevelNumber;
        if (levelNumber == 0) GameManager.Instance.StartGame();
        else GameManager.Instance.LoadNextLevel();

        var player = GameManager.Instance.player;
        player.transform.position = new Vector3(0, -4, 0);
        player.lastTimeShot = Time.time;
        Time.timeScale = 1f;
    }

    public GameObject informationUI;
    public void BTN_InformationBack()
    {
        informationUI.SetActive(false);
    }

    public void BTN_Information()
    {
        GameManager.Information();
    }

    public void BTN_InformationOpen()
    {
        informationUI.SetActive(true);
    }

    public void BTN_Gameover()
    {
        GameManager.Gameover();
    }

    void SetScoreUI(int amount)
    {
        textHighScore.text = amount.ToString();
    }

    // void SetStars(int amount)
    // {
    //     var stars = Mathf.Clamp(amount, 0, 3);
    //     for (var x = 0; x < stars; x++)
    //     {
    //         starsParent.GetChild(x).GetComponent<Image>().sprite = spriteStarON;
    //     }
    // }


}
