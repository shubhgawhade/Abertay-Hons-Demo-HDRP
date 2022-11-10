using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isMoveable;
    public static bool IsMoveable = false;
    public static bool isInteracting;

    private void Awake()
    {
        IsMoveable = isMoveable;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // print("IS INTERACTING: " + isInteracting);
    }
}
