using UnityEngine;

public class Chapter2 : Chapters
{
    [SerializeField] private GameObject[] ai;
    [SerializeField] private Interactable[] interactables;
    [SerializeField] private TextAsset[] storyScripts;
    [SerializeField] private int storyScriptNum;
    [SerializeField] private Transform[] friendlyLocations;
    [SerializeField] private int friendlyLocationsCount;
        
    [SerializeField] private TextAsset receptionistDialogue2;
    
    public enum Scene
    {
        InitialExploration,
        ReceptionistDialogue,
        MiniGame,
        MoeDoor,
        Moe,
        GangMemberKillsMoe,
        GangMemberRunsToCar,
        ShootingTutorial,
        
    }

    public Scene scene = Scene.InitialExploration;

    public void SceneFlow()
    {
        scene = (Scene)sceneNum;
        switch (scene)
        {
            case Scene.InitialExploration:

                foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                {
                    if (aicharacterControl.isFriendly)
                    {
                        aicharacterControl.targetTransform = friendlyLocations[friendlyLocationsCount];
                    }
                }
                friendlyLocationsCount++;
                
                textReader.textAsset = storyScripts[storyScriptNum];
                textReader.LoadScript();
                textReader.ToggleUI();
                storyScriptNum++;
                sceneNum++;
                
                break;
            
            case Scene.ReceptionistDialogue:

                if (textReader.dialogueTracker == 0)
                {
                    foreach (AICharacterControl aicharacterControl in aiCharacterControl)
                    {
                        if (aicharacterControl.isFriendly)
                        {
                            aicharacterControl.targetTransform = friendlyLocations[friendlyLocationsCount];
                        }
                    }
                }

                break;
            
            
            case Scene.MiniGame:
                
                // MINIGAME
                
                // AFTER GAME DIALOGUE

                if (interactables[3].GetComponent<TextReader>().alreadyInteracted)
                {
                    textReader.textAsset = receptionistDialogue2;
                    textReader.LoadScript();
                    textReader.ToggleUI();
                    sceneNum++;
                }

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
