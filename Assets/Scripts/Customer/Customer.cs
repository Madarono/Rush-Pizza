using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SpecialStrings
{
    public string character;
    public float addedWaitTime;
}

public enum States
{
    Static,
    Talking,
    Waiting,
    Ending
}


[System.Serializable]
public class IngrediantPricing
{
    public PizzaOptions topping;
    public float fullPriceTopping;
    public float patienceInSeconds;
}

[System.Serializable]
public class Lines
{
    public string englishVersion;
    public string deutschVersion;
}

//Pizza
[System.Serializable]
public class SideToCheck
{
    public bool left = false;
    public bool right = false;
}

[System.Serializable]
public class ToppingAttributes
{
    public PizzaOptions ingrediant;
    public int amount;
    public int minDifferenceAccepted;
    public int maxDifferenceAccepted;
}

//

[System.Serializable]
public class EmotionRequirements
{
    public Sprite emotion;
    public float maxPatienceRequired;
    public float minPatienceRequired;
}

[System.Serializable]
public class CheckToppings
{
    public List<PizzaOptions> ingrediant = new List<PizzaOptions>();
    public List<int> amount = new List<int>();
}

public class Customer : MonoBehaviour
{
    public States state;
    public AdvancedTippingSystem advancedTip;
    public IngrediantPricing[] pricing;
    [HideInInspector]public Pausing pause;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public Mission mission;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;
    [HideInInspector]public Stats stats;
    [HideInInspector]public RecipeSystem recipeSys;
    [HideInInspector]public CustomerManager manager;
    [HideInInspector]public Brief brief;
    [HideInInspector]public DailyChallenges dailyChallenges;

    [Header("Checkng Toppings")]
    public List<CheckToppings> pizzaboxToppings = new List<CheckToppings>();
    public List<PizzaOptions> dialogToppings = new List<PizzaOptions>();

    [Header("Ordering")]
    public Dialog dialog;
    public PizzaCook cookTimes;
    public List<SideToCheck> pizzaSides; 
    public ToppingAttributes[] toppingAttributes;

    public float pizzasNeeded;
    public List<PizzaBox> pizzaBoxes;

    public int cookedTimes;
    public int numberOfCuts;

    public PizzaRating overallPizzaRating;
    public TipAdditions[] tipAdditions;
    // public float tip;
    public float bill;

    [Header("Lines")]
    public AdvancedFeedback feedback;
    public Lines[] happyLines;
    public Lines[] upsetLines;
    public float delayOfVoice = 0.1f;

    private string cacheLines;
    private bool skipLines = false; 

    private int randomVoice;

    [Header("Patience")]
    private float totalPatience;
    private float percentageOfPatience;
    public float patience;
    public TextMeshProUGUI patienceCounter;

    public Image emotion;
    public EmotionRequirements[] emotionStages;

    [Header("Visual")]
    public GameObject dialogWindow;
    private DialogBox dialogBox;
    public TextMeshProUGUI dialogContent;
    public GameObject okayButton;
    public GameObject whatButton;
    public GameObject hintButton;
    public bool askedWhat = false;
    public bool askedHint = false;
    
    public Settings settings;
    public SpecialStrings[] specialCharcters;

    [Header("Rush Hour")]
    public float patienceMultiplyer = 1f;

    void Start()
    {
        SetPatience();
        dialogBox = dialogWindow.GetComponent<DialogBox>();
    }
    
    void Update()
    {
        if(state == States.Waiting && patience > 0)
        {
            patience -= Time.deltaTime;
            UpdatePatience();
            SetEmotion();
        }
        else if(patience <= 0 && state == States.Waiting)
        {
            LeaveWithoutNotice();
        }

        if(skipLines && Input.GetMouseButtonDown(0))
        {
            SkipTalk();
        }
    }

