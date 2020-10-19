using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyBehavior
{
    None,
    Kamikaze,
    PointAndShoot,
    Shooter
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
