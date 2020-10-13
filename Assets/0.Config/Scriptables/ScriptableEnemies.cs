using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableEnemies : ScriptableObject
{
    public List<EnemyModel> enemigos = new List<EnemyModel> ();
}
