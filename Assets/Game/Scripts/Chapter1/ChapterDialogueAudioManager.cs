using System;
using UnityEngine;


// [ExecuteInEditMode]
public class ChapterDialogueAudioManager : MonoBehaviour
{
    public int previousSpeaker;
    [SerializeField] private AudioSource[] charactersAudioSource;
    public AudioClip[] sceneCharactersAudio;
    private bool ran;

 
    private void Awake()
    {
        TextReader.SetDialogueAudio += SetDialogueAudio;
    }

    private void SetDialogueAudio(SpeakerEnum currentSpeaker, AudioClip clip)
    {

        int tempSpeaker = (int)currentSpeaker;
        try
        {
            charactersAudioSource[tempSpeaker].clip = clip;
        }
        catch (Exception e)
        {
            print(currentSpeaker + " doesnt have enough dialogue");
            throw;
        }

        if (charactersAudioSource[previousSpeaker].isPlaying)
        {
            charactersAudioSource[previousSpeaker].Stop();
        }
        

        previousSpeaker = tempSpeaker;
        charactersAudioSource[tempSpeaker].Play();
    }
}
