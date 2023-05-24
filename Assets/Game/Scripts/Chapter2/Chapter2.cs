using UnityEngine;

public class Chapter2 : Chapters
{
    [SerializeField] private GameObject[] ai;
    [SerializeField] private Interactable[] interactables;
    [SerializeField] private TextReader[] interactablesTextReader;
    [SerializeField] private TextAsset[] storyScripts;
    [SerializeField] private int storyScriptNum;
    [SerializeField] private Transform[] friendlyLocations;
    [SerializeField] private int friendlyLocationsCount;
        
    [SerializeField] private TextAsset receptionistDialogue2;

    [SerializeField] private GameObject minigame;

    public bool positionSet;
    public bool scriptSet;
    public bool knows;
    
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

    protected override void Awake()
    {
        base.Awake();

        interactablesTextReader = new TextReader[interactables.Length];
        for (int i = 0; i < interactables.Length - 1; i++)
        {
            interactablesTextReader[i] = interactables[i].GetComponent<TextReader>();
        }
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
                    textReader.alreadyInteracted = false;
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
                    friendlyLocationsCount++;
                }

                if (interactables[3].isOccupied)
                {
                    textReader.alreadyInteracted = false;
                    scriptSet = false;
                    positionSet = false;
                    sceneNum++;
                }

                break;
            
            case Scene.PreMiniGameDialogue:
                
                if (knows)
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
        
        SceneFlow();
    }
}
