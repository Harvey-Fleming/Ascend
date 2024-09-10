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
        Debug.LogWarning("Play Error: Could not Find Clip: " + clipName);
    }

    public void Stop(string clipName)
    {
        foreach (Sound sound in sounds)
        {
            if (clipName == sound.clipName)
            {
                if (sound.source.isPlaying)
                {
                    sound.source.Stop();
                }
                return;
            }
            Debug.LogWarning("Stop Error: Could not Find Clip: " + clipName);
        }
    }

    public void StopAll()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
    }

    public void FadeOutSound(string clipName, int fadeDuration)
    {
        foreach (Sound sound in sounds)
        {
            if (clipName == sound.clipName)
            {
                if (sound.source.isPlaying)
                {
                    float maxVol = sound.volume;

                    StartCoroutine(FadeSound(sound.source, maxVol, 0, fadeDuration));
                }
                return;
            }
            Debug.LogWarning("Fade Out Error: Could not find Clip: " + clipName);
        }
    }

    public void FadeInSound(string clipName, int fadeDuration)
    {
        foreach (Sound sound in sounds)
        {
            if (clipName == sound.clipName)
            {
                sound.source.Play();
                if (sound.source.isPlaying)
                {
                    sound.source.volume = 0;
                    float maxVol = sound.volume;
                    

                    StartCoroutine(FadeSound(sound.source, 0, maxVol, fadeDuration));
                }
                return;
            }
            Debug.LogWarning("Fade In Error: Could not find Clip: " + clipName);
        }
    }

    IEnumerator FadeSound(AudioSource source, float start, float end, float duration)
    {
        float t = 0;
        while(t/duration <= 1)
        {
            //Debug.Log(source.clip.name + " is now at " + source.volume);
            source.volume = Mathf.Lerp(start, end, t/duration);
            t += Time.deltaTime;
            yield return null;
        }

        if(end == 0)
        {
            source.Stop();
        }
        source.volume = end;
        yield return null;
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
