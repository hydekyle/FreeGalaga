﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;

    public GameObject touchpadGO;
    public Image musicIMG, soundIMG, touchpadIMG;
    public Sprite musicON, musicOFF, soundON, soundOFF, touchpadON, touchpadOFF;

    bool music, sound, touchpad;

    private void OnEnable ()
    {
        if (PlayerPrefs.HasKey ("Music")) music = PlayerPrefs.GetInt ("Music") == 1 ? true : false;
        if (PlayerPrefs.HasKey ("Sound")) sound = PlayerPrefs.GetInt ("Sound") == 1 ? true : false;
        if (PlayerPrefs.HasKey ("Touchpad")) touchpad = PlayerPrefs.GetInt ("Touchpad") == 1 ? true : false;

        SetValues ();
    }

    void SetValues ()
    {
        if (music) musicIMG.sprite = musicON;
        else musicIMG.sprite = musicOFF;
        if (sound) soundIMG.sprite = soundON;
        else soundIMG.sprite = soundOFF;
        if (touchpad) touchpadIMG.sprite = touchpadON;
        else touchpadIMG.sprite = touchpadOFF;

        MusicSetActive (music);
        SoundSetActive (sound);
        TouchpadSetActive (touchpad);
    }

    public void BTN_Music ()
    {
        music = !music;
        SetValues ();
    }

    public void BTN_Sound ()
    {
        sound = !sound;
        SetValues ();
    }

    public void BTN_Touchpad ()
    {
        touchpad = !touchpad;
        SetValues ();
    }

    public void BTN_Accept ()
    {
        PlayerPrefs.SetInt ("Music", music == true ? 1 : 0);
        PlayerPrefs.SetInt ("Sound", sound == true ? 1 : 0);
        PlayerPrefs.SetInt ("Touchpad", touchpad == true ? 1 : 0);

        Quit ();
    }

    public void BTN_Back ()
    {
        Quit ();
    }

    void Quit ()
    {
        gameObject.SetActive (false);
        Time.timeScale = 1f;
    }

    public void MusicSetActive (bool active)
    {
        audioMixer.SetFloat ("MusicVolume", active == true ? -25f : -100f);
    }

    public void SoundSetActive (bool active)
    {
        audioMixer.SetFloat ("SoundVolume", active == true ? 0f : -100f);
    }

    public void TouchpadSetActive (bool active)
    {
        touchpadGO.SetActive (active);
    }

}