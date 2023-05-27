using System;
using UnityEngine;

public class Chapter2 : Chapters
{
    [SerializeField] private GameObject[] ai;
    [SerializeField] public Interactable[] interactables;
    [SerializeField] private TextReader[] interactablesTextReader;
    [SerializeField] private TextAsset[] storyScripts;
    public int storyScriptNum;
    [SerializeField] private Transform[] friendlyLocations;
    public int friendlyLocationsCount;
    
    [SerializeField] private GameObject minigame;
    public bool positionSet;
    public bool scriptSet;
    
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
    
    public enum Scene
    { 
        InitialExploration,
        ReceptionistDialogue,
        PreMiniGameDialogue,
        MiniGame,
        PostMiniGameDialogue,
        MoeDoor,
        Moe,
        GangMemberKillsMoe,
        GangMemberRunsToCar,
        ShootingTutorial,
        
    }

    public Scene scene = Scene.InitialExploration;
    private readonly int open = Animator.StringToHash("Open");

    protected override void Awake()
    {
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

                GameManager.IsInteracting = true;
                if (!positionSet)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[friendlyLocationsCount];
                        }
                    }
                    positionSet = true;
                    friendlyLocationsCount++;
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
                            aicharacterControl.targetTransform = friendlyLocations[friendlyLocationsCount];
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
                print(minigame.activeSelf);
                if (!minigame.activeSelf)
                {
                    sceneNum++;
                    playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Exploration);
                }
                // AFTER GAME DIALOGUE

                break;
            
            case Scene.PostMiniGameDialogue:

                // STORY KNOWS ABOUT MAN BEHIND THE COUNTER
                if (unlockables[0].unlocked && !scriptSet)
                {
                    textReader.textAsset = storyScripts[2];
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    // storyScriptNum++;
                    scriptSet = true;
                }

                if (textReader.alreadyInteracted)
                {
                    // textReader.alreadyInteracted = false;
                    scriptSet = false;
                }

                // STORY RECEPTIONIST DIALOGUE
                if (!scriptSet)
                {
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
                
                if (interactables[4].GetComponent<TextReader>().alreadyInteracted)
                {
                    sceneNum++;
                }
                
                break;
            
            case Scene.GangMemberKillsMoe:
                
                playerCharacterControl.ChangeCharacterState(CharacterControl.CharacterState.Cutscene);
                // playerCharacterControl.targetTransform = characters[1].transform;
                
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

        if (GameManager.DoneLoadingChapter)
        {
            SceneFlow();
        }
    }

    private void OnDestroy()
    {
        TextReader.Unlock -= ChoiceTaken;
        InspectableInteractables.Unlock -= ChoiceTaken;
    }
}
