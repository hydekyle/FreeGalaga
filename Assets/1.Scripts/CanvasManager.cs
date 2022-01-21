using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Linq;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    int myRankPosition = 11;
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
    public TextMeshProUGUI historyText;
    public GameObject storyImage, mainStoryButton, aliasEditWindow;
    public RectTransform loadingBlackScreen, courtineScreen;
    public Button informationButton;
    public InputField inputTextAlias;

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

    public void ShowHighScores(User[] users)
    {
        var topUsers = users.ToList();
        Transform content = highScoresWindow.Find("Leader Board").Find("Scroll View").Find("Viewport").Find("Content");
        for (var x = 0; x < topUsers.Count; x++)
        {
            var userSlot = content.GetChild(x);
            bool itsMe = topUsers[x].alias == GameSession.Instance.user.alias;
            var usernameText = userSlot.Find("Username").GetComponent<TextMeshProUGUI>();
            usernameText.text = itsMe ? GameSession.Instance.user.alias : topUsers[x].alias;
            userSlot.Find("Points").GetComponent<TextMeshProUGUI>().text = topUsers[x].score.ToString();
            userSlot.Find("Avatar").GetComponent<Image>().sprite = GetAvatarSprite(itsMe ? PlayerPrefs.GetInt("avatar") : topUsers[x].avatar);
            if (itsMe)
            {
                usernameText.color = Color.yellow;
                myRankPosition = x + 1;
            }
            SetAndroidControls(false);
            highScoresWindow.gameObject.SetActive(true);
            Invoke("MakeRetryAvailable", 1f);
        }
    }

    public void SetAndroidControls(bool status)
    {
        androidControls.gameObject.SetActive(status);
    }

    public void BTN_SendScoreFB()
    {
        GameManager.ShareScore(myRankPosition, PlayerPrefs.GetInt("score"));
    }

    private void MakeRetryAvailable()
    {
        GameManager.Instance.retryAvailable = true;
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

    public void SetAvatarSprite(int avatarIndex)
    {
        PlayerPrefs.SetInt("avatar", avatarIndex);
        GameSession.Instance.user.avatar = avatarIndex;
        avatarHolder.sprite = GetAvatarSprite(avatarIndex);
    }

    Sprite GetAvatarSprite(int avatarIndex)
    {
        var selected = Mathf.Clamp(avatarIndex, 1, spritesAvatar.Count + 1) - 1;
        return spritesAvatar[selected];
    }

    public void LoadCanvasText()
    {
        var user = GameSession.Instance.user;
        informationText.text = GameSession.Instance.gameConfiguration.gameInfo;
        informationText.ForceMeshUpdate();
        avatarHolder.sprite = GetAvatarSprite(user.avatar);
        textAlias.text = user.alias;
        usernameText.text = user.alias;
        SetScoreUI(user.score);
    }

    public void ShowStartMenu()
    {
        loadingBlackScreen.gameObject.SetActive(false);
        startMenu.SetActive(true);
        var menuScreen = startMenu.GetComponent<RectTransform>();
        var topY = courtineScreen.transform.localPosition;
        courtineScreen.DOAnchorPosY(300f, 1f, false).SetEase(Ease.OutBack).onComplete = new TweenCallback(() =>
        {
            courtineScreen.DOAnchorPosY(1100f, 2f, false).SetEase(Ease.OutBack);
        });
        menuScreen.anchoredPosition = Vector2.down * 600;
        menuScreen.DOAnchorPosY(-44f, 2f, false).SetEase(Ease.InOutExpo);
    }

    public void BTN_Start()
    {
        AudioManager.Instance.PlayRetroPunch();
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
        historyText.text = GameSession.Instance.gameConfiguration.history;
        storiesUI.SetActive(true);
    }

    public void ShowStartGameMessage()
    {
        historyText.text = GameSession.Instance.gameConfiguration.startGameMessage;
        storiesUI.SetActive(true);
        Invoke(nameof(BTN_Next), GameSession.Instance.gameConfiguration.storyLevelWaitTime / 1000f);
    }

    void DisableStoryButton()
    {
        mainStoryButton.gameObject.SetActive(false);
    }

    public void BTN_CloseMainStory()
    {
        AudioManager.Instance.PlayRetroPunch();
        FadeOutMainStory();
        ShowStartGameMessage();
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

    public void BTN_Next()
    {
        storiesUI.SetActive(false);
        var levelNumber = GameManager.Instance.activeLevelNumber;
        if (levelNumber == 0) GameManager.Instance.StartGame();
        else GameManager.Instance.LoadNextLevel();
    }

    public GameObject informationUI;
    public void BTN_InformationBack()
    {
        AudioManager.Instance.PlayButtonClick();
        informationUI.SetActive(false);
    }

    public void BTN_Information()
    {
        AudioManager.Instance.PlayRetroPunch();
        GameManager.Information();
    }

    public void BTN_InformationOpen()
    {
        AudioManager.Instance.PlayButtonClick();
        informationUI.SetActive(true);
    }

    public void BTN_Gameover()
    {
        AudioManager.Instance.PlayRetroPunch();
        GameManager.Gameover();
    }

    public void SetScoreUI(int amount)
    {
        var scoreAmountText = amount.ToString();
        textHighScore.text = scoreAmountText;
        scoreText.text = scoreAmountText;
    }

    public void SetNewAlias()
    {
        var newAlias = inputTextAlias.text;
        if (newAlias.Length == 0 || newAlias.ToLower() == "new alias") return;
        PlayerPrefs.SetString("alias", newAlias);
        GameSession.Instance.user.alias = newAlias;
        textAlias.text = newAlias;
        aliasEditWindow.SetActive(false);
        LoadCanvasText();
    }

}
