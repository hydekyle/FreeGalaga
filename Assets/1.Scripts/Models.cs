using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct User
{
    public string username, points, avatar;
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
    public int health, damage, movementVelocity, shootCooldown, shootSpeed;
}

[Serializable]
public struct GameConfig
{
    public string highScoresURL;
    public string sendScoreURL;

    public int credits_per_player;
    public int lives_per_credit;
}
