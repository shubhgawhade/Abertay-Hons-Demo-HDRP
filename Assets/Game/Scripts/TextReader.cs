using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TextReader : MonoBehaviour
{
    // Create variables if player is already in conversation with someone or examining

    [SerializeField] private GameObject ui;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    
    public string[] lines;
    public string[] a;
    public TextAsset ta;
    public string text;
    public StringBuilder strB;
    StringBuilder lineAdd = new();

    public bool nextDialogue;
    public bool dialogueOver;

    public int dialogueTracker;
    private void Awake()
    {
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
        ParseUI();
    }

    private void ParseUI()
    {
        if (nextDialogue)
        {
            for(int i = dialogueTracker; i < lines.Length; i++)
            {
                print(i);

                if (lines[i].Contains(";"))
                {
                    a = lines[i].Split(";", StringSplitOptions.RemoveEmptyEntries);
                    lineAdd.AppendLine(a[0]);

                    print(lines[i]);
                    nextDialogue = false;
                    dialogueTracker = i + 1;
                    
                    if (dialogueTracker == lines.Length)
                    {
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
            NextDialogue();
        }
    }

    public void ToggleUI()
    {
        text = ta.text;
        strB = new StringBuilder(ta.text);
        // text = strB.ToString();
        lines = strB.ToString().Split("\n");
        ui.SetActive(!ui.activeSelf);
        
        NextDialogue();
    }

    public void NextDialogue()
    {
        if (dialogueTracker == -1)
        {
            dialogueTracker = 0;
            nextDialogue = false;
            dialogueOver = true;
            ToggleUI();
        }
        else
        {
            dialogueOver = false;
            nextDialogue = true;
        }
    }
}