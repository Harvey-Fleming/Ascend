using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] Sound[] sounds;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.shouldLoop;
        }
    }

    public void Play(string clipName)
    {
        foreach(Sound sound in sounds)
        {
            if(clipName == sound.clipName)
            {
                if(!sound.source.isPlaying)
                {
                    sound.source.Play();
                }
                return;
            }
        }
        Debug.LogWarning("Could not Find Clip: " + clipName);
    }
}

[System.Serializable]
public class Sound
{
    public string clipName;
    public AudioClip clip;

    [Range(0, 1)] public float volume = 1f;
    public bool shouldLoop = false;


    public AudioSource source;
}
