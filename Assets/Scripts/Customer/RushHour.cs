using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;

public class RushHour : MonoBehaviour
{
    [Header("Scripts")]
    public Music music;
    public SoundManager sounds;
    public StartDays startDays;
    public MainMenu main;
    public CustomerManager manager;
    public Settings settings;
    public PlayerMovement player;
    public Conveyer conveyer;
    
    [Header("Post Processing")]
    public PostProcessVolume volume;
    public float minChromatic = 0f;
    public float maxChromatic = 0.6f;
    public float chromaticSpeed = 2f;
    public float currentChromatic;
    private ChromaticAberration ac;

    [Header("Chance to start")]
    public float minRushDelay = 0f;
    public float maxRushDelay = 240f;
    public float rushDelay;
    public float chanceToRush = 40f;
    public float durationInSeconds = 120f;
    public float currentDuration;
    
    [Header("Modifications")]
    public float patience = .5f;
    public float tip = 2f;
    
    public float rushSpeed;
    private float currentPlayerSpeed;

    public float sprintSpeed; 
    private float currentPlayerSprint;

    public float throwForce = 15f;
    private float currentThrowForce;

    public float conveyerSpeed = 2f;
    private float currentConveyerSpeed;

    [Header("Visuals")]
    public GameObject heartBeat;
    public float heartBeatFrequency = 1f;
    public AudioSource heartSource;
    public GameObject window;
    public Animator anim;
    public float delayInactive;
    public TextMeshProUGUI visual;
    public LanguageStrings visualScript;
    public string glitchCharacters = "abcdefghijklmnopqrstuvwxyz";
    public float delayGlitch = 0.05f;
    public float delayOriginal = 0.5f;
    private bool doGlitch;
    public bool repeat = false;
    public bool rush = false;
    public bool canBeRushed = false;

    public GameObject song;
    public AudioSource songSource;

    void Start()
    {
        volume.profile.TryGetSettings(out ac);
        ac.intensity.value = maxChromatic;
        heartBeat.SetActive(false);
        window.SetActive(false);
        DetermineRushHour();
    }

    void Update()
    {
        if(startDays.canCheck)
        {
            return;
        }

        if(songSource != null)
        {
            songSource.volume = sounds.controls.background * 0.8f;
        }

        if(rush && currentChromatic < maxChromatic)
        {
            currentChromatic += Time.deltaTime * chromaticSpeed;
            ac.intensity.value = currentChromatic;
        }
        else if(!rush && currentChromatic > minChromatic)
        {
            currentChromatic -= Time.deltaTime * chromaticSpeed;
            ac.intensity.value = currentChromatic;
        }

        if(currentChromatic > maxChromatic)
        {
            currentChromatic = maxChromatic;
            ac.intensity.value = currentChromatic;
        }
        else if(currentChromatic < minChromatic)
        {
            currentChromatic = minChromatic;
            ac.intensity.value = currentChromatic;
            ac.enabled.value = false;
        }

        if(rushDelay > 0 && canBeRushed && main.gameState == PizzaGameState.InGame)
        {
            rushDelay -= Time.deltaTime;
        }

        if(rush && currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
        }
        else if(rush && currentDuration <= 0 && manager.customer == null)
        {
            StopRush();
        }
    }

    public void DetermineRushHour()
    {
        float chance = Random.Range(0, 100f);
        if(chance < chanceToRush)
        {
            rushDelay = Random.Range(minRushDelay, maxRushDelay);
            canBeRushed = true;
        }
    }

    public void StartRush()
    {
        ac.enabled.value = true;
        currentDuration = durationInSeconds;
        rush = true;
        currentPlayerSpeed = player.moveSpeed;
        currentPlayerSprint = player.sprintSpeed;
        currentConveyerSpeed = conveyer.speed;
        currentThrowForce = settings.throwForce;
        
        settings.throwForce = throwForce;
        player.moveSpeed = rushSpeed;
        player.sprintSpeed = sprintSpeed;
        conveyer.speed = conveyerSpeed;

        
        repeat = true;
        window.SetActive(true);   
        RepeatGlitch(repeat, delayGlitch / 5f, delayOriginal / 5f);
        sounds.Generate2DSoundLoop(transform.position, sounds.rushHourMusic, false, 0.8f);
        song = sounds.lastSound;
        songSource = sounds.lastScript;
        music.source.Pause();
        music.isPaused = true;
        StartCoroutine(HeartBeat());
    }

    public void StopRush()
    {
        rush = false;
        settings.throwForce = currentThrowForce;
        player.moveSpeed = currentPlayerSpeed;
        player.sprintSpeed = currentPlayerSprint;
        conveyer.speed = currentConveyerSpeed;   
        heartSource.Stop();
        heartBeat.SetActive(false);

        music.source.Play();
        music.isPaused = false;

        Destroy(song);

        StopAllCoroutines();
        StartCoroutine(InactiveWindow());
    }

    public void AddRushCustomer(Customer customer, AdvancedTippingSystem tip)
    {
        customer.patienceMultiplyer = patience;
        customer.SetPatience();

        tip.tipMultiplyer = this.tip;
    }

    IEnumerator HeartBeat()
    {
        while(rush)
        {
            heartBeat.SetActive(true);
            heartSource.Play();
            yield return new WaitForSeconds(heartBeatFrequency);
            heartSource.Stop();
            // heartBeat.SetActive(false);
        }
    }
    
    IEnumerator InactiveWindow()
    {
        anim.SetTrigger("Leave");
        yield return new WaitForSeconds(delayInactive);
        window.SetActive(false);
    }

    public void RepeatGlitch(bool repeat, float delay, float wordDelay)
    {
        StartCoroutine(TextGlitch(repeat, delay, wordDelay));
    }

    IEnumerator TextGlitch(bool repeat, float delay, float wordDelay)
    {
        visualScript.enabled = false;
        string originalVisual = "";

        if(settings.english)
        {
            originalVisual = visualScript.english;
        }
        else
        {
            originalVisual = visualScript.deutsch;
        }
        
        string glitchedText = visual.text;
        for(int i = 0; i < originalVisual.Length; i++)
        {
            doGlitch = true;
            if(originalVisual[i] == ' ')
            {
                doGlitch = false;
            }
            StartCoroutine(Countdown(wordDelay));
            // glitchedText = glitchedText + " ";
            while(doGlitch)
            {
                char[] c = glitchedText.ToCharArray(); 
                c[i] = glitchCharacters[Random.Range(0, glitchCharacters.Length)];
                glitchedText = new string(c);
                visual.text = glitchedText;
                yield return new WaitForSeconds(delay);
            }
            
            char[] newOriginal = glitchedText.ToCharArray();
            newOriginal[i] = originalVisual[i];
            glitchedText = new string(newOriginal);
            visual.text = glitchedText;
        }
        visual.text = originalVisual;
        visualScript.enabled = true;

        if(repeat)
        {
            RepeatGlitch(this.repeat, this.delayGlitch, this.delayOriginal);
        }
    }

    IEnumerator Countdown(float wordDelay)
    {
        yield return new WaitForSeconds(wordDelay);
        doGlitch = false;
    }
}
