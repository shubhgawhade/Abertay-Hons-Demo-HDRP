using System;
using UnityEngine.Audio;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgAudioManager : MonoBehaviour
{
    public static BgAudioManager instance;
    public Sound[] bgMusic;

    //public Sound[] sounds;
    public int prevVal;
    public int chooseTrack;

    // public string a;


    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // foreach (Sound s in bgMusic)
        // {
        //     s.source = gameObject.AddComponent<AudioSource>();
        //     s.source.clip = s.clip;
        //     s.source.volume = s.volume;
        //     s.source.pitch = s.pitch;
        //     s.source.loop = s.loop;
        //     s.source.playOnAwake = s.playOnAwake;
        //     s.source.spatialBlend = s.spatialBlend;
        // }

        /*
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.spatialBlend = s.spatialBlend;
        }
        */
    }

    private void Start()
    {

        chooseTrack = UnityEngine.Random.Range(0, bgMusic.Length);
        prevVal = chooseTrack;
        //print("track chosen: " + chooseTrack);
        // a = bgMusic[chooseTrack].name;

        //Play("BgMusic");
    }

    private void Update()
    {
        //print(sounds.Length);
        
        // print(bgMusic[chooseTrack].source.isPlaying);
        if (!audioSource.isPlaying)
        {
            do 
            {
                chooseTrack = UnityEngine.Random.Range(0, bgMusic.Length);
            } while (prevVal == chooseTrack);
            

            prevVal = chooseTrack;
            print("track chosen: " + chooseTrack);
            // a = bgMusic[chooseTrack].name;
            // PlayBgMusic(a);

            PlayBgMusic(chooseTrack);
        }
    }

    public void PlayBgMusic(int num)
    {
        Sound s = bgMusic[num];
        // Sound s =Array.Find(bgMusic, sound => sound.name == name);
        
        s.source = audioSource;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        // s.source.loop = s.loop;
        // s.source.playOnAwake = s.playOnAwake;
        s.source.spatialBlend = s.spatialBlend;
        
        s.source.Play();
    }
    
    /*
    public void Play(string name)
    {
        Sound s =Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    */
}