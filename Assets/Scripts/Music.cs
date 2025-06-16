using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public Controls controls;
    public float amplifier = 0.4f;
    public float clipLength;
    public SoundManager sounds;
    public AudioSource source;
    public bool isPaused;

    void Awake()
    {
        foreach(AudioClip clip in sounds.backgroundMusic)
        {
            source.clip = clip;
            source.Play();
        }

        source.clip = sounds.rushHourMusic;
        source.Play();
        source.Stop();
        source.clip = null;

        RandomizeSong();
    }

    void RandomizeSong()
    {
        int random = Random.Range(0, sounds.backgroundMusic.Length);
        source.clip = sounds.backgroundMusic[random];
        clipLength = sounds.backgroundMusic[random].length;
        source.Play();
    }

    void Update()
    {
        source.volume = controls.background * amplifier;

        if(!isPaused && clipLength > 0)
        {
            clipLength -= Time.deltaTime;
        }
        else if(!isPaused && clipLength <= 0)
        {
            RandomizeSong();
        }
    }
}
