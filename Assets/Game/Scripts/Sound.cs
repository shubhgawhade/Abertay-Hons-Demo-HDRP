using System;
using UnityEngine;

[Serializable]
public class Sound
{
    // public string name;
    public AudioClip clip;
    [Range(0f,100f)]
    public float volume = 100;
    [Range(0.1f,3f)]
    public float pitch = 1;
    // public bool playOnAwake;
    // public bool loop;
    [Range(0f,1f)]
    public float spatialBlend = 1;

    [HideInInspector]
    public AudioSource source;
}
