using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Controls controls;
    public GameObject soundPrefab3D;
    public GameObject soundPrefab2D;

    [HideInInspector]public GameObject lastSound;
    [HideInInspector]public AudioSource lastScript;

    [Header("Pizza Klingen")]
    public AudioClip splat;
    public AudioClip cut;
    public AudioClip spreadDough;
    public AudioClip closePizzaBox;
    public AudioClip registerGet;
    public AudioClip registerLose;
    public AudioClip selectTopping;
    public AudioClip conveyerBelt;

    [Header("Pick Up")]
    public AudioClip pickup;
    public AudioClip drop;

    [Header("Misc")]
    public AudioClip[] buttonClick;
    public AudioClip decorationSelection;
    public AudioClip buyMerchant;
    public AudioClip startGame;

    [Header("Cache")]
    public bool cacheSound;
    public AudioClip[] soundsToCache;
    public AudioSource source;

    void Awake()
    {
        if(!cacheSound)
        {
            return;
        }

        source.volume = 0f;
        foreach(AudioClip clip in soundsToCache)
        {
            source.clip = clip;
            source.Play();
        }

        source.Stop();
    }


    public void GenerateSound(Vector3 place, AudioClip clip, bool isMaster, float amplifier)
    {
        GameObject go = Instantiate(soundPrefab3D, place, Quaternion.identity);
        AudioSource source = go.GetComponent<AudioSource>();

        source.volume = (isMaster ? controls.master : controls.background) * amplifier;
        source.clip = clip;
        source.Play();

        lastSound = go;

        Destroy(go, clip.length);
    }

    public void Generate2DSound(Vector3 place, AudioClip clip, bool isMaster, float amplifier)
    {
        GameObject go = Instantiate(soundPrefab2D, place, Quaternion.identity);
        AudioSource source = go.GetComponent<AudioSource>();

        source.volume = (isMaster ? controls.master : controls.background) * amplifier;
        source.clip = clip;
        source.Play();

        lastSound = go;

        Destroy(go, clip.length);
    }

    public void DeleteLastSound()
    {
        lastScript = null;
        Destroy(lastSound);
    }
}
