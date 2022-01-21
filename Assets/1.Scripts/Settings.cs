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
    bool music, sound, touchpad;

    void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            music = PlayerPrefs.GetInt("Music") == 1 ? true : false;
            sound = PlayerPrefs.GetInt("Sound") == 1 ? true : false;
            touchpad = PlayerPrefs.GetInt("Touchpad") == 1 ? true : false;
        }
        else
        {
            music = sound = touchpad = true;
        }

        ResolveValues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenSettings();
    }

    private void ResolveValues()
    {
        if (music) musicIMG.sprite = musicON;
        else musicIMG.sprite = musicOFF;
        if (sound) soundIMG.sprite = soundON;
        else soundIMG.sprite = soundOFF;

        MusicSetActive(music);
        SoundSetActive(sound);
    }

    public void TouchpadEnabled()
    {
        if (touchpad) touchpadIMG.sprite = touchpadON;
        else touchpadIMG.sprite = touchpadOFF;
        TouchpadSetActive(touchpad);
    }

    public void BTN_Music()
    {
        music = !music;
        ResolveValues();
    }

    public void BTN_Sound()
    {
        sound = !sound;
        ResolveValues();
    }

    public void BTN_Touchpad()
    {
        touchpad = !touchpad;
        ResolveValues();
    }

    public void BTN_Accept()
    {
        PlayerPrefs.SetInt("Music", music == true ? 1 : 0);
        PlayerPrefs.SetInt("Sound", sound == true ? 1 : 0);
        PlayerPrefs.SetInt("Touchpad", touchpad == true ? 1 : 0);

        Quit();
    }

    public void BTN_Back()
    {
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
        if (!settings.activeSelf)
        {
            settings.SetActive(true);
            Time.timeScale = 0f;
        }
        //ReadCookie ();
    }

    private void Quit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
