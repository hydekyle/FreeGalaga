using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public ScriptableSounds scriptableSounds;
    public AudioSource musicSource, playerSource, enemySource;

    private void Awake()
    {
        if (Instance) Destroy(this.gameObject);
        Instance = this;
    }

    public void PlayMainThemeMusic()
    {
        musicSource.clip = scriptableSounds.mainTheme;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayAudioPlayer(AudioClip clip)
    {
        playerSource.PlayOneShot(clip);
    }

    public void PlayAudioEnemy(AudioClip clip)
    {
        enemySource.PlayOneShot(clip);
    }

    public void PlayAudioTyping()
    {
        playerSource.PlayOneShot(scriptableSounds.basicShot);
    }

    public void PlayButtonClick()
    {
        PlayAudioPlayer(scriptableSounds.buttonClick);
    }

    public void PlayRetroPunch()
    {
        PlayAudioPlayer(scriptableSounds.retroPunch);
    }

    public void PlayBlop()
    {
        PlayAudioPlayer(scriptableSounds.blop);
    }

    public void PlayPlayerShot()
    {
        PlayAudioPlayer(scriptableSounds.basicShot);
    }

    public void PlayPlayerLevelUp()
    {
        PlayAudioPlayer(scriptableSounds.shipUpgrade);
    }

    public void PlayPlayerLifeUp()
    {
        PlayAudioPlayer(scriptableSounds.lifeUp);
    }

    public void PlayPlayerDestroyed()
    {
        PlayAudioPlayer(scriptableSounds.playerDestroyed);
    }

    public void PlayEnemyShot()
    {
        PlayAudioEnemy(scriptableSounds.enemyShot);
    }

    public void PlayEnemyShotSlow()
    {
        PlayAudioEnemy(scriptableSounds.enemyShot2);
    }

    public void PlayEnemyShotFire()
    {
        PlayAudioEnemy(scriptableSounds.enemyShotFire);
    }

    public void PlayEnemyDamaged()
    {
        PlayAudioEnemy(scriptableSounds.enemyDamaged);
    }

    public void PlayEnemyDamagedLow()
    {
        PlayAudioEnemy(scriptableSounds.enemyDamagedLow);
    }

    public void PlayEnemyExplosionLow()
    {
        PlayAudioEnemy(scriptableSounds.explosionLow);
    }

    public void PlayEnemyShieldImpact()
    {
        PlayAudioEnemy(scriptableSounds.shieldImpact);
    }

    public void PlayBossDamaged()
    {
        PlayAudioEnemy(scriptableSounds.bossDamaged);
    }

}
