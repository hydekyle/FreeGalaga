using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ShootModel
{
    public Sprite sprite;
    public string name;
    public ShootStats stats;
}

[Serializable]
public struct ShootStats
{
    public int damage, cooldown;
}

[Serializable]
public struct ShipModel
{
    public Sprite sprite;
    public string name;
    public Stats stats;
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
