using System;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// LOAD dialogues as single lines
/// Add audio clip to respective character dialogue
/// Format the text before displaying it as UI
/// </summary>

// DO NOT ENABLE UNLESS YOU WANT TO MANUALLY CLEANUP LOADED SCRIPT VARIABLES (LOADED BEFORE RUNTIME) EDIT: RUN UnloadScript() TO UNDO THIS
// [ExecuteInEditMode]
public class TextReader : MonoBehaviour
{
    // CHAPTER VARIABLES
    [SerializeField] private GameObject chapterManager;
    private ChapterDialogueAudioManager _chapterDialogueAudioManager;
    public DialogueAudioMatch[] dialogueAudioMatch;

    // UI VARIABLES
    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    // CURRENT INTERACTION STATE
    public InteractableStates interactableStates = InteractableStates.NotInteracted;
    
    // TEXT BUILDING VARIABLES
    public TextAsset textAsset;
    public string[] lines;
    public string[] currentDialogue;
    public string currentSpeaker;
    private StringBuilder strB;
    private StringBuilder lineAdd = new();

    public bool nextDialogue;
    // public bool dialogueOver;

    public int dialogueTracker;

    // DIALOGUE SKIP VARIABLES
    private Timer dialogueSkipTimer;
    public float skipDelay = 1;

    private Interactable interactable;
    public bool alreadyInteracted;

    // ACTIONS RELATED TO EXTERNAL SCRIPTS
    public static Action<Transform> RemoveCinemachineTarget;
    public static Action<SpeakerEnum, AudioClip> SetDialogueAudio;

    private void Awake()
    {
        if (!gameObject.GetComponent<Timer>())
        {
            dialogueSkipTimer = gameObject.AddComponent<Timer>();
        }
        else
        {
            dialogueSkipTimer = gameObject.GetComponent<Timer>();
        }
        
        _chapterDialogueAudioManager = chapterManager.GetComponent<ChapterDialogueAudioManager>();
        LoadScript();

        interactable = GetComponent<Interactable>();

        //USE LOAD ASYNC
        //Call from another script when player is in proximity of someone who has a dialogue
        // ta = Resources.Load<TextAsset>("a");
    }

    // Update is called once per frame
    void Update()
    {
        if (interactableStates == InteractableStates.Interacting)
        {
            ParseUI();
            PlayerInput();
        }
    }
    
    private void ParseUI()
    {
        if (nextDialogue)
        {
            for(int i = dialogueTracker; i < lines.Length; i++)
            {
                // print(i);

                // CheckSpeaker(lines, i, textAsset.name);
                SetDialogueAudio(dialogueAudioMatch[i].speakerId, dialogueAudioMatch[i].dialogueAudio);

                // if (lines[i].Contains(";"))
                {
                    currentDialogue = lines[i].Split("$", StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in currentDialogue)
                    {
                        lineAdd.AppendLine(s);
                    }

                    // print(lines[i]);
                    nextDialogue = false;
                    dialogueTracker = i + 1;
                    
                    if (dialogueTracker == lines.Length)
                    {
                        nextDialogue = false;
                        dialogueTracker = -1;
                    }
                }
                
                dialogueText.text = lineAdd.ToString();
                
                if(!nextDialogue)
                {
                    lineAdd.Clear();
                    break;
                }
            }
        }
    }
    
    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // print(interactableStates.ToString());

            if (interactableStates == InteractableStates.InteractionOver)
            {
                ToggleUI();
            }

            NextDialogue();
        }

        // LONG PRESS SPACE TO SKIP
        else if (Input.GetKey(KeyCode.Space))
        {
            // print("SPACE HOLD");

            if (!dialogueSkipTimer.isRunning)
            {
                GameManager.dialogueSkipDelay = skipDelay;
                dialogueSkipTimer.StartTimer(GameManager.dialogueSkipDelay);
            }
            else if (dialogueSkipTimer.isCompleted)
            {
                EndDialogue();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // print("SPACE UP");

            dialogueSkipTimer.StopTimer();
        }
    }

    // private void CheckSpeaker(string[] lines, int i, string script)
    // {
    //     if (lines[i].Contains(":"))
    //     {
    //         currentSpeaker = lines[i].Split(":");
    //         print(currentSpeaker[0]);
    //         
    //         //SET CURRENT SPEAKER INFO
    //         SetDialogueAudio(currentSpeaker[0], script);
    //     }
    // }

    public void ToggleUI()
    {
        // LoadScript();
        
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            NextDialogue();
        }
    }

    public void NextDialogue()
    {
        if (dialogueTracker == -1)
        {
            interactableStates = InteractableStates.InteractionOver;
            if (GameManager.IsInteracting)
            {
                // print(gameObject.transform);
                RemoveCinemachineTarget(interactable.targetLocation.transform);
            }

            if (interactable)
            {
                // ADDS INTEL ONLY IF THE PLAYER HASNT ALREADY INTERACTED WITH THIS OBJECT AND NOT SKIPPED THE DIALOGUE
                if (!alreadyInteracted && !dialogueSkipTimer.isCompleted)
                {
                    GameManager.Intelligence = interactable.minIntel + interactable.rewardIntel;
                    alreadyInteracted = true;
                }
                
                interactable.isOccupied = false;
                GameManager.IsInteracting = false;
            }
            
            currentDialogue = null;
            dialogueText.text = "";
            dialogueTracker = 0;

            ToggleUI();
            // textAsset = null;
        }
        else
        {
            // dialogueOver = false;
            dialogueSkipTimer.isCompleted = false;
            nextDialogue = true;
            
            interactableStates = InteractableStates.Interacting;
        }
    }

    public void LoadScript()
    {
        // RESETS TIMER
        // dialogueSkipTimer.isCompleted = false;

        strB = new StringBuilder(textAsset.text);
        lines = strB.ToString().Split("\n");
        
        dialogueAudioMatch = new DialogueAudioMatch[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            dialogueAudioMatch[i] = new DialogueAudioMatch();
            dialogueAudioMatch[i].dialogueLine = lines[i];
            
            string scriptName = textAsset.name + "_" + i;
            dialogueAudioMatch[i].dialogueAudio = Array.Find(_chapterDialogueAudioManager.sceneCharactersAudio, p => p.name == scriptName);
            
            currentSpeaker = lines[i].Split(":")[0];

            switch (currentSpeaker)
            {
                case "Mariano":
                    dialogueAudioMatch[i].speakerId = SpeakerEnum.Mariano;
                    break;
            
                case "Luca":
                    dialogueAudioMatch[i].speakerId = SpeakerEnum.Luca;
                    break;
            
                case "Paulie":
                    dialogueAudioMatch[i].speakerId = SpeakerEnum.Paulie;
                    break;
            
                case "Stripper":
                    dialogueAudioMatch[i].speakerId = SpeakerEnum.Stripper;
                    break;
            }

        }
    }

    public void EndDialogue()
    {
        dialogueTracker = -1;
        NextDialogue();
    }

    // UNLOADS THE SCRIPT FROM MEMORY
    public void UnloadScript()
    {
        currentSpeaker = "";
        strB = new StringBuilder();
        lines = Array.Empty<string>();
        currentDialogue = Array.Empty<string>();
        dialogueAudioMatch = Array.Empty<DialogueAudioMatch>();
    }
}