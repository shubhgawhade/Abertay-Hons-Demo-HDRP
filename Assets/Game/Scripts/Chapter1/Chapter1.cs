using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Chapter1 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private AICharacterControl[] aiCharacterControl;
    [SerializeField] private Animator[] animators;

    [SerializeField] private GameObject killDecisionUI;

    public bool isPaulieKilled;
    public bool animPaused;
    private bool showKillDecisionUI;
    private Animator playerAnimator;
    private TextReader textReader;
    
    public enum Scene
    {
        InitialMove,
        InitialDialogue,
        PreFightAnims,
        KillDecision,
        PostDecisionAnimations,
        PostDecisionDialogue,
        SurvivorDecision,
        EOC
    }

    public Scene scene = Scene.InitialMove;

    private void Awake()
    {
        textReader = GetComponent<TextReader>();
        aiCharacterControl = new AICharacterControl[characters.Length];
        animators = new Animator[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            aiCharacterControl[i] = characters[i].GetComponent<AICharacterControl>();
            animators[i] = characters[i].GetComponent<Animator>();
        }

        playerAnimator = player.GetComponent<Animator>();

    }

    private void A()
    {
        
        print("STOPPED");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (scene)
        {
            case Scene.InitialMove:

                for (int i = 0; i < characters.Length;)
                {
                    if (aiCharacterControl[i].aIStopped)
                    {
                        aiCharacterControl[i].cachedTransform = null;
                        i++;

                        // print(i);
                        if (i == characters.Length)
                        {
                            // print(i + "STOPPED");
                            player.GetComponent<AICharacterControl>().cachedTransform = null;
                            scene++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                
                /*
                foreach (AICharacterControl item in aiCharacterControl)
                {
                    if (!item.aIStopped)
                    {
                        break;
                    }

                    print(item.name);
                    player.GetComponent<AICharacterControl>().cachedTransform = null;
                    item.cachedTransform = null;
                    scene++;
                }
                */
                
                break;
            
            case Scene.InitialDialogue:
                
                textReader.ToggleUI();
                scene++;
                break;
            
            case Scene.PreFightAnims:
                if (textReader.dialogueTracker == 0)
                {
                    StartCoroutine(A(0));
                    animators[1].SetTrigger("DrawGun");
                    // LUCA RUNNING ANIM

                    if (showKillDecisionUI)
                    {
                        scene++;
                    }
                    
                }
                
                break;
            
            case Scene.KillDecision:
                
                killDecisionUI.SetActive(true);

                if (!animPaused)
                {
                    Time.timeScale = 0;
                    animPaused = true;
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    print("Left");
                    isPaulieKilled = true;
                    // ROTATE TOWARDS PAULIE
                    StartCoroutine(A(1));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    print("Right");
                    // ROTATE TOWARDS LUCA
                    StartCoroutine(A(1));
                }
                
                
                break;
            
            case Scene.PostDecisionAnimations:
                
                
                if (isPaulieKilled)
                {
                    print("Paulie Death Anim");
                    animators[1].SetTrigger("Dead");
                    
                    // LUCA STAB ANIMATION
                }
                else
                {
                    print("Luca Death Anim");
                    animators[0].SetTrigger("Dead");
                    
                    // PAULIE RUNNING TOWARDS PLAYER
                }
                
                // PLAYER INJURED ANIMATION
                // PLAYER SHOOTS AT LAST CHARACTER
                // OTHER CHARACTER INJURED ANIMATION
                
                break;
            
            case Scene.PostDecisionDialogue:
                break;
            
            case Scene.SurvivorDecision:
                break;
            
            case Scene.EOC:
                break;
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

    IEnumerator A(int num)
    {
        switch (num)
        {
            case 0:
                yield return new WaitForSeconds(0.5f);
                playerAnimator.SetTrigger("DrawGun");
                playerAnimator.SetTrigger("Grab");
                animators[2].SetTrigger("Grabbed");
                yield return new WaitForSeconds(1.5f);
                showKillDecisionUI = true;
                break;
            
            case 1:
                // PAULIE GUNSHOT ANIM
                // PLAYER GUNSHOT ANIM
                // GUNSHOT PARTICLE FX

                animPaused = false;
                Time.timeScale = 1;
                // yield return new WaitForSeconds(0.3f);
                killDecisionUI.SetActive(false);
                scene++;
                break;
        }
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