using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class Chapter2 : Chapters
{
    [SerializeField] private CinemachineTargetGroup cinemachineTarget;
    [SerializeField] private GameObject[] ai;
    [SerializeField] public Interactable[] interactables;
    [SerializeField] private TextReader[] interactablesTextReader;
    [SerializeField] private TextAsset[] storyScripts;
    public int storyScriptNum;
    [SerializeField] private Transform[] friendlyLocations;
    [SerializeField] private Transform[] enemyLocations;
    [SerializeField] private Transform[] playerLocations;
    // public int friendlyLocationsCount;
    
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject doneUI;
    
    
    [Serializable]
    public class Unlockables
    {
        public string what;
        public bool unlocked;
    }

    public Unlockables[] unlockables;

    [Serializable]
    public class UnlockableLocations
    {
        public MaterialFade materialFade;
        public Animator[] doors;
        public bool[] unlocked;
    }
    
    public UnlockableLocations[] unlockLocations;

    [SerializeField] private GameObject shootingTutorialUi;
    [SerializeField] private Animator carAnimator;
    [SerializeField] private GameObject failUI;
    
    public enum Scene
    { 
        InitialExploration,
        ReceptionistDialogue,
        PreMiniGameDialogue,
        MiniGame,
        PostMiniGameDialogue,
        Moe,
        GangMemberKillsMoe,
        GangMemberRunsToCar,
        ShootingTutorial,
        QTE,
        GangMemberRunsAway,
        BackToFriends,
        Shootout,
        Warehouse
    }

    public Scene scene = Scene.InitialExploration;
    private readonly int open = Animator.StringToHash("Open");

    protected override void Awake()
    {
        Time.timeScale = 1;
        GameManager.CurrentScene = 2;
        GameManager.Chapter2Manager = this;
        
        // LOAD SAVED DATA
        
        base.Awake();

        interactablesTextReader = new TextReader[interactables.Length];
        for (int i = 0; i < interactables.Length - 1; i++)
        {
            interactablesTextReader[i] = interactables[i].GetComponent<TextReader>();
        }

        TextReader.Unlock += ChoiceTaken;
        InspectableInteractables.Unlock += ChoiceTaken;
    }

    private void ChoiceTaken(int knows)
    {
        unlockables[knows].unlocked = true;
    }

    public void SceneFlow()
    {
        scene = (Scene)sceneNum;
        switch (scene)
        {
            case Scene.InitialExploration:

                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[0];
                        }
                    }
                    positionSet = true;
                    // friendlyLocationsCount++;
                }

                if (!scriptSet)
                {
                    textReader.textAsset = storyScripts[storyScriptNum];
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    storyScriptNum++;
                    scriptSet = true;
                }

                if (textReader.alreadyInteracted)
                {
                    GameManager.IsInteracting = false;
                    // textReader.alreadyInteracted = false;
                    scriptSet = false;
                    positionSet = false;
                    sceneNum++;
                }
                
                break;
            
            case Scene.ReceptionistDialogue:

                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[1];
                        }
                    }
                    positionSet = true;
                    // friendlyLocationsCount++;
                }

                if (interactables[3].isOccupied)
                {
                    // AUTOSAVE
                    PersistentSave.Save();
                    
                    textReader.alreadyInteracted = false;
                    scriptSet = false;
                    positionSet = false;
                    sceneNum++;
                }

                break;
            
            case Scene.PreMiniGameDialogue:
                
                if (unlockables[0].unlocked)
                {
                    interactablesTextReader[3].textAsset = storyScripts[1];
                    interactablesTextReader[3].LoadScript();
                }

                if (!interactables[3].isOccupied)
                {
                    minigame.SetActive(true);
                    playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Cutscene);
                    sceneNum++;
                }
                
                break;
            
            case Scene.MiniGame:
                
                // MINIGAME
                GameManager.IsInteracting = true;
                print(minigame.activeSelf);
                if (!minigame.activeSelf)
                {
                    GameManager.IsInteracting = false;
                    GameManager.Drunkenness = 50;
                    sceneNum++;
                    playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Exploration);
                }
                // AFTER GAME DIALOGUE

                break;
            
            case Scene.PostMiniGameDialogue:

                // STORY KNOWS ABOUT MAN BEHIND THE COUNTER
                if (unlockables[0].unlocked && !scriptSet)
                {
                    GameManager.IsInteracting = true;
                    textReader.textAsset = storyScripts[2];
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    // storyScriptNum++;
                    scriptSet = true;
                }

                if (textReader.alreadyInteracted)
                {
                    // textReader.alreadyInteracted = false;
                    GameManager.IsInteracting = false;
                    scriptSet = false;
                }

                // STORY RECEPTIONIST DIALOGUE
                if (!scriptSet)
                {
                    GameManager.IsInteracting = true;
                    textReader.textAsset = storyScripts[3];
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    
                    // storyScriptNum++;
                    scriptSet = true;
                    
                    // DOORS OPEN
                    // UNLOCK THE OFFICE AND FADE INTO VIEW
                    // unlockables[1].unlocked = true;
                    UnlockLocation(0, 1);
                    characters[2].SetActive(true);
                }
                
                // NEXT SCENE
                if (characters[2].activeSelf)
                {
                    textReader.alreadyInteracted = false;
                    scriptSet = false;
                    positionSet = false;
                    sceneNum++;
                }
                
                // if (interactables[3].GetComponent<TextReader>().alreadyInteracted)
                // {
                //     textReader.textAsset = receptionistDialogue2;
                //     textReader.LoadScript();
                //     textReader.ToggleUI();
                //     sceneNum++;
                // }
                
                break;
            
            case Scene.Moe:
                
                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[2];
                        }
                    }
                    positionSet = true;
                    // friendlyLocationsCount++;
                }

                if (textReader.alreadyInteracted)
                {
                    GameManager.IsInteracting = false;
                    textReader.alreadyInteracted = false;
                }
                
                if (interactables[4].alreadyInteracted)
                {
                    positionSet = false;
                    sceneNum++;
                }
                
                break;
            
            case Scene.GangMemberKillsMoe:
                
                playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Cutscene);
                // playerCharacterControl.targetTransform = characters[1].transform;

                aiCharacterControl[6].weapon = aiCharacterControl[6].weapons[0];
                aiCharacterControl[6].weapon.gameObject.SetActive(true);
                aiCharacterControl[6].crouch = false;
                aiCharacterControl[6].pointShootRig.weight = 1;
                Vector3 a = interactables[4].transform.position;
                a.y = 2.5f;
                aiCharacterControl[6].aiShootTarget.transform.position = a;
                aiCharacterControl[6].aiLookTarget.transform.position = a;
                aiCharacterControl[2].isFriendly = true;
                
                if (!aiCharacterControl[6].weapon.onCooldown)
                {
                    aiCharacterControl[6].weapon.ShootTarget(aiCharacterControl[6].aiShootTarget);
                }
                
                StartCoroutine(TasksToWait(0));

                break;
            
            case Scene.GangMemberRunsToCar:
                
                if ((player.transform.position - playerLocations[0].transform.position).magnitude < 0.8f)
                {
                    playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Exploration);
                    Time.timeScale = 0;
                    shootingTutorialUi.SetActive(true);
                    
                    // ENABLE UI FOR SHOOTING TUTORIAL
                    if (!textReader.alreadyInteracted)
                    {
                        textReader.alreadyInteracted = true;
                        textReader.textAsset = storyScripts[4];
                        textReader.LoadScript();
                        textReader.ToggleUI();
                        textReader.alreadyInteracted = false;

                        positionSet = false;
                        sceneNum++;
                    }
                }

                // if (Input.GetKeyDown(KeyCode.Escape))
                // {
                //     Time.timeScale = 1;
                // }
                
                break;
            
            case Scene.ShootingTutorial:
                
                if (textReader.alreadyInteracted)
                {
                    shootingTutorialUi.SetActive(false);
                    Time.timeScale = 1;
                }

                if (aiCharacterControl[6].aIStopped)
                {
                    interactables[0].enabled = false;
                    interactables[0].GetComponent<BoxCollider>().enabled = false;
                    carAnimator.SetBool("FLDoor", true);
                    aiCharacterControl[6].targetTransform = enemyLocations[1];
                    aiCharacterControl[6].crouch = true;
                    sceneNum++;
                }
                
                break;
            
            case Scene.QTE:
                
                if (aiCharacterControl[6].aIStopped)
                {
                    // aiCharacterControl[6].targetTransform = aiCharacterControl[6].transform;
                    // aiCharacterControl[6].enabled = false;
                    aiCharacterControl[6].transform.parent = carAnimator.transform;
                    aiCharacterControl[6].agent.enabled = false;
                    carAnimator.SetBool("FLDoor", false);
                    carAnimator.SetBool("Flee", true);
                    
                    // 3 seconds before setting to false and if player passes
                    StartCoroutine(TasksToWait(1));
                }
                
                
                // FAIL IF CAR GOES OUTSIDE
                // SHOOT THE CAR 3 TIMES TO GET THE CAR TO CRASH
                // IF PASS GANG MEMBER RUNS TOWARDS THE WAREHOUSE
                // PLAYER GOES BACK TO MEET PAULIE AND LUCA FOR GUNFIGHT

                break;
            
            case Scene.GangMemberRunsAway:
                
                carAnimator.SetBool("FLDoor", true);
                StartCoroutine(TasksToWait(2));

                break;
            
            case Scene.BackToFriends:

                if (aiCharacterControl[6].aIStopped)
                {
                    aiCharacterControl[6].gameObject.SetActive(false);
                }
                
                if (GameManager.IsInteracting)
                {
                    playerCharacterControl.weapon.gameObject.SetActive(false);
                    playerAnimator.SetBool("GunDrawn", false);
                    playerCharacterControl.currentInteractable.tag = "Interactable";
                    playerCharacterControl.currentInteractable.isOccupied = false;
                    GameManager.IsInteracting = false;
                }
                    
                playerCharacterControl.pointShootRig.weight = 0;
                playerCharacterControl.coverAnimRig.weight = 0;
                playerCharacterControl.characterMovement.run = true;
                playerCharacterControl.crouch = false;
                playerCharacterControl.targetTransform = playerLocations[1].transform;

                if (!playerCharacterControl.cachedTransform)
                {
                    playerCharacterControl.characterState = CharacterControl.CharacterState.Exploration;
                    // AUTOSAVE
                    // Transform tempTransform = interactables[0].transform;
                    // interactables[0].transform.position = carAnimator.transform.position;
                    // interactables[0].transform.rotation = carAnimator.transform.rotation;
                    PersistentSave.Save();
                    // interactables[0].transform.position = tempTransform.position;
                    // interactables[0].transform.rotation = tempTransform.rotation;
                    sceneNum++;
                }
                
                break;
            
            case Scene.Shootout:

                foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                {
                    if (aicharacterControl.aiBehaviour == AICharacterControl.InternalBehaviour.RandomLocationPicker)
                    {
                        aicharacterControl.aiBehaviour = AICharacterControl.InternalBehaviour.Chase;
                    }

                    // if (aicharacterControl.characterState == CharacterControl.CharacterState.None)
                    // {
                    //     b++;
                    //     print(b);
                    // }
                }
                
                int enemiesDead = 0;
                int friendlyDead = 0;

                if (playerCharacterControl.characterState == CharacterControl.CharacterState.None)
                {
                    failUI.SetActive(true);
                }
                
                for (int j = 0; j <= 1; j++)
                {
                    if (aiCharacterControl[j].characterState == CharacterControl.CharacterState.None)
                    {
                        print(aiCharacterControl[j].name);
                        GameManager.Intelligence++;
                        friendlyDead++;
                    }
                }
                
                for (int i = 3; i <= 5; i++)
                {
                    if (aiCharacterControl[i].characterState == CharacterControl.CharacterState.None)
                    {
                        enemiesDead++;

                        if (enemiesDead == 3)
                        {
                            if (friendlyDead > 0)
                            {
                                // FAIL
                                print($"{friendlyDead} : {enemiesDead}");
                                failUI.SetActive(true);
                                playerCharacterControl.characterState = CharacterControl.CharacterState.Cutscene;
                            }
                            else
                            {
                                // PASS
                                print("PASS!");
                                positionSet = false;
                                sceneNum++;
                            }
                        }
                    }
                }

                print($"{friendlyDead} : {enemiesDead}");
                
                // if (friendlyDead == 2)
                // {
                //     Time.timeScale = 0;
                // }
                
                

                break;
            
            case Scene.Warehouse:
                
                foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                {
                    if (aicharacterControl.isFriendly)
                    {
                        if (aicharacterControl.isInteracting)
                        {
                            aicharacterControl.pointShootRig.weight = 0;
                            aicharacterControl.anim.SetBool("GunDrawn", false);
                            aicharacterControl.currentInteractable.tag = "Interactable";
                            aicharacterControl.weapon.gameObject.SetActive(false);
                            aicharacterControl.currentInteractable.isOccupied = false;
                            aicharacterControl.characterState = CharacterControl.CharacterState.Exploration;
                            aicharacterControl.isInteracting = false;
                        }

                        aicharacterControl.characterMovement.run = false;
                        aicharacterControl.crouch = false;
                        aicharacterControl.aiBehaviour = AICharacterControl.InternalBehaviour.RandomLocationPicker;
                    }
                }

                
                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly && aicharacterControl.characterState != CharacterControl.CharacterState.None)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[4];
                            aicharacterControl.GetComponent<CharacterMovement>().run = true;
                        }
                    }
                    positionSet = true;
                }

                if (GameManager.Intelligence == 5)
                {
                    // ENABLE FINAL NOTE
                    doneUI.SetActive(true);
                }
                
                break;
        }
    }

    IEnumerator TasksToWait(int taskNum)
    {
        switch (taskNum)
        {
            case 0:

                yield return new WaitForSeconds(1f);
                aiCharacterControl[6].pointShootRig.weight = 0;
                aiCharacterControl[6].characterState = CharacterControl.CharacterState.Exploration;
                aiCharacterControl[6].GetComponent<CharacterMovement>().run = true;
                aiCharacterControl[6].targetTransform = enemyLocations[0];
                playerCharacterControl.targetTransform = playerLocations[0].transform;
                sceneNum = 7;
                if (!animators[2].enabled && aiCharacterControl[2].characterState == CharacterControl.CharacterState.None)
                {
                    animators[2].enabled = true;
                    animators[2].SetTrigger("Dead");
                }
                
                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly && aicharacterControl.characterState != CharacterControl.CharacterState.None)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[3];
                            aicharacterControl.GetComponent<CharacterMovement>().run = true;
                        }
                    }
                    positionSet = true;
                    // friendlyLocationsCount++;
                }

                yield return new WaitForSeconds(0.3f);
                player.GetComponent<CharacterMovement>().run = true;

                break;
            
            case 1:

                yield return new WaitForSeconds(4f);
                if (unlockables[1].unlocked)
                {
                    carAnimator.SetBool("Flee", false);
                    aiCharacterControl[6].weapon.gameObject.SetActive(false);
                    aiCharacterControl[6].weapon = null;
                    sceneNum = 10;
                    StopAllCoroutines();
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    cinemachineTarget.RemoveMember(aiCharacterControl[6].transform);
                    
                    // FAIL SCREEN
                    failUI.SetActive(true);
                    Time.timeScale = 0;
                }

                break;
            
            case 2:

                yield return new WaitForSeconds(1.5f);
                aiCharacterControl[6].agent.enabled = true;
                aiCharacterControl[6].transform.parent = null;
                aiCharacterControl[6].targetTransform = enemyLocations[2];
                aiCharacterControl[6].crouch = false;
                aiCharacterControl[6].GetComponent<CharacterMovement>().run = true;
                
                yield return new WaitForSeconds(4f);
                cinemachineTarget.RemoveMember(aiCharacterControl[6].transform);
                playerCharacterControl.characterState = CharacterControl.CharacterState.Cutscene;
                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly && aicharacterControl.characterState != CharacterControl.CharacterState.None)
                        {
                            aicharacterControl.targetTransform = playerLocations[1];
                            aicharacterControl.GetComponent<CharacterMovement>().run = true;
                            aicharacterControl.characterState = CharacterControl.CharacterState.Exploration;
                        }
                    }
                    positionSet = true;
                    // friendlyLocationsCount++;
                }
                characters[3].SetActive(true);
                characters[4].SetActive(true);
                characters[5].SetActive(true);
                
                sceneNum = 11;

                break;
            
            default:

                yield return null;
                
                break;
        }
    }

    public void UnlockLocation(int unlockLocationNum, int doorsUnlocked)
    {
        if (!unlockLocations[unlockLocationNum].materialFade.enabled)
        {
            unlockLocations[unlockLocationNum].materialFade.enabled = true;
        }

        if (doorsUnlocked >= 0)
        {
            for (int j = 0; j <= doorsUnlocked; j++)
            {
                if (!unlockLocations[unlockLocationNum].unlocked[j])
                {
                    unlockLocations[unlockLocationNum].doors[j].SetBool(open, true);
                    unlockLocations[unlockLocationNum].unlocked[j] = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (GameObject go in ai)
            {
                go.SetActive(true);
            }
        }
        
        // if (Input.GetMouseButtonDown(1))
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // }

        // if (GameManager.DoneLoadingChapter)
        {
            SceneFlow();
        }
    }

    private void OnDestroy()
    {
        TextReader.Unlock -= ChoiceTaken;
        InspectableInteractables.Unlock -= ChoiceTaken;
        Time.timeScale = 1;
    }
}
