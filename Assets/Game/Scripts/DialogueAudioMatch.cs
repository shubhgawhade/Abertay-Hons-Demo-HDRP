using System;
using UnityEngine;

[Serializable]
public class DialogueAudioMatch
{
        public string name;
        public string dialogueLine;
        public AudioClip dialogueAudio;
        public SpeakerEnum speakerId;
}
