using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Chapter1 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private AICharacterControl[] aiCharacterControl;
    [SerializeField] private Animator[] animators;

    private bool nextScene;
    private Animator playerAnimator;

    private void Awake()
    {
        aiCharacterControl = new AICharacterControl[characters.Length];
        animators = new Animator[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            aiCharacterControl[i] = characters[i].GetComponent<AICharacterControl>();
            animators[i] = characters[i].GetComponent<Animator>();
        }

        playerAnimator = player.GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //switch(player.name)


        nextScene = true;
        foreach (AICharacterControl item in aiCharacterControl)
        {
            if(item.agent.velocity.magnitude == 0.0f && item.targetTransform == null) nextScene = false;
        }
        
        
        if(nextScene)
        {
            print("Scene Change");
            playerAnimator.SetTrigger("DrawGun");
            playerAnimator.SetTrigger("Grab");
        }


        /*
            Activate the looking at player 
            on mouse right-click.
        
        if (Input.GetMouseButtonDown(1))
        {
            LookAtPlayer();
            
        }
        */
    }

   
   
/*
    Angle characters in scene to look at the player.

    private void LookAtPlayer()
    {
        foreach (GameObject character in characters)
        {
            character.transform.LookAt(player.transform);
        }
    }
*/
}
