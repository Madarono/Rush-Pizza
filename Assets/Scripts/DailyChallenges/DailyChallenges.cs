using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredValues
{
    public string nameOfValue; //For readability
    public float value;
}

public class DailyChallenges : MonoBehaviour
{
    [Header("Scripts")]
    public StartDays startDays;
    public Stats stats;
    public Settings settings;
    public Player_Cam playerCam;
    public PlayerMovement playerMovement;
    public Pausing pausing;
    public MouseCursor mouseCursor; 

    [Header("Window")]
    public GameObject window;
    public Animator[] anim;
    public float leaveDuration;
    private bool isOpen;

    [Header("Paper")]
    public GameObject paper;
    public Animator paperAnim;
    public float paperLeaveDuration;

    [Header("Challenges")]
    public Challenge[] challenges;
    public float easyChances = 100f;
    public float normalChances = 50f;
    public float hardChances = 10f;

    private List<Challenge> easyChallenges = new List<Challenge>();
    private List<Challenge> normalChallenges = new List<Challenge>();
    private List<Challenge> hardChallenges = new List<Challenge>();
    
    public StoredValues[] store;
    public List<ChallengeItem> items = new List<ChallengeItem>();
    public GameObject itemPrefab;
    public Transform parent;
    public int amountOfChallenges = 3;

    private bool canSee = true;

    void Start()
    {
        window.SetActive(false);
        OrganizeChallenges();
        // GenerateItems();
    }

    void OrganizeChallenges()
    {
        foreach(Challenge c in challenges)
        {
            switch(c.difficulty)
            {
                case Difficulty.Easy:
                    easyChallenges.Add(c);
                    break;
                
                case Difficulty.Medium:
                    normalChallenges.Add(c);
                    break;

                case Difficulty.Hard:
                    hardChallenges.Add(c);
                    break; 
            }
        }
    }

    void Update()
    {
        if(!canSee)
        {
            return;
        }

        if(Input.GetKeyDown(settings.dailyChallenge))
        {
            BothWindow();
        }
    }

    public void BothWindow()
    {
        if(pausing.isPausing || startDays.canCheck)
        {
            return;
        }

        isOpen = !isOpen;
        
        if(isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    void OpenWindow()
    {
        window.SetActive(true);
        playerCam.canMove = false;
        playerMovement.canMove = false;
        pausing.lockMouse = false;
        mouseCursor.FreeCusorState();
    }

    void CloseWindow()
    {
        StartCoroutine(CloseChallengeWindow());
    }

    IEnumerator CloseChallengeWindow()
    {
        foreach(Animator animator in anim)
        {
            animator.SetTrigger("Close");
        }
        playerCam.canMove = true;
        playerMovement.canMove = true;
        pausing.lockMouse = true;
        mouseCursor.LockCusorState();
        yield return new WaitForSeconds(leaveDuration);
        window.SetActive(false);
    }

    public void GenerateItems()
    {
        List<Challenge> availableEasyChallenges = easyChallenges;
        List<Challenge> availableNormalChallenges = normalChallenges;
        List<Challenge> availableHardChallenges = hardChallenges;
        for(int i = 0; i < amountOfChallenges; i++)
        {
            GameObject go = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.rotation = parent.rotation;
            go.transform.localScale = Vector3.one;

            ChallengeItem goScript = go.GetComponent<ChallengeItem>();
            items.Add(goScript);
            float randomChance = Random.Range(0, 100f);
            
            int random = 0;
            Challenge challenge = null;

            if(randomChance < hardChances)
            {
                random = Random.Range(0, availableHardChallenges.Count);
                int indexOfValue = availableHardChallenges[random].indexOfValue;
                challenge = availableHardChallenges[random];
                
                availableHardChallenges.RemoveAt(random);
                // for(int o = availableHardChallenges.Count; o >= 0; o--)
                // {
                //     if(availableHardChallenges[o].indexOfValue == indexOfValue)
                //     {
                //         availableHardChallenges.RemoveAt(o);
                //     }
                // }
            }
            else if(randomChance < normalChances)
            {
                random = Random.Range(0, availableNormalChallenges.Count);
                int indexOfValue = availableNormalChallenges[random].indexOfValue;
                challenge = availableNormalChallenges[random];
                
                availableNormalChallenges.RemoveAt(random);
                // for(int o = availableNormalChallenges.Count; o >= 0; o--)
                // {
                //     if(availableNormalChallenges[o].indexOfValue == indexOfValue)
                //     {
                //         availableNormalChallenges.RemoveAt(o);
                //     }
                // }
            }
            else
            {
                random = Random.Range(0, availableEasyChallenges.Count);
                int indexOfValue = availableEasyChallenges[random].indexOfValue;
                challenge = availableEasyChallenges[random];
                
                availableEasyChallenges.RemoveAt(random);
                // for(int o = availableEasyChallenges.Count; o >= 0; o--)
                // {
                //     if(availableEasyChallenges[o].indexOfValue == indexOfValue)
                //     {
                //         availableEasyChallenges.RemoveAt(o);
                //     }
                // }
            }
            
            goScript.challenge = challenge;
            goScript.settings = this.settings;
            goScript.manager = this;
            goScript.stats = this.stats;
            goScript.Refresh();
        }
    }

    public void RefreshItems()
    {
        startDays.CheckRequirements();
        foreach(ChallengeItem script in items)
        {
            script.Refresh();
        }
    }

    public void CalculateEnd()
    {
        canSee = false;
        StartCoroutine(LeavePaper());
        foreach(ChallengeItem script in items)
        {
            script.CheckConditions();
        }
    }

    IEnumerator LeavePaper()
    {
        paperAnim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(paperLeaveDuration);
        paper.SetActive(false);
    }
}
