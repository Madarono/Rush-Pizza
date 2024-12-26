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
public class TipAdditions
{
    public PizzaRating rating;
    public float percentageOfTip;
}

[System.Serializable]
public class Lines
{
    public string englishVersion;
    public string deutschVersion;
}

public class Customer : MonoBehaviour
{
    public States state;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;

    [Header("Ordering")]
    public Dialog dialog;
    public PizzaTopping[] toppings;
    public PizzaCook cookTimes;
    public int cookedTimes;
    public int numberOfCuts;

    public PizzaRating overallPizzaRating;
    public TipAdditions[] tipAdditions;
    public float tip;
    public float bill;

    public Lines[] happyLines;
    public Lines[] upsetLines;

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

    void Start()
    {
        dialogBox = dialogWindow.GetComponent<DialogBox>();
    }

    public void InitiateTalk(TalkType talkType)
    {
        if(state == States.Waiting)
        {
            return;
        }
        if(state == States.Static)
        {
            bill = 5f;

            for(int i = 0; i < toppings.Length; i++)
            {
                bill += toppings[i].priceOfTopping;
            }
        }

        state = States.Talking;

        dialogWindow.SetActive(true);
        if(settings.english)
        {
            for(int i = 0; i < dialog.talk.Length; i++)
            {
                if(dialog.talk[i].type == talkType)
                {           
                    StartCoroutine(ShowText(dialog.talk[i].content));   
                    break;
                }
            }
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
        else
        {
            for(int i = 0; i < dialog.talk.Length; i++)
            {
                if(dialog.talk[i].type == talkType)
                {                
                    StartCoroutine(ShowText(dialog.talk[i].contentDeutsch));
                    break;
                }
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
    }

    void OnCollisionEnter(Collision col)
    {
        if(state != States.Waiting)
        {
            return;
        }

        GameObject obj = col.collider.gameObject;

        PizzaBox box = obj.GetComponent<PizzaBox>();

        if(box != null)
        {
            Debug.Log("Found Box!");

            if(box.ingrediants.Length > 0)
            {
                Debug.Log("Box has information");
                CheckIngrediants(box);
                Destroy(obj);
            }
            else
            {
                Debug.Log("Box has no information");
            }
        }
    }

    public void CheckIngrediants(PizzaBox pizzabox)
    {
        for(int i = 0; i < toppings.Length; i++)
        {
            for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
            {
                if(toppings[i].topping == pizzabox.toppingInfo[o].ingrediant)
                {
                    switch(toppings[i].side)
                    {
                        case PizzaSide.Left:
                            if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                            {
                                Upset();
                                return;
                            }

                            if(pizzabox.toppingInfo[o].leftSideCount == toppings[i].amount)
                            {
                                Debug.Log("Exact");
                                
                                PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                for(int r = 0; r < tipAdditions.Length; r++)
                                {
                                    if(rating == tipAdditions[r].rating)
                                    {
                                        tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                    }
                                }
                            }
                            else
                            {
                                int difference = pizzabox.toppingInfo[o].leftSideCount - toppings[i].amount;
                                Debug.Log("Not exact, difference: " + difference);
                                if(difference <= toppings[i].differenceAccepted && difference >= -toppings[i].differenceAccepted)
                                {
                                    Debug.Log("The difference is accepted.");
                                    
                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                        }
                                    }
                                }
                                else
                                {
                                    Upset();
                                    return;
                                }
                            }
                            break;

                        case PizzaSide.Right:
                            if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                            {
                                Upset();
                                return;
                            }

                            if(pizzabox.toppingInfo[o].rightSideCount == toppings[i].amount)
                            {

                                Debug.Log("Exact");
                                
                                PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                for(int r = 0; r < tipAdditions.Length; r++)
                                {
                                    if(rating == tipAdditions[r].rating)
                                    {
                                        tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                    }
                                }
                            }
                            else
                            {
                                int difference = pizzabox.toppingInfo[o].rightSideCount - toppings[i].amount;
                                Debug.Log("Not exact, difference: " + difference);
                                if(difference <= toppings[i].differenceAccepted && difference >= -toppings[i].differenceAccepted)
                                {
                                    Debug.Log("The difference is accepted.");
                                    
                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                        }
                                    }
                                }
                                else
                                {
                                    Upset();
                                    return;
                                }
                            }
                            break;
                        
                        case PizzaSide.AllRound:
                            if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                            {
                                Upset();
                                return;
                            }

                            if(pizzabox.toppingInfo[o].toppingCount == toppings[i].amount)
                            {
                                
                                Debug.Log("Exact");
                                
                                PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                for(int r = 0; r < tipAdditions.Length; r++)
                                {
                                    if(rating == tipAdditions[r].rating)
                                    {
                                        tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                    }
                                }
                            }
                            else
                            {
                                int difference = pizzabox.toppingInfo[o].toppingCount - toppings[i].amount;
                                Debug.Log("Not exact, difference: " + difference);
                                if(difference <= toppings[i].differenceAccepted && difference >= -toppings[i].differenceAccepted)
                                {
                                    Debug.Log("The difference is accepted.");
                                    
                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            tip += toppings[i].priceOfTopping / tipAdditions[r].percentageOfTip; 
                                        }
                                    }
                                }
                                else
                                {
                                    Upset();
                                    return;
                                }
                            }
                            break;
                    }
                }
            }
        }

        switch(cookTimes)
        {
            case PizzaCook.Raw:
                cookedTimes = 0;
                break;

            case PizzaCook.Cooked:
                cookedTimes = 1;
                break;

            case PizzaCook.DoubleCooked:
                cookedTimes = 2;
                break;

            case PizzaCook.TripleCooked:
                cookedTimes = 3;
                break;
        }

        if(pizzabox.cookedTimes != this.cookedTimes)
        {
            if(pizzabox.cookedTimes != this.cookedTimes + 1 && this.cookedTimes + 1 != 4)
            {
                Upset();
                return;
            }
        }
        else
        {
            tip += 1.5f;
        }

        if(pizzabox.cuts.numberOfCuts != this.numberOfCuts)
        {
            if(pizzabox.cuts.numberOfCuts <= this.numberOfCuts + 2)
            {
                tip += 0.5f;
            }
            else
            {
                Upset();
                return;
            }
        }
        else
        {
            tip += 0.5f;
        }

        Satisfied();
    }

    void Upset()
    {
        settings.money -= bill;
        Debug.Log("Very angry");
        string line = "";
        if(settings.english)
        {
            line = upsetLines[Random.Range(0,upsetLines.Length)].englishVersion;
        }
        else
        {
            line = upsetLines[Random.Range(0,upsetLines.Length)].deutschVersion;
        }

        mouseCursor.FreeCusorState();
        playerMovement.canMove = false;
        playerCam.canMove = false;
        state = States.Ending;
        StartCoroutine(ShowText(line));
    }

    void Satisfied()
    {
        settings.money += tip;
        Debug.Log("Very happy");
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
        StartCoroutine(ShowText(line));
    }
}
