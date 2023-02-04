using System;
using System.Text;
using TMPro;
using UnityEngine;

//Get dialogues as single lines
//Add audio clip to respective character dialogue
//Format the text before displaying it as UI

[ExecuteInEditMode]
public class TextReader : MonoBehaviour
{
    [SerializeField] private GameObject chapter1;
    private Chapter1DialogueAudioManager chapter1DialogueAudioManager;
    public DialogueAudioMatch[] dialogueAudioMatch;

    // Create variables if player is already in conversation with someone or examining

    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;


    public InteractableStates interactableStates = InteractableStates.NotInteracted;
    
    public TextAsset textAsset;
    public string[] lines;
    public string[] currentDialogue;
    public string currentSpeaker;
    public string text;
    public StringBuilder strB;
    StringBuilder lineAdd = new();

    public bool nextDialogue;
    // public bool dialogueOver;

    public int dialogueTracker;

    private Timer dialogueSkipTimer;
    public float skipDelay;

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
        
        chapter1DialogueAudioManager = chapter1.GetComponent<Chapter1DialogueAudioManager>();

        //USE LOAD ASYNC
        //Call from another script when player is in proximity of someone who has a dialogue
        // ta = Resources.Load<TextAsset>("a");
    }

    // Start is called before the first frame update
    void Start()
    {
        // text = ta.text;
        // strB = new StringBuilder(ta.text);
        // // text = strB.ToString();
        // lines = strB.ToString().Split("\n");
    }

    // Update is called once per frame
    void Update()
    {
        if (interactableStates == InteractableStates.Interacting)
        {
            ParseUI();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (interactableStates == InteractableStates.InteractionOver)
            {
                ToggleUI();
            }
            NextDialogue();
        }
        
        // LONG PRESS SPACE TO SKIP
        else if (Input.GetKey(KeyCode.Space))
        {
            if (!dialogueSkipTimer.isRunning)
            {
                GameManager.dialogueSkipDelay = skipDelay;
                dialogueSkipTimer.StartTimer(GameManager.dialogueSkipDelay);
                
            }
            else if (dialogueSkipTimer.isCompleted)
            {
                dialogueTracker = -1;
                NextDialogue();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
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
        LoadScript();
        
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            NextDialogue();
        }
    }

    public void NextDialogue()
    {
        if(dialogueTracker == -1)
        {
            interactableStates = InteractableStates.InteractionOver;
            if (GameManager.isInteracting)
            {
                print(gameObject.transform);
                RemoveCinemachineTarget(gameObject.GetComponent<Interactable>().targetLocation.transform);
            }
            GameManager.isInteracting = false;
            currentDialogue = null;
            dialogueText.text = "";
            dialogueTracker = 0;

            ToggleUI();
            // textAsset = null;
        }
        else
        {
            // dialogueOver = false;
            nextDialogue = true;
            
            interactableStates = InteractableStates.Interacting;
        }
    }

    private void LoadScript()
    {
        text = textAsset.text;
        strB = new StringBuilder(textAsset.text);
        // text = strB.ToString();
        lines = strB.ToString().Split("\n");
        
        dialogueAudioMatch = new DialogueAudioMatch[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            dialogueAudioMatch[i] = new DialogueAudioMatch();
            dialogueAudioMatch[i].dialogueLine = lines[i];
            
            string scriptName = textAsset.name + "_" + i;
            dialogueAudioMatch[i].dialogueAudio = Array.Find(chapter1DialogueAudioManager.sceneCharactersAudio, p => p.name == scriptName);
            
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

    // private void OnDisable()
    // {
    //     // dialogueSkipTimer
    //     DestroyImmediate(dialogueSkipTimer);
    // }
}