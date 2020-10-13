using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ScriptableNaves : ScriptableObject 
{
    public List<ShipModel> playerShips = new List<ShipModel>();
    public List<EnemyModel> enemies = new List<EnemyModel>();
}
