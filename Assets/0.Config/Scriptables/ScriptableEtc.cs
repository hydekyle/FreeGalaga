using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class ScriptableEtc : ScriptableObject
{
    public List<Sprite> navesJugador = new List<Sprite> ();
    public List<GameObject> disparosJugador = new List<GameObject> ();
    public List<GameObject> disparosEnemigos = new List<GameObject> ();

}
