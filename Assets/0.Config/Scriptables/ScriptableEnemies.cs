using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableEnemies : ScriptableObject
{
    public List<EnemyModel> level1 = new List<EnemyModel> ();
    public List<EnemyModel> level2 = new List<EnemyModel> ();
}
