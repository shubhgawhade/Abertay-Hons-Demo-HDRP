using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // GENERAL SAVE VARIABLES
    public int currentscene;
    public int intelligence;

    // CHAPTERS MANAGER SAVE VARIABLES
    public int sceneNum;
    public SaveObject player;
    
    // CHAPTER 1 MANAGER SAVE VARIABLES
    // public <T>

    // CHAPTER 2 MANAGER SAVE VARIABLES
    public SaveObject[] characters;
    public int storyScriptNum;
    public bool[] alreadyinteracted;
    public AICharacterControl[] aiCharacterControl;
    public int friendlyLocationsCount;
    public bool positionSet;
    public bool scriptSet;
    public Chapter2.Unlockables[] unocklabes;
    public Chapter2.UnlockableLocations[] unlockLocations;

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
            
            // SWITCH TO THE CURRENT CHAPTER DATA TO SAVE
            switch (GameManager.ChaptersManager)
            {
                case Chapter1:
                    Chapter1 ch1M = GameManager.Chapter1Manager;

                    break;
                
                case Chapter2:

                    Chapter2 ch2M = GameManager.Chapter2Manager;
                    
                    characters = new SaveObject[ch2M.characters.Length];
                    for (int i = 0; i < ch2M.characters.Length; i++)
                    {
                        characters[i] = new SaveObject
                        {
                            enabled = ch2M.characters[i].activeSelf,
                            position = ch2M.characters[i].transform.position,
                            rotation = ch2M.characters[i].transform.eulerAngles
                        };
                        // Debug.Log(ch2M.characters[i].transform.position);
                    }
                    
                    alreadyinteracted = new bool[ch2M.interactables.Length];
                    for (int i = 0; i < alreadyinteracted.Length; i++)
                    {
                        alreadyinteracted[i] = ch2M.interactables[i].alreadyInteracted;
                    }
                    
                    storyScriptNum = ch2M.storyScriptNum;
                    friendlyLocationsCount = ch2M.friendlyLocationsCount;
                    unocklabes = ch2M.unlockables;
                    // positionSet = ch2.positionSet;
                    // positionSet = ch2.positionSet;
                    // scriptSet = ch2.scriptSet;
                    unlockLocations = ch2M.unlockLocations;
                    
                    break;
            }
        }

        intelligence = GameManager.Intelligence;
        // saveObject = new SaveObject[]
    }
}

[Serializable]
public struct SaveObject
{
    public bool enabled;
    public Vector3 position;
    public Vector3 rotation;
}
