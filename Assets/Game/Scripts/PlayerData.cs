using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public class SaveObject
    {
        public bool enabled;
        public Vector3 position;
        public Vector3 rotation;
    }
    
    [Serializable]
    public class CharacterSaveObject : SaveObject
    {
        public int characterState;
        public int internalAIState;
    }

    [Serializable]
    public class InteractableSaveObject : SaveObject
    {
        public bool alreadyInteracted;
    }
    
    // GENERAL SAVE VARIABLES
    public int currentscene;
    public int intelligence;

    // CHAPTERS MANAGER SAVE VARIABLES
    public int sceneNum;
    public SaveObject player;
    public float drunkenness;
    
    // CHAPTER 1 MANAGER SAVE VARIABLES
    // public <T>

    // CHAPTER 2 MANAGER SAVE VARIABLES
    public CharacterSaveObject[] characters;
    public int storyScriptNum;
    public InteractableSaveObject[] interactableSaveObjects;
    public AICharacterControl[] aiCharacterControl;
    // public int friendlyLocationsCount;
    public bool positionSet;
    public bool scriptSet;
    public Chapter2.Unlockables[] unocklabes;
    public UnlockableLocationsSave[] unlockLocations;
    
    [Serializable]
    public struct UnlockableLocationsSave
    {
        public bool[] unlocked;
    }

    public PlayerData()
    {
        currentscene = GameManager.CurrentScene;
        if (GameManager.ChaptersManager)
        {
            // GET CHAPTERS MANAGER SAVE VARIABLES
            Chapters chM = GameManager.ChaptersManager;
            sceneNum = chM.sceneNum;
            player = new SaveObject
            {
                position = chM.player.transform.position,
                rotation = chM.player.transform.eulerAngles
            };

            drunkenness = GameManager.Drunkenness;
            
            // SWITCH TO THE CURRENT CHAPTER DATA TO SAVE
            switch (GameManager.ChaptersManager)
            {
                case Chapter1:
                    Chapter1 ch1M = GameManager.Chapter1Manager;

                    break;
                
                case Chapter2:

                    Chapter2 ch2M = GameManager.Chapter2Manager;
                    
                    characters = new CharacterSaveObject[ch2M.characters.Length];
                    for (int i = 0; i < ch2M.characters.Length; i++)
                    {
                        characters[i] = new CharacterSaveObject
                        {
                            characterState = (int)ch2M.aiCharacterControl[i].characterState,
                            internalAIState = (int) ch2M.aiCharacterControl[i].aiBehaviour,
                            enabled = ch2M.characters[i].activeSelf,
                            position = ch2M.characters[i].transform.position,
                            rotation = ch2M.characters[i].transform.eulerAngles
                        };
                        // Debug.Log(ch2M.characters[i].transform.position);
                    }
                    
                    interactableSaveObjects = new InteractableSaveObject[ch2M.interactables.Length];
                    for (int i = 0; i < interactableSaveObjects.Length; i++)
                    {
                        interactableSaveObjects[i] = new InteractableSaveObject()
                        {
                            enabled = ch2M.interactables[i].gameObject.activeSelf,
                            position = ch2M.interactables[i].transform.position,
                            rotation = ch2M.interactables[i].transform.eulerAngles,
                            alreadyInteracted = ch2M.interactables[i].alreadyInteracted
                        };
                    }
                    
                    storyScriptNum = ch2M.storyScriptNum;
                    // friendlyLocationsCount = ch2M.friendlyLocationsCount;
                    unocklabes = ch2M.unlockables;
                    // positionSet = ch2.positionSet;
                    // positionSet = ch2.positionSet;
                    // scriptSet = ch2.scriptSet;
                    unlockLocations = new UnlockableLocationsSave[ch2M.unlockLocations.Length];
                    for (int i = 0; i < unlockLocations.Length; i++)
                    {
                        unlockLocations[i].unlocked = ch2M.unlockLocations[i].unlocked;
                    }
                    
                    break;
            }
        }

        intelligence = GameManager.Intelligence;
        // saveObject = new SaveObject[]
    }
}