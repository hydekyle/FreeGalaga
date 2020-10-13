using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableSounds : ScriptableObject
{
    [Header ("Músicas")]
    public AudioClip mainTheme;
    public AudioClip gameOver;

    [Header ("Sonidos")]
    public AudioClip basicShot;
    public AudioClip specialShot;
}
