using System;
using System.Text;
using UnityEngine;

// [ExecuteInEditMode]
public class AudioDialogueTest : MonoBehaviour
{
    [SerializeField] private AudioClip[] ac;

    public TextAsset textAsset;
    public string[] lines;
    public DialogueAudioMatch[] dialogueAudioMatch;
    public string currentDialogue;
    public string currentSpeaker;
    public string text;
    public StringBuilder strB;
    StringBuilder lineAdd = new();
    public int dialogueTracker;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
            dialogueAudioMatch[i].dialogueAudio = Array.Find(ac, p => p.name == scriptName);
            
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
}
