using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isMoveable;
    public static bool IsMoveable = false;
    public static bool isInteracting;
    
    // TEXT READER VARIABLES

    // public static float DialogueSKipDelay;
    [SerializeField] public static float dialogueSkipDelay { get; set; }

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
