using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Sprite sprite;
    public string name;
    public Stats stats;
}

[Serializable]
public struct Stats
{
    public int health, damage, movementVelocity, shootCooldown, shootSpeed;
}
