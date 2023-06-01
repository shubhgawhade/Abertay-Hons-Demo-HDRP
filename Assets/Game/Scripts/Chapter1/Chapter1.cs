using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter1 : Chapters
{
    [SerializeField] private GameObject sceneEndUI;
    [SerializeField] private GameObject sceneStartUI;
    
    [SerializeField] private Transform lucaPos2;
    [SerializeField] private Transform pauliePos2;
    [SerializeField] private Transform endScenePos;

    [SerializeField] private GameObject killDecisionUI;
    [SerializeField] private GameObject killOrSpareUI;

    [SerializeField] private TextAsset dialogue1_2Luca;
    [SerializeField] private TextAsset dialogue1_2Paulie;

    [SerializeField] private Transform[] locations;
    [SerializeField] private TextAsset[] storyScripts;
    
    public bool isPaulieKilled;
    public bool calledPostDecision;
    private bool showKillDecisionUI;
    
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

    private bool note;

    protected override void Awake()
    {
        GameManager.Chapter1Manager = this;

        base.Awake();
    }

    private void A()
    {
        // print("STOPPED");
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

                if (!note)
                {
                    sceneStartUI.SetActive(true);
                    Time.timeScale = 0;
                    note = true;
                }

                for (int i = 0; i < characters.Length;)
                {
                    if (aiCharacterControl[i].aIStopped)
                    {
                        // aiCharacterControl[i].cachedTransform = null;
                        i++;    

                        // print(i);
                        if (i == characters.Length)
                        {
                            // print(i + "STOPPED");
                            // player.GetComponent<CharacterControl>().cachedTransform = null;3
                            note = false;
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
                
                if (!scriptSet)
                {
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    scriptSet = true;
                    
                }

                if (textReader.alreadyInteracted)
                {
                    // STRIPPER WALKS
                    aiCharacterControl[2].targetTransform = locations[0];
                    
                    textReader.textAsset = storyScripts[0];
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    scene++;
                }

                break;
            
            case Scene.PreFightAnims:
                if (textReader.dialogueTracker == 0)
                {
                    StartCoroutine(A(0));
                    animators[0].SetBool("GunDrawn", true);
                    animators[1].SetBool("GunDrawn", true);
                    //ENABLE WEAPONS
                    weapons[1].SetActive(true);
                    weapons[2].SetActive(true);

                    if (showKillDecisionUI)
                    {
                        
                        scene++;
                    }
                    
                }
                
                break;
            
            case Scene.KillDecision:
                
                killDecisionUI.SetActive(true);
                characters[2].transform.parent = player.transform;

                if (!scenePaused)
                {
                    Time.timeScale = 0;
                    scenePaused = true;
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    // print("Left");
                    isPaulieKilled = true;
                    // ROTATE TOWARDS PAULIE
                    player.transform.LookAt(characters[1].transform.position);
                    StartCoroutine(A(1));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    // print("Right");
                    // ROTATE TOWARDS LUCA
                    player.transform.LookAt(characters[0].transform.position);
                    // animators[0].SetBool("GunDrawn", false);
                    StartCoroutine(A(1));
                }
                
                
                break;
            
            case Scene.PostDecisionAnimations:
                // STRIPPER DEATH ANIM
                animators[2].SetTrigger("Dead");
                // aiCharacterControl[2].characterState = CharacterControl.CharacterState.Dead;
                aiCharacterControl[2].m_Capsule.enabled = false;
                aiCharacterControl[2].enabled = false;
                aiCharacterControl[2].agent.enabled = false;
                
                playerAnimator.SetBool("Grabbing", false);
                
                characters[2].transform.parent = null;
                
                if (isPaulieKilled)
                {
                    // print("Paulie Death Anim");
                    animators[1].SetTrigger("Dead");
                    // aiCharacterControl[1].characterState = CharacterControl.CharacterState.Dead;
                    aiCharacterControl[1].m_Capsule.enabled = false;
                    aiCharacterControl[1].enabled = false;
                    aiCharacterControl[1].agent.enabled = false;


                    // LUCA STAB ANIMATION and add timer before shot
                    animators[0].SetTrigger("Stabbing");
                    animators[1].SetBool("GunDrawn", false);


                    StartCoroutine(A(2));
                    // animators[0].SetBool("Shot", true);
                }
                else
                {
                    // print("Luca Death Anim");
                    animators[0].SetTrigger("Dead");
                    // aiCharacterControl[0].characterState = CharacterControl.CharacterState.Dead;
                    aiCharacterControl[0].m_Capsule.enabled = false;
                    aiCharacterControl[0].enabled = false;
                    aiCharacterControl[0].agent.enabled = false;

                    animators[0].SetBool("GunDrawn", false);
                    // PAULIE RUNNING TOWARDS PLAYER
                    StartCoroutine(A(2));
                }
                
                // PLAYER INJURED ANIMATION
                // PLAYER SHOOTS AT LAST CHARACTER
                
                // OTHER CHARACTER INJURED ANIMATION
                
                /*
                print(aiCharacterControl[1].aIStopped);
                if (isPaulieKilled)
                {
                    animators[0].SetBool("Shot", true);
                }
                else if(aiCharacterControl[1].aIStopped)
                {
                    animators[1].SetBool("Shot", true);
                }
                */
                
                break;
            
            case Scene.PostDecisionDialogue:
                // print("POST DECISION DIALOGUE");
                playerAnimator.ResetTrigger("Shoot");

                if (isPaulieKilled)
                {
                    textReader.textAsset = dialogue1_2Luca;
                    player.transform.LookAt(characters[0].transform);
                }
                else
                {
                    textReader.textAsset = dialogue1_2Paulie;
                    player.transform.LookAt(characters[1].transform);
                }
                
                textReader.LoadScript();
                textReader.ToggleUI();
                
                scene++;
                break;
            
            case Scene.SurvivorDecision:
                
                if (textReader.dialogueTracker == 0)
                {
                    killOrSpareUI.SetActive(true);
                    playerAnimator.SetBool("GunDrawn", true);

                    if (Input.GetMouseButtonDown(0))
                    {
                        // print("KILL");
                        if (isPaulieKilled)
                        {
                            player.transform.LookAt(characters[0].transform);
                            animators[0].SetTrigger("Finished");
                        }
                        else
                        {
                            player.transform.LookAt(characters[1].transform);
                            animators[1].SetTrigger("Finished");
                        }
                        playerAnimator.SetTrigger("Shoot");
                        weapons[0].transform.GetChild(0).gameObject.SetActive(true);
                        // print("FLARE");
                        //PLAY DEATH ANIM ON LUCA OR PAULIE
                        
                        killOrSpareUI.SetActive(false);
                        
                        // ENABLE FREE ROAM
                        // GameManager.IsMoveable = true;
                        scene++;
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        // print("SPARE");
                        
                        killOrSpareUI.SetActive(false);
                        
                        // ENABLE FREE ROAM
                        // GameManager.IsMoveable = true;
                        scene++;
                    }
                }
                
                break;
            
            case Scene.EOC:
                playerAnimator.SetBool("GunDrawn", false);
                weapons[0].SetActive(false);
                playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Exploration);

                if ((player.transform.position - endScenePos.transform.position).magnitude < 3f)
                {
                    // print("CHAPTER 2");
                    Time.timeScale = 0;
                    sceneEndUI.SetActive(true);
                    // ENABLE SCRIPT
                }
                
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

    public void Continue()
    {
        sceneStartUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene(2);
    }

    IEnumerator A(int num)
    {
        switch (num)
        {
            case 0:
                yield return new WaitForSeconds(0.5f);
                playerAnimator.SetBool("GunDrawn", true);
                playerAnimator.SetBool("Grabbing", true);
                weapons[0].SetActive(true);
                animators[2].SetBool("Grabbed", true);
                // LUCA RUNNING ANIM
                aiCharacterControl[0].targetTransform = lucaPos2;
                characters[0].GetComponent<CharacterMovement>().run = true;
                yield return new WaitForSeconds(1.5f);
                showKillDecisionUI = true;
                break;
            
            case 1:
                // PLAYER GUNSHOT ANIM
                playerAnimator.SetTrigger("Shoot");
                playerAnimator.SetLayerWeight(1, 1);
                weapons[0].transform.GetChild(0).gameObject.SetActive(true);
                // print("FLARE");

                // PAULIE GUNSHOT ANIM
                animators[1].SetTrigger("Shoot");
                weapons[2].transform.GetChild(0).gameObject.SetActive(true);
                // print("FLARE");

                // GUNSHOT PARTICLE FX

                scenePaused = false;
                Time.timeScale = 1;
                // yield return new WaitForSeconds(0.3f);
                killDecisionUI.SetActive(false);
                scene++;
                break;
            
            case 2:

                if (!calledPostDecision)
                {
                    if (isPaulieKilled)
                    {
                        calledPostDecision = true;
                        
                        yield return new WaitForSeconds(0.5f);
                        animators[0].SetBool("GunDrawn", false);
                        yield return new WaitForSeconds(0.5f);
                        player.transform.LookAt(characters[0].transform);
                        playerAnimator.SetTrigger("Shoot");
                        yield return new WaitForSeconds(0.3f);
                        weapons[0].transform.GetChild(0).gameObject.SetActive(true);
                        playerAnimator.SetBool("GunDrawn", false);
                        animators[0].SetBool("Shot", true);
                        // aiCharacterControl[0].characterState = CharacterControl.CharacterState.Dead;
                        aiCharacterControl[0].m_Capsule.enabled = false;
                        aiCharacterControl[0].enabled = false;
                        aiCharacterControl[0].agent.enabled = false;
                        yield return new WaitForSeconds(2f);
                        scene++;
                        StopAllCoroutines();
                    }
                    else
                    {
                        calledPostDecision = true;

                        yield return new WaitForSeconds(0.5f);
                        animators[1].SetBool("GunDrawn", false);
                        aiCharacterControl[1].targetTransform = pauliePos2;
                        characters[1].GetComponent<CharacterMovement>().run = true;
                        yield return new WaitForSeconds(3f);
                        player.transform.LookAt(characters[1].transform);
                        playerAnimator.SetTrigger("Shoot");
                        // print("FLARE" + calledPostDecision);
                        weapons[0].transform.GetChild(0).gameObject.SetActive(true);
                        yield return new WaitForSeconds(0.3f);
                        playerAnimator.SetBool("GunDrawn", false);
                        animators[1].SetBool("Shot", true);
                        // aiCharacterControl[1].characterState = CharacterControl.CharacterState.Dead;
                        aiCharacterControl[1].m_Capsule.enabled = false;
                        aiCharacterControl[1].enabled = false;
                        aiCharacterControl[1].agent.enabled = false;
                        yield return new WaitForSeconds(2f);
                        scene++;
                        StopAllCoroutines();
                    }
                }

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