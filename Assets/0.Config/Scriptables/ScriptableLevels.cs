using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableLevels : ScriptableObject
{
    [Header ("Level 1")]
    public Sprite backgroundLevel1;
    public Color barColorLevel1;
    public float animSpeed1;

    [Header ("Level 2")]
    public Sprite backgroundLevel2;
    public Color barColorLevel2;
    public float animSpeed2;

    [Header ("Level 3")]
    public Sprite backgroundLevel3;
    public Color barColorLevel3;
    public float animSpeed3;

    [Header ("Level 4")]
    public Sprite backgroundLevel4;
    public Color barColorLevel4;
    public float animSpeed4;

    [Header ("Level Final")]
    public Sprite backgroundLevelFinal;
    public Color barColorLevelFinal;
    public float animSpeedFinal;
}
