using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TutorialStates
{
    Walking,
    Running,
    Crouch,
    Interact,
    Use,
    PutToppings,
    CookPizza,
    CutPizza,
    DeliverPizza,
    Finished,
}

[System.Serializable]
public class TutorialSubtitles
{
    public TutorialStates state;
    public string english;
    public string deutsch;
}

public class Tutorial : MonoBehaviour, IDataPersistence
{
    public Stats stats;
    public Settings settings;
    public int day;
    public bool hasCompleted = false; 

    [Header("Tutorial")]
    public TutorialStates states;
    private float horizontalInput;
    private float verticalInput;
    [HideInInspector]public bool hasPicked;
    [HideInInspector]public bool hasUsed;
    [HideInInspector]public bool hasPutToppings;
    [HideInInspector]public bool hasCooked;
    [HideInInspector]public bool hasCut;
    [HideInInspector]public bool hasDelivered;
    public Outline[] toppingsOutline;
    public Outline[] ovenOutline;
    public Outline[] cutOutline;
    public GameObject startGameButton;

    [Header("Pizza Spawning")]
    public GameObject pizzaPrefab;
    public Transform placement;
    public SoundManager sound;
    public DragAndDrop dragAndDrop;

    [Header("Pizzabox Spawning")]
    public DoughSpawner doughSpawner;
    public GameObject pizzaboxPrefab;
    private GameObject boxCache;
    public Transform placementBox;
    public float durationToKill = 1f;

    [Header("Visual")]
    public GameObject subtitleWindow;
    public TextMeshProUGUI subtitleVisual;
    public float speedOfTalk = 0.02f;
    public TutorialSubtitles[] sub;
    public Animator subAnimator;
    public float endDuration = 2.1f;

    public GameObject tutorialButton;
    public Animator tutorialAnim; 
    public float tutorialDuration = 1f;

    public void SaveData(GameData data)
    {
        data.hasCompleted = this.hasCompleted;
    }
    public void LoadData(GameData data)
    {
        this.hasCompleted = data.hasCompleted;
        this.day = data.day;
        if(day == 0 && !hasCompleted)
        {
            StartTutorial();
        }
        else
        {
            hasCompleted = true;
        }
    }

    public void StartTutorial()
    {
        states = TutorialStates.Walking;
        hasPicked = false;
        hasUsed = false;
        hasPutToppings = false;
        hasCooked = false;
        hasCut = false;
        hasDelivered = false;
        startGameButton.SetActive(false);
        subtitleWindow.SetActive(true);
        StartCoroutine(TutorialAnimation());
    }
    
    IEnumerator TutorialAnimation()
    {
        subtitleVisual.text = "";
        tutorialAnim.SetTrigger("Tutorial");
        yield return new WaitForSeconds(tutorialDuration);
        tutorialButton.SetActive(false);
        hasCompleted = false;
        RefreshSubtitles();
    }

    void Update()
    {
        if(!hasCompleted)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            CheckRequirements();
        }
    }

    public void CheckRequirements()
    {
        switch(states)
        {
            case TutorialStates.Walking:
                if(horizontalInput != 0 || verticalInput != 0)
                {
                    states = TutorialStates.Running;
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.Running:
                if(Input.GetKeyDown(settings.sprint) && (horizontalInput != 0 || verticalInput != 0))
                {
                    states = TutorialStates.Crouch;
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.Crouch:
                if(Input.GetKeyDown(settings.crouch))
                {
                    states = TutorialStates.Interact;
                    GameObject go = Instantiate(pizzaPrefab, placement.position, Quaternion.identity);
                    Interactable dough = go.GetComponent<Interactable>();
                    dough.sound = this.sound;
                    dough.settings = this.settings;
                    dough.stats = this.stats;
                    dough.dragAndDrop = this.dragAndDrop;
                    dough.tutorial = this;
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.Interact:
                if(hasPicked)
                {
                    states = TutorialStates.Use;
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.Use:
                if(hasUsed)
                {
                    states = TutorialStates.PutToppings;
                    foreach(Outline outline in toppingsOutline)
                    {
                        outline.enabled = true;
                    }
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.PutToppings:
                if(hasPutToppings)
                {
                    states = TutorialStates.CookPizza;
                    foreach(Outline outline in toppingsOutline)
                    {
                        outline.enabled = false;
                    }
                    foreach(Outline outline in ovenOutline)
                    {
                        outline.enabled = true;
                    }
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.CookPizza:
                if(hasCooked)
                {
                    states = TutorialStates.CutPizza;
                    foreach(Outline outline in ovenOutline)
                    {
                        outline.enabled = false;
                    }
                    foreach(Outline outline in cutOutline)
                    {
                        outline.enabled = true;
                    }
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.CutPizza:
                if(hasCut)
                {
                    states = TutorialStates.DeliverPizza;
                    foreach(Outline outline in cutOutline)
                    {
                        outline.enabled = false;
                    }

                    boxCache = Instantiate(pizzaboxPrefab, new Vector3(doughSpawner.doughSpawns[0].position.x, doughSpawner.doughSpawns[0].position.y + 1f, doughSpawner.doughSpawns[0].position.z), Quaternion.identity);
                    boxCache.transform.rotation = Quaternion.Euler(doughSpawner.rotationOffset.x, doughSpawner.rotationOffset.y, doughSpawner.rotationOffset.z);
                    RefreshSubtitles();
                }
                break;

            case TutorialStates.DeliverPizza:
                if(hasDelivered)
                {
                    states = TutorialStates.Finished;
                    RefreshSubtitles();
                    StartCoroutine(KillPizza());
                    hasCompleted = true;
                }
                break;
        }
    }

    IEnumerator KillPizza() //Ending the tutorial
    {
        yield return new WaitForSeconds(durationToKill);
        Destroy(boxCache);
        subAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(endDuration);
        subtitleWindow.SetActive(false);
        startGameButton.SetActive(true);
        tutorialButton.SetActive(true);
    }

    IEnumerator Talk(string talk)
    {
        subtitleVisual.text = "";
        for(int i = 0; i < talk.Length; i++)
        {
            float delay = this.speedOfTalk;
            char character = talk[i];
            subtitleVisual.text = subtitleVisual.text + character.ToString();
            yield return new WaitForSeconds(delay);
        }
    }

    public void RefreshSubtitles()
    {
        for(int i = 0; i < sub.Length; i++)
        {
            if(states == sub[i].state)
            {
                StopAllCoroutines();
                string talk = settings.english ? sub[i].english : sub[i].deutsch;
                talk = DeciferString(talk);
                StartCoroutine(Talk(talk));

                break;
            }
        }
    }

    string DeciferString(string decifer)
    {
        decifer = decifer.Replace("(run)", settings.sprint.ToString());
        decifer = decifer.Replace("(crouch)", settings.crouch.ToString());

        return decifer;
    }
}
