using System;
using UnityEngine;


// [ExecuteInEditMode]
public class Chapter1DialogueAudioManager : MonoBehaviour
{
    public int speaker;
    public int previousSpeaker;
    [SerializeField] private AudioSource[] charactersAudioSource;
    [SerializeField] private int[] charactersClipNum;


    public AudioClip[] sceneCharactersAudio;
    [SerializeField] private AudioClip[] marianoAudio;
    [SerializeField] private AudioClip[] lucaAudio;
    [SerializeField] private AudioClip[] paulieAudio;
    [SerializeField] private AudioClip[] stripperAudio;

    private bool ran;

 
    private void Awake()
    {
        charactersClipNum = new int[charactersAudioSource.Length];
        TextReader.SetDialogueAudio += SetDialogueAudio;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        // if (currentSpeaker == "Luca")
        // {
        //     ran = true;
        //     print("SPEAK");
        // charactersAudioSource[1].clip = lucaAudio[0];
        // charactersAudioSource[1].Play();
        // }
    }
}
