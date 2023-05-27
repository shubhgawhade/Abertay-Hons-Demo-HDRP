using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool hasSave { get; set; }
    
    // public int currentScene;
    public static int CurrentScene{ get; set; }
    
    // public bool isMoveable;
    // public static bool IsMoveable;
    public static bool IsInteracting{ get; set; }
    // public static string CurrentSpeaker;
    
    // public int health;
    public static float PlayerHealth;

    // public int intelligence{ get; set; }
    public static int Intelligence{ get; set; }

    // public bool testRay;
    public static bool TestRay;

    public static bool DoneLoadingChapter;
    public static Chapters ChaptersManager{ get; set; }
    public static Chapter1 Chapter1Manager{ get; set; }
    public static Chapter2 Chapter2Manager{ get; set; }

    // public static float DialogueSKipDelay;
    [SerializeField] public static float dialogueSkipDelay { get; set; }

    private PlayerData data;
    
    private void Awake()
    {
        data = null;
        data = PersistentSave.Load();
        if (hasSave)
        {
            CurrentScene = data.currentscene;
            Intelligence = data.intelligence;
        }
        else
        {
            CurrentScene = 2;
        }
        
        // IsMoveable = isMoveable;
        // Intelligence = intelligence;

        Chapters.SceneActive += LoadSceneData;
        
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            PersistentSave.Save();
        }


        // print($"PLAYER HEALTH: {PlayerHealth}");
        // print($" IS INTERACTING : {IsInteracting}");
        // intelligence = Intelligence;
        // TestRay = testRay;
        // currentScene = CurrentScene;
        // print("IS INTERACTING: " + isInteracting);
    }

    // WAIT FOR SCENE TO LOAD BEFORE LOADING THE REMAINING DATA
    private void LoadSceneData()
    {
        print($"HAS SAVE : {hasSave}");
        // print(data.player.position);
        if (hasSave)
        {
            ChaptersManager.sceneNum = data.sceneNum;
            ChaptersManager.player.SetActive(false);
            ChaptersManager.player.transform.position = data.player.position;
            ChaptersManager.player.transform.eulerAngles = data.player.rotation;
            ChaptersManager.player.SetActive(true);

            switch (ChaptersManager)
            {
                case Chapter1:
                    
                    //
                    
                    break;
                
                case Chapter2:

                    for (int i = 0; i < Chapter2Manager.characters.Length; i++)
                    {
                        bool isActive = false;

                        // Chapter2Manager.characters[i].SetActive(data.characters[i].enabled);
                        if (data.characters[i].enabled)
                        {
                            isActive = true;
                            Chapter2Manager.characters[i].SetActive(false);
                        }
                        Chapter2Manager.characters[i].transform.position = data.characters[i].position;
                        Chapter2Manager.characters[i].transform.eulerAngles = data.characters[i].rotation;

                        if (isActive)
                        {
                            Chapter2Manager.characters[i].SetActive(true);
                        }
                    }
                    
                    for (int i = 0; i < Chapter2Manager.interactables.Length; i++)
                    {
                        Chapter2Manager.interactables[i].alreadyInteracted = data.alreadyinteracted[i];
                    }
                    
                    Chapter2Manager.storyScriptNum = data.storyScriptNum;
                    Chapter2Manager.friendlyLocationsCount = data.friendlyLocationsCount;
                    Chapter2Manager.unlockables = data.unocklabes;
                    // Chapter2Manager.positionSet = data.positionSet;
                    // Chapter2Manager.scriptSet = data.scriptSet;
                    // FADE OUT AREAS
                    // FIND HOW MANY DOORS ARE UNLOCKED AND SEND THAT NUMBER TO CH2M]
                    int doorsUnlocked = -1;
                    for (int i = 0; i < Chapter2Manager.unlockLocations.Length; i++)
                    {
                        for (int j = 0; j < data.unlockLocations[i].unlocked.Length; j++)
                        {
                            if (data.unlockLocations[i].unlocked[j])
                            {
                                doorsUnlocked++;
                            }
                        }

                        if (doorsUnlocked >= 0)
                        {
                            print($"{i} : {doorsUnlocked}");
                            Chapter2Manager.UnlockLocation(i, doorsUnlocked);
                        }
                    }
                    // Chapter2Manager.unlockLocations = data.unlockLocations;
                    

                    break;
            }

            DoneLoadingChapter = true;
        }
        else
        {
            DoneLoadingChapter = true;
        }
    }

    private void OnDisable()
    {
        Chapters.SceneActive -= LoadSceneData;
        // data = null;
        // ChaptersManager = null;
        // Chapter1Manager = null;
        // Chapter2Manager = null;
    }
}