    public void InitiateTalk(TalkType talkType)
    {
        randomVoice = Random.Range(0, manager.sound.customerVoice.Length);
        brief.dialog = dialog;
        StopAllCoroutines();

        if(state == States.Waiting)
        {
            return;
        }
        if(state == States.Static)
        {
            SetBill();
        }

        state = States.Talking;
        pause.lockMouse = false;

        dialogWindow.SetActive(true);
        
        int random = 0;

        if(settings.english)
        {
            switch(talkType)
            {
                case TalkType.Initial:  
                    random = Random.Range(0, dialog.initialTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[0] = random;
                    StartCoroutine(ShowText(dialog.initialTalk[random].content)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.initialTalk[random].content));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.initialTalk[random].content;  
                    break;

                case TalkType.What:
                    random = Random.Range(0, dialog.whatTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[1] = random;
                    StartCoroutine(ShowText(dialog.whatTalk[random].content)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.whatTalk[random].content));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.whatTalk[random].content;  
                    break;

                case TalkType.Hint:
                    random = Random.Range(0, dialog.hintTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[2] = random;
                    StartCoroutine(ShowText(dialog.hintTalk[random].content)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.hintTalk[random].content));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.hintTalk[random].content;  
                    break;
            }

            // for(int i = 0; i < dialog.talk.Length; i++)
            // {
            //     if(dialog.talk[i].type == talkType)
            //     {           
            //         StartCoroutine(ShowText(dialog.talk[i].content)); 
            //         if(settings.enableVoice)
            //         {
            //             StartCoroutine(MakeVoice(dialog.talk[i].content));
            //         }
            //         StartCoroutine(EnableSkipLines());
            //         cacheLines = dialog.talk[i].content;  
            //         break;
            //     }
            // }
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
        else
        {
            switch(talkType)
            {
                case TalkType.Initial:  
                    random = Random.Range(0, dialog.initialTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[0] = random;
                    StartCoroutine(ShowText(dialog.initialTalk[random].contentDeutsch)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.initialTalk[random].contentDeutsch));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.initialTalk[random].contentDeutsch;  
                    break;

                case TalkType.What:
                    random = Random.Range(0, dialog.whatTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[1] = random;
                    StartCoroutine(ShowText(dialog.whatTalk[random].contentDeutsch)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.whatTalk[random].contentDeutsch));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.whatTalk[random].contentDeutsch;  
                    break;

                case TalkType.Hint:
                    random = Random.Range(0, dialog.hintTalk.Length);
                    Debug.Log(random);
                    brief.idTalk[2] = random;
                    StartCoroutine(ShowText(dialog.hintTalk[random].contentDeutsch)); 
                    if(settings.enableVoice)
                    {
                        StartCoroutine(MakeVoice(dialog.hintTalk[random].contentDeutsch));
                    }
                    StartCoroutine(EnableSkipLines());
                    cacheLines = dialog.hintTalk[random].contentDeutsch;  
                    break;
            }
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
    }

    IEnumerator ShowText(string content)
    {
        dialogWindow.SetActive(true);
        dialogBox.CheckDimentions(content);
        whatButton.SetActive(false);
        hintButton.SetActive(false);

        okayButton.SetActive(true);
        if(!askedHint && state != States.Ending)
        {
            if(askedWhat)
            {
                hintButton.SetActive(true);
            }
            else
            {
                whatButton.SetActive(true);
            }
        }

        dialogContent.text = "";
        for(int i = 0; i < content.Length; i++)
        {
            float delay = dialog.speedOfTalk;
            char character = content[i];
            dialogContent.text = dialogContent.text + character.ToString();
            
            for(int o = 0; o < specialCharcters.Length; o++)
            {
                string characterString = character.ToString();
                if(characterString == specialCharcters[o].character)
                {
                    delay += specialCharcters[o].addedWaitTime;
                    break;
                }
            }

            yield return new WaitForSeconds(delay);
        }

        StopAllCoroutines(); //Maybe remove this if there are bugs
        skipLines = false;
    }
    IEnumerator MakeVoice(string content)
    {
        for(int i = 0; i < content.Length; i++)
        {
            int randomType = Random.Range(0, manager.sound.customerVoice[randomVoice].voice.Length);
            manager.sound.Generate2DSound(transform.position, manager.sound.customerVoice[randomVoice].voice[randomType], true, 0.6f);
            yield return new WaitForSeconds(delayOfVoice);
        }
    }

    IEnumerator EnableSkipLines()
    {
        yield return new WaitForSeconds(0.1f);
        skipLines = true;
    }
    
    void SkipTalk()
    {
        StopAllCoroutines();
        dialogContent.text = cacheLines;
        skipLines = false;
    }

    public void SetBill()
    {
        bill = dialog.pizzas.Length * 5f;

        for(int p = 0; p < dialog.pizzas.Length; p++)
        {
            for(int i = 0; i < dialog.pizzas[p].leftToppings.Length; i++)
            {
                for(int o = 0; o < pricing.Length; o++)
                {
                    if(dialog.pizzas[p].leftToppings[i] == pricing[o].topping)
                    {
                        bill += pricing[o].fullPriceTopping / 2f;
                        break;
                    }
                }
            }
        }

        for(int p = 0; p < dialog.pizzas.Length; p++)
        {
            for(int i = 0; i < dialog.pizzas[p].rightToppings.Length; i++)
            {
                for(int o = 0; o < pricing.Length; o++)
                {
                    if(dialog.pizzas[p].rightToppings[i] == pricing[o].topping)
                    {
                        bill += pricing[o].fullPriceTopping / 2f;
                        break;
                    }
                }
            }
        }

        // Debug.Log("Bill: " + bill.ToString());
    }
    public void SetPatience()
    {   
        patience = 60f;

        for(int p = 0; p < dialog.pizzas.Length; p++)
        {
            for(int i = 0; i < dialog.pizzas[p].leftToppings.Length; i++)
            {
                for(int o = 0; o < pricing.Length; o++)
                {
                    if(dialog.pizzas[p].leftToppings[i] == pricing[o].topping)
                    {
                        patience += pricing[o].patienceInSeconds;
                        break;
                    }
                }
            }
        }

        for(int p = 0; p < dialog.pizzas.Length; p++)
        {
            for(int i = 0; i < dialog.pizzas[p].rightToppings.Length; i++)
            {
                for(int o = 0; o < pricing.Length; o++)
                {
                    if(dialog.pizzas[p].rightToppings[i] == pricing[o].topping)
                    {
                        patience += pricing[o].patienceInSeconds;
                        break;
                    }
                }
            }
        }

        patience = patience * patienceMultiplyer;
        
        totalPatience = patience;
        UpdatePatience();
    }
    public void UpdatePatience()
    {
        percentageOfPatience = patience / totalPatience * 100f;
        patienceCounter.text = "%" + percentageOfPatience.ToString("F0");
    }

    public void SetEmotion() //0 for happy, 1 for impatient, 2 for angry
    {
        for(int i = 0; i < emotionStages.Length; i++)
        {
            if(percentageOfPatience <= emotionStages[i].maxPatienceRequired && percentageOfPatience >= emotionStages[i].minPatienceRequired)
            {
                emotion.sprite = emotionStages[i].emotion;
                return;
            }
        }
    }

    public void SetEmotion(int id)
    {
        emotion.sprite = emotionStages[id].emotion;
    }

    void OnCollisionEnter(Collision col) //When order is ready
    {
        if(state != States.Waiting)
        {
            return;
        }

        GameObject obj = col.collider.gameObject;

        PizzaBox box = obj.GetComponent<PizzaBox>();

        if(patience <= 0)
        {
            Upset();
            return;
        }

        if(box != null)
        {

            if(box.ingrediants.Length > 0)
            {
                pizzaBoxes.Add(box);
                obj.SetActive(false);
                if(pizzaBoxes.Count < pizzasNeeded)
                {
                    return;
                }

                bool sideCheck = CheckSide(pizzaBoxes);
                bool toppingCheck = CheckToppings(pizzaBoxes);
                if(sideCheck && toppingCheck)
                {
                    CheckIngrediants(pizzaBoxes);
                }
                else
                {
                    Upset();
                    return;
                }
            }
            else
            {
                Debug.Log("Box has no information");
            }
        }
    }

    //Checking Phase
    public void CheckIngrediants(List<PizzaBox> pizzabox) //Sides 0 and 1 is in pizza 0
    {
        int minSidesChecked = 0;
        int maxSidesChecked = 1;
        for(int w = 0; w < dialog.pizzas.Length; w++)
        {
            minSidesChecked = w * 2;
            maxSidesChecked = minSidesChecked + 1;
            for(int i = minSidesChecked; i < maxSidesChecked; i++)
            {
                if(pizzaSides[i].left)
                {
                    if(i == minSidesChecked) //minSides for left, else for Right side of the checking process
                    {
                        for(int q = 0; q < dialog.pizzas[w].leftToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].leftToppings[q] == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        //Terrible quality
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible || pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Bad)
                                        {
                                            feedback.BadQuality();
                                            Upset();
                                            return;
                                        }

                                        int index = 0;
                                        for(int j = 0; j < toppingAttributes.Length; j++)
                                        {
                                            if(pizzabox[k].toppingInfo[o].ingrediant == toppingAttributes[j].ingrediant)
                                            {
                                                index = j;
                                                break;
                                            }
                                        }
                                        if(pizzabox[k].toppingInfo[o].leftSideCount == toppingAttributes[index].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].leftToppings[q] == pricing[p].topping)
                                                        {
                                                            advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);

                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].leftSideCount - toppingAttributes[index].amount;
                                            Debug.Log("Not exact, difference: " + difference + " in " + dialog.pizzas[w].leftToppings[q]);
                                            if(difference <= toppingAttributes[index].maxDifferenceAccepted && difference >= toppingAttributes[index].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].leftToppings[q] == pricing[p].topping)
                                                            {
                                                                advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);                                                          
                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Difference Denied"); //When the difference is too high or too low
                                                feedback.TurnIngrediantToName(pizzabox[k].toppingInfo[o].ingrediant);
                                                feedback.CheckDifference(difference);
                                                Upset();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        for(int q = 0; q < dialog.pizzas[w].rightToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].rightToppings[q] == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible || pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Bad)
                                        {
                                            feedback.BadQuality();
                                            Upset();
                                            return;
                                        }

                                        int index = 0;
                                        for(int j = 0; j < toppingAttributes.Length; j++)
                                        {
                                            if(pizzabox[k].toppingInfo[o].ingrediant == toppingAttributes[j].ingrediant)
                                            {
                                                index = j;
                                                break;
                                            }
                                        }
                                        if(pizzabox[k].toppingInfo[o].leftSideCount == toppingAttributes[index].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].rightToppings[q] == pricing[p].topping)
                                                        {
                                                            advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);                                                        
                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].leftSideCount - toppingAttributes[index].amount;
                                            Debug.Log("Not exact, difference: " + difference + " in " + dialog.pizzas[w].rightToppings[q]);
                                            if(difference <= toppingAttributes[index].maxDifferenceAccepted && difference >= toppingAttributes[index].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].rightToppings[q] == pricing[p].topping)
                                                            {
                                                                advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);
                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Difference Denied");
                                                feedback.TurnIngrediantToName(pizzabox[k].toppingInfo[o].ingrediant);
                                                feedback.CheckDifference(difference);
                                                Upset();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                }
                else
                {
                    if(i == minSidesChecked) //minSides for left, else for Right side of the checking process
                    {
                        for(int q = 0; q < dialog.pizzas[w].leftToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].leftToppings[q] == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible || pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Bad)
                                        {
                                            feedback.BadQuality();
                                            Upset();
                                            return;
                                        }

                                        int index = 0;
                                        for(int j = 0; j < toppingAttributes.Length; j++)
                                        {
                                            if(pizzabox[k].toppingInfo[o].ingrediant == toppingAttributes[j].ingrediant)
                                            {
                                                index = j;
                                                break;
                                            }
                                        }
                                        if(pizzabox[k].toppingInfo[o].rightSideCount == toppingAttributes[index].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].leftToppings[q] == pricing[p].topping)
                                                        {
                                                                advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);
                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].rightSideCount - toppingAttributes[index].amount;
                                            Debug.Log("Not exact, difference: " + difference + " in " + dialog.pizzas[w].leftToppings[q]);
                                            if(difference <= toppingAttributes[index].maxDifferenceAccepted && difference >= toppingAttributes[index].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].leftToppings[q] == pricing[p].topping)
                                                            {
                                                                advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);
                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Difference Denied");
                                                feedback.TurnIngrediantToName(pizzabox[k].toppingInfo[o].ingrediant);
                                                feedback.CheckDifference(difference);
                                                Upset();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        for(int q = 0; q < dialog.pizzas[w].rightToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].rightToppings[q] == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible || pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Bad)
                                        {
                                            feedback.BadQuality();
                                            Upset();
                                            return;
                                        }

                                        int index = 0;
                                        for(int j = 0; j < toppingAttributes.Length; j++)
                                        {
                                            if(pizzabox[k].toppingInfo[o].ingrediant == toppingAttributes[j].ingrediant)
                                            {
                                                index = j;
                                                break;
                                            }
                                        }
                                        if(pizzabox[k].toppingInfo[o].rightSideCount == toppingAttributes[index].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].rightToppings[q] == pricing[p].topping)
                                                        {
                                                            advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);

                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].rightSideCount - toppingAttributes[index].amount;
                                            Debug.Log("Not exact, difference: " + difference + " in " + dialog.pizzas[w].rightToppings[q]);
                                            if(difference <= toppingAttributes[index].maxDifferenceAccepted && difference >= toppingAttributes[index].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].rightToppings[q] == pricing[p].topping)
                                                            {
                                                                advancedTip.AddToTip(rating, pricing[p].fullPriceTopping);
                                                            

                                                                
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Debug.Log("Difference Denied");
                                                feedback.TurnIngrediantToName(pizzabox[k].toppingInfo[o].ingrediant);
                                                feedback.CheckDifference(difference);
                                                Upset();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }   

        }

        List<int> cookedTimes = new List<int>();
        while(cookedTimes.Count < dialog.pizzas.Length)
        {
            cookedTimes.Add(0);
        }

        for(int i = 0; i < dialog.pizzas.Length; i++)
        {
            switch(dialog.pizzas[i].cookTimes) //Sets the cookTimes
            {
                case PizzaCook.Raw:
                    cookedTimes[i] = 0;
                    break;

                case PizzaCook.Cooked:
                    cookedTimes[i] = 1;
                    break;

                case PizzaCook.DoubleCooked:
                    cookedTimes[i] = 2;
                    break;

                case PizzaCook.TripleCooked:
                    cookedTimes[i] = 3;
                    break;
            }
        }

        for(int i = 0; i < cookedTimes.Count; i++)
        {
            for(int p = 0; p < pizzabox.Count; p++)
            {
                if(pizzabox[p].cookedTimes != cookedTimes[i]) //If the cook times isn't exact then check if it is 1 more than it.
                {
                    if(pizzabox[p].cookedTimes <= cookedTimes[i] + 2 && cookedTimes[i] + 2 != 4)
                    {
                        advancedTip.OtherTip(pizzabox[p].cookedTimes, cookedTimes[i], cookedTimes[i] + 2, advancedTip.perfectCookingTip);
                                                break;
                    }
                    else
                    {
                        feedback.Checkcooked(pizzabox[p].cookedTimes);
                        Upset();
                        return;
                    }
                }
                else
                {
                    advancedTip.OtherTip(pizzabox[p].cookedTimes, cookedTimes[i], cookedTimes[i] + 2, advancedTip.perfectCookingTip);
                                        break;
                }
            }
        }

        for(int i = 0; i < dialog.pizzas.Length; i++)
        {
            for(int p = 0; p < pizzabox.Count; p++)
            {
                if(pizzabox[p].cuts.numberOfCuts != dialog.pizzas[i].numberOfCuts) //Checks if cuts are exact and if not then less or equal to two more cuts.
                {
                    if(pizzabox[p].cuts.numberOfCuts <= dialog.pizzas[i].maximumCutsAllowed && pizzabox[p].cuts.numberOfCuts >= dialog.pizzas[i].minimumCutsAllowed)
                    {
                        advancedTip.OtherTip((int)pizzabox[p].cuts.numberOfCuts, dialog.pizzas[i].minimumCutsAllowed, dialog.pizzas[i].maximumCutsAllowed, advancedTip.perfectCutTip);
                        
                    }
                    else
                    {
                        feedback.CheckCuts((int)pizzabox[p].cuts.numberOfCuts, dialog.pizzas[i].minimumCutsAllowed, dialog.pizzas[i].maximumCutsAllowed);
                        Upset();
                        return;
                    }
                }
                else
                {
                    advancedTip.OtherTip((int)pizzabox[p].cuts.numberOfCuts, dialog.pizzas[i].minimumCutsAllowed, dialog.pizzas[i].maximumCutsAllowed, advancedTip.perfectCutTip);
                    
                }
            }
        }

        advancedTip.CalculateFinalTip(patience, totalPatience);

        Satisfied();
    }

    public bool CheckSide(List<PizzaBox> pizzabox)
    {
        while(pizzaSides.Count < (pizzaBoxes.Count * 2))
        {
            SideToCheck sides = new SideToCheck();
            pizzaSides.Add(sides);
        }

        if(dialog.hatedToppings.Length > 0)
        {
            foreach(PizzaBox box in pizzabox)
            {
                for(int i = 0; i < box.toppingInfo.Length; i++)
                {
                    for(int o = 0; o < dialog.hatedToppings.Length; o++)
                    {
                        if((box.toppingInfo[i].rightSideCount > 0 || box.toppingInfo[i].leftSideCount > 0) && box.toppingInfo[i].ingrediant == dialog.hatedToppings[o])
                        {
                            feedback.TurnIngrediantToName(dialog.hatedToppings[o]);
                            feedback.HatedTopping();
                            return false;
                        }
                    }
                }
            }
        }

        int minSidesChecked = 0;
        int maxSidesChecked = 1;
        for(int q = 0; q < dialog.pizzas.Length; q++) //1
        {
            minSidesChecked = q * 2;
            maxSidesChecked = minSidesChecked + 1;
            for(int p = 0; p < pizzabox.Count; p++) //Both of pizzaboxes
            {
                bool[] leftChecks = new bool[dialog.pizzas[q].leftToppings.Length];
                bool[] rightChecks = new bool[dialog.pizzas[q].leftToppings.Length];

                for(int i = 0; i < dialog.pizzas[q].leftToppings.Length; i++)
                {
                    for(int o = 0; o < pizzabox[p].toppingInfo.Length; o++)
                    {
                        if(dialog.pizzas[q].leftToppings[i] == pizzabox[p].toppingInfo[o].ingrediant)
                        {
                            if(pizzabox[p].toppingInfo[o].leftSideCount > 0)
                            {
                                leftChecks[i] = true;
                            }
                            if(pizzabox[p].toppingInfo[o].rightSideCount > 0)
                            {
                                rightChecks[i] = true;
                            }
                        }
                    }
                }

                bool continueCheck = false;
                for(int i = 0; i < leftChecks.Length; i++)
                {
                    if(!leftChecks[i])
                    {
                        Debug.Log("Left Failed For LeftSide");
                        continueCheck = true;
                        break;
                    }

                    Debug.Log("Left succeeded For LeftSide");
                    pizzaSides[minSidesChecked].left = true;
                    pizzaSides[minSidesChecked].right = false;
                }

                if(continueCheck)
                {
                    for(int i = 0; i < rightChecks.Length; i++)
                    {
                        if(!rightChecks[i])
                        {
                            Debug.Log("Right Failed For LeftSide");
                            // Upset();
                            // return;
                        }

                        Debug.Log("Right succeeded For LeftSide");
                        pizzaSides[minSidesChecked].left = false;
                        pizzaSides[minSidesChecked].right = true;
                    }
                }

                //Right Side
                bool[] leftChecks1 = new bool[dialog.pizzas[q].rightToppings.Length];
                bool[] rightChecks1 = new bool[dialog.pizzas[q].rightToppings.Length];

                for(int i = 0; i < dialog.pizzas[q].rightToppings.Length; i++)
                {
                    for(int o = 0; o < pizzabox[p].toppingInfo.Length; o++)
                    {
                        if(dialog.pizzas[q].rightToppings[i] == pizzabox[p].toppingInfo[o].ingrediant)
                        {
                            if(pizzabox[p].toppingInfo[o].leftSideCount > 0)
                            {
                                leftChecks1[i] = true;
                            }
                            if(pizzabox[p].toppingInfo[o].rightSideCount > 0)
                            {
                                rightChecks1[i] = true;
                            }
                        }
                    }
                }

                bool continueCheck1 = false;
                for(int i = 0; i < leftChecks1.Length; i++)
                {
                    if(!leftChecks1[i] || !continueCheck)
                    {
                        Debug.Log("Left Failed For RightSide");
                        continueCheck1 = true;
                        break;
                    }

                    Debug.Log("Left succeeded For RightSide");
                    pizzaSides[maxSidesChecked].left = true;
                    pizzaSides[maxSidesChecked].right = false;
                }

                if(continueCheck1)
                {
                    for(int i = 0; i < rightChecks1.Length; i++)
                    {
                        if(!rightChecks1[i])
                        {
                            Debug.Log("Right Failed For RightSide");
                            // Upset();
                            // return;
                        }

                        Debug.Log("Right succeeded For RightSide");
                        pizzaSides[maxSidesChecked].left = false;
                        pizzaSides[maxSidesChecked].right = true;
                    }
                }
            }
        }

        bool callUpset = false;
        for(int i = 0; i < pizzaSides.Count; i++)
        {
            if(pizzaSides[i].left || pizzaSides[i].right)
            {
                Debug.Log("Index " + i + " Has either left or right in true");
                callUpset = false;
                continue;
            }

            callUpset = true;
            break;
        }

        if(callUpset)
        {
            Upset();
            return false;
        }

        return true;
    }

    bool CheckToppings(List<PizzaBox> pizzabox)
    {
        pizzaboxToppings.Clear();
        dialogToppings.Clear();

        foreach(PizzaBox box in pizzabox)
        {
            CheckToppings topping = new CheckToppings();
            for(int i = 0; i < box.toppingInfo.Length; i++)
            {
                if(box.toppingInfo[i].leftSideCount > 0 || box.toppingInfo[i].rightSideCount > 0)
                {
                    topping.ingrediant.Add(box.toppingInfo[i].ingrediant);
                    int bothSides = box.toppingInfo[i].leftSideCount + box.toppingInfo[i].rightSideCount;
                    topping.amount.Add(bothSides);
                }
            }
            pizzaboxToppings.Add(topping);
        }

        for(int i = 0; i < dialog.pizzas.Length; i++)
        {
            foreach(PizzaOptions option in dialog.pizzas[i].leftToppings)
            {
                if(!dialogToppings.Contains(option))
                {
                    dialogToppings.Add(option);
                }
            }
            foreach(PizzaOptions option in dialog.pizzas[i].rightToppings)
            {
                if(!dialogToppings.Contains(option))
                {
                    dialogToppings.Add(option);
                }
            }
        }

        if(pizzaboxToppings.Count <= 0)
        {
            Debug.Log("Nothing in box");
            return false;
        }

        List<PizzaOptions> duplicatePizza = new List<PizzaOptions>();
        List<int> duplicateAmount = new List<int>();
        foreach(CheckToppings pizza in pizzaboxToppings)
        {
            for(int i = 0; i < pizza.ingrediant.Count; i++)
            {
                if(!duplicatePizza.Contains(pizza.ingrediant[i]))
                {
                    duplicatePizza.Add(pizza.ingrediant[i]);
                    duplicateAmount.Add(pizza.amount[i]);
                }
            }
        }

        for (int i = duplicatePizza.Count - 1; i >= 0; i--)
        {
            foreach (PizzaOptions topping in dialogToppings)
            {
                if (duplicatePizza[i] == topping)
                {
                    Debug.Log("Removed: " + duplicatePizza[i].ToString());
                    int index = duplicatePizza.IndexOf(duplicatePizza[i]);
                    if (index >= 0 && index < duplicateAmount.Count) //Prevents out of range errors
                    {
                        duplicateAmount.RemoveAt(index);
                    }
                    
                    duplicatePizza.RemoveAt(i);
                    
                    break;
                }
            }
        }

        int extraToppings = 0;
        foreach(int duplicate in duplicateAmount)
        {
            extraToppings += duplicate;
        }

        if(extraToppings > dialog.maximumToppingsAllowed)
        {
            Debug.Log("Too much toppings");
            int highest = 0;
            for(int i = 0; i < duplicateAmount.Count; i++)
            {
                if(duplicateAmount[i] > highest)
                {
                    highest = duplicateAmount[i];
                }
            }
            for(int i = 0; i < duplicateAmount.Count; i++)
            {
                if(highest == duplicateAmount[i])
                {
                    feedback.TurnIngrediantToName(duplicatePizza[i]);
                    break;
                }
            }
            feedback.TooMuchForeignToppings();
            return false;
        }
        else
        {
            Debug.Log("Accepted toppings");
            return true;
        }
    }

    void Upset()
    {
        settings.RegisterLoseSound();
        pause.lockMouse = false;
        mission.pizzasMade++;
        SetEmotion(2);
        settings.AddToMoney(-bill);
        stats.refundsLost += bill;
        dailyChallenges.store[2].value += bill;
        float percentage = (patience / totalPatience) * 100f;
        if(percentage < dailyChallenges.store[5].value)
        {
            dailyChallenges.store[5].value = percentage;
        }
        dailyChallenges.RefreshItems();
        string line = "";
        if(settings.english)
        {
            if(feedback.dialogText != "")
            {
                line = feedback.dialogText;
            }
            else
            {
                line = upsetLines[Random.Range(0,upsetLines.Length)].englishVersion;
            }
        }
        else
        {
            if(feedback.dialogText != "")
            {
                line = feedback.dialogText;
            }
            else
            {
                line = upsetLines[Random.Range(0,upsetLines.Length)].deutschVersion;
            }
        }

        mouseCursor.FreeCusorState();
        playerMovement.canMove = false;
        playerCam.canMove = false;
        state = States.Ending;
        cacheLines = line;
        StartCoroutine(EnableSkipLines());
        StartCoroutine(ShowText(line));
        if(settings.enableVoice)
        {
            StartCoroutine(MakeVoice(line));
        }

        if(dialog.giveToRecipe)
        {
            recipeSys.VisibleRecipe(dialog.indexOfRecipe);
        }

        RemoveBoxes();
    }

    void LeaveWithoutNotice()
    {
        settings.AddToMoney(-bill);
        settings.RegisterLoseSound();
        stats.refundsLost += bill;
        state = States.Ending;
        dailyChallenges.store[2].value += bill;
        dailyChallenges.store[4].value += 1f;
        float percentage = (patience / totalPatience) * 100f;
        if(percentage < dailyChallenges.store[5].value)
        {
            dailyChallenges.store[5].value = percentage;
        }
        dailyChallenges.RefreshItems();
        manager.AskOkay();
    }

    void Satisfied()
    {
        settings.RegisterGetSound();
        pause.lockMouse = false;
        mission.pizzasMade++;
        SetEmotion(0);
        float endingTip = advancedTip.finalTip;      
        settings.AddToMoney(endingTip);
        dailyChallenges.store[3].value += endingTip;
        float percentage = patience / totalPatience;
        if(percentage > 0.5f)
        {
            dailyChallenges.store[6].value += 1f;
        }
        float percentage1 = (patience / totalPatience) * 100f;
        if(percentage1 < dailyChallenges.store[5].value)
        {
            dailyChallenges.store[5].value = percentage1;
        }
        dailyChallenges.RefreshItems();
        stats.tipGained += endingTip;
        
        string line = "";
        if(settings.english)
        {
            line = happyLines[Random.Range(0,happyLines.Length)].englishVersion;
        }
        else
        {
            line = happyLines[Random.Range(0,happyLines.Length)].deutschVersion;
        }

        mouseCursor.FreeCusorState();
        playerMovement.canMove = false;
        playerCam.canMove = false;
        state = States.Ending;
        cacheLines = line;
        StartCoroutine(EnableSkipLines());
        StartCoroutine(ShowText(line));
        if(settings.enableVoice)
        {
            StartCoroutine(MakeVoice(line));
        }

        if(dialog.giveToRecipe)
        {
            recipeSys.VisibleRecipe(dialog.indexOfRecipe);
        }

        RemoveBoxes();
    }

    void RemoveBoxes()
    {
        if(pizzaBoxes.Count <= 0)
        {
            return;
        }

        dailyChallenges.store[0].value += (float)pizzaBoxes.Count;
        dailyChallenges.RefreshItems();

        for(int i = pizzaBoxes.Count - 1; i >= 0; i--)
        {
            Destroy(pizzaBoxes[i].gameObject);
        }
        pizzaBoxes.Clear();
    }
}
