using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct User
{
    public string alias, score, avatar, intentos;
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
    public int health, damage, shootCooldown, shootSpeed;
    public float movementVelocity;
}

[Serializable]
public struct GameData
{
    public string userAlias;

    public string getHighScoresURL;
    public string sendScoreURL;
    public string getUserDataURL;
    public string consumeIntentosURL;
}

[Serializable]
public struct GameConfig
{
    public int lives_per_credit;
}
