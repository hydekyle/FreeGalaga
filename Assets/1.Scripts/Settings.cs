using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static Settings Instance;
    public AudioMixer audioMixer;
    public Image musicIMG, soundIMG, touchpadIMG;
    public Sprite musicON, musicOFF, soundON, soundOFF, touchpadON, touchpadOFF;
    bool isMusicEnabled, isSoundEnabled, isTouchpadEnabled;
    bool musicCached, soundCached, touchpadCached;

    void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            isMusicEnabled = PlayerPrefs.GetInt("Music") == 1 ? true : false;
            isSoundEnabled = PlayerPrefs.GetInt("Sound") == 1 ? true : false;
            isTouchpadEnabled = PlayerPrefs.GetInt("Touchpad") == 1 ? true : false;
        }
        else
        {
            isMusicEnabled = isSoundEnabled = isTouchpadEnabled = true;
        }

        ResolveSetting();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenSettings();
    }

    private void ResolveSetting()
    {
        if (isMusicEnabled) musicIMG.sprite = musicON;
        else musicIMG.sprite = musicOFF;
        if (isSoundEnabled) soundIMG.sprite = soundON;
        else soundIMG.sprite = soundOFF;
        if (isTouchpadEnabled) touchpadIMG.sprite = touchpadON;
        else touchpadIMG.sprite = touchpadOFF;

        MusicSetActive(isMusicEnabled);
        SoundSetActive(isSoundEnabled);
        TouchpadSetActive(isTouchpadEnabled);
    }

    public void TouchpadEnabled()
    {
        TouchpadSetActive(isTouchpadEnabled);
    }

    public void BTN_Music()
    {
        isMusicEnabled = !isMusicEnabled;
        ResolveSetting();
    }

    public void BTN_Sound()
    {
        isSoundEnabled = !isSoundEnabled;
        ResolveSetting();
    }

    public void BTN_Touchpad()
    {
        isTouchpadEnabled = !isTouchpadEnabled;
        ResolveSetting();
    }

    public void BTN_Accept()
    {
        SaveSettings();
        Quit();
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("Music", isMusicEnabled == true ? 1 : 0);
        PlayerPrefs.SetInt("Sound", isSoundEnabled == true ? 1 : 0);
        PlayerPrefs.SetInt("Touchpad", isTouchpadEnabled == true ? 1 : 0);
    }

    public void BTN_Back()
    {
        isMusicEnabled = musicCached;
        isSoundEnabled = soundCached;
        isTouchpadEnabled = touchpadCached;
        ResolveSetting();
        SaveSettings();
        Quit();
    }

    public void MusicSetActive(bool active)
    {
        audioMixer.SetFloat("MusicVolume", active == true ? -25f : -100f);
    }

    public void SoundSetActive(bool active)
    {
        audioMixer.SetFloat("SoundVolume", active == true ? 0f : -100f);
    }

    void TouchpadSetActive(bool active)
    {
        CanvasManager.Instance.androidControls.gameObject.SetActive(active);
    }

    public void OpenSettings()
    {
        var settings = transform.GetChild(0).gameObject;
        settings.SetActive(true);
        Time.timeScale = 0f;
        musicCached = isMusicEnabled;
        soundCached = isSoundEnabled;
        touchpadCached = isTouchpadEnabled;
    }

    private void Quit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
