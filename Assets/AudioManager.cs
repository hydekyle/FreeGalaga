using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public ScriptableSounds scriptableSounds;
    public AudioSource audioSource;

    private void Awake ()
    {
        if (Instance) Destroy (this.gameObject);
        Instance = this;
    }

    public void StartMusic ()
    {
        audioSource.clip = scriptableSounds.mainTheme;
        audioSource.Play ();
    }
}
