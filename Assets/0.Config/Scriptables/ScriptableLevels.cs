using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableLevels : ScriptableObject
{
    [Header ("Level 1")]
    public Sprite backgroundLevel1;
    public float animSpeed1;

    [Header ("Level 2")]
    public Sprite backgroundLevel2;
    public float animSpeed2;
}
