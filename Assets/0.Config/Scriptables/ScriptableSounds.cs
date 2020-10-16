using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableSounds : ScriptableObject
{
    [Header ("Músicas")]
    public AudioClip mainTheme;

    [Header ("Sonidos")]
    public AudioClip basicShot;
    public AudioClip specialShot;
    public AudioClip playerDestroyed;
    public AudioClip explosionLow, explosionBig;
    public AudioClip shieldBuff, shipUpgrade;
    public AudioClip gameOverSFX;
}
