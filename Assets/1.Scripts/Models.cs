using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct User
{
    public int avatar, score;
    public string alias;
}

[Serializable]
public struct TopScore
{
    public User[] users;
}

[Serializable]
public struct GameDataResponse
{
    public GameConfiguration gameConfig;
    public User userData;
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
public struct GameServer
{
    public string getHighScoresURL;
    public string updateScoreURL;
    public string getGameDataURL;
}

[Serializable]
public struct GameConfiguration
{
    public int livesPerCredit, playerMovementSpeed, playerAttackSpeed, storyLevelWaitTime, miniBossHealth, finalBossHealth;
    public string gameInfo, startGameMessage, history;
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