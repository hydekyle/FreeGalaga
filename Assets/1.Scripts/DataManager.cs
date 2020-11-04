using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public string username;

    private void Awake ()
    {
        if (Instance != null) Destroy (this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad (this.gameObject);
        }
    }
}
