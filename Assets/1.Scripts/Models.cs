using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct User
{
    public int avatar;
    public string id, alias, score;
}

public struct GameConfiguration
{
    public int livesPerCredit, playerMovementSpeed, playerAttackSpeed, storyLevelWaitTime, miniBossHealth, finalBossHealth;
    public List<string> stories;
}

public enum BoostType
{
    None,
    Shield,
    Health,
    Points,
    AttackSpeed
}

public enum EnemyBehavior
{
    None,
    Kamikaze,
    PointAndShoot,
    Shooter,
    Leader
}

public enum ScreenPosition
{
    TopLeft,
    TopMid,
    TopRight,
    MidLeft,
    MidMid,
    MidRight,
    BotLeft,
    BotMid,
    BotRight
}

public enum BulletType
{
    GreenBullet,
    RedBullet,
    FireBullet
}

[Serializable]
public struct EnemyModel
{
    public BoostType powerupDrop;
    public Sprite sprite;
    public string name;
    public Stats stats;
}

[Serializable]
public struct Stats
{
    public int health, damage;
    public float movementVelocity, shootCooldown, shootSpeed;
}

[Serializable]
public struct GameData
{
    public string userAlias;

    public string getHighScoresURL;
    public string sendScoreURL;
    public string getUserDataURL;
    public string gameDataURL;
}