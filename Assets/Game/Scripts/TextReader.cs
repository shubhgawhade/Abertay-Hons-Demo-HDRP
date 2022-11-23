using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TextReader : MonoBehaviour
{
    // Create variables if player is already in conversation with someone or examining

    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;


    public InteractableStates interactableStates = InteractableStates.NotInteracted;
    
    public TextAsset textAsset;
    public string[] lines;
    public string[] currentDialogue;
    public string text;
    public StringBuilder strB;
    StringBuilder lineAdd = new();

    public bool nextDialogue;
    // public bool dialogueOver;

    public int dialogueTracker;

    private Timer dialogueSkipTimer;
    public float skipDelay;
    private void Awake()
    {
        dialogueSkipTimer = gameObject.AddComponent<Timer>();
        
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

                if (lines[i].Contains(";"))
                {
                    currentDialogue = lines[i].Split(";", StringSplitOptions.RemoveEmptyEntries);
                    lineAdd.AppendLine(currentDialogue[0]);

                    // print(lines[i]);
                    nextDialogue = false;
                    dialogueTracker = i + 1;
                    
                    if (dialogueTracker == lines.Length)
                    {
                        nextDialogue = false;
                        dialogueTracker = -1;
                    }
                }
                else
                {
                    lineAdd.AppendLine(lines[i]);
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
            GameManager.isInteracting = false;
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
    }
}