using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ScriptableSounds : ScriptableObject
{
    [Header("Músicas")]
    public AudioClip mainTheme;

    [Header("Sonidos")]
    public AudioClip basicShot;
    public AudioClip enemyShot, enemyShot2, enemyShotFire;
    public AudioClip enemyDamaged, enemyDamagedLow, bossDamaged;
    public AudioClip lifeUp;
    public AudioClip shieldImpact;
    public AudioClip playerDestroyed;
    public AudioClip explosionLow, explosionBig;
    public AudioClip shipUpgrade;
    public AudioClip gameOverSFX;
    public AudioClip theWin;
    public AudioClip buttonClick;
    public AudioClip blop;
    public AudioClip retroPunch;
}
