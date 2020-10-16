using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public ScriptableSounds scriptableSounds;
    public AudioSource musicSource, soundsSource;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    public void StartMusic ()
    {
        musicSource.clip = scriptableSounds.mainTheme;
        musicSource.Play ();
    }

    public void PlayAudioClip (AudioClip clip)
    {
        soundsSource.PlayOneShot (clip);
    }
}
