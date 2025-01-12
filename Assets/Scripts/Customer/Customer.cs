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
public class IngrediantPricing
{
    public PizzaOptions topping;
    public float fullPriceTopping;
}

[System.Serializable]
public class Lines
{
    public string englishVersion;
    public string deutschVersion;
}

[System.Serializable]
public class SideToCheck
{
    public bool left;
    public bool right;
}

public class Customer : MonoBehaviour
{
    public States state;
    public IngrediantPricing[] pricing;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;

    [Header("Ordering")]
    public Dialog dialog;
    public PizzaTopping[] leftToppings;
    public PizzaTopping[] rightToppings;
    public PizzaCook cookTimes;
    public SideToCheck[] pizzaSides = new SideToCheck[2]; 

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

    public void SetBill()
    {
        bill = 5f;

        for(int i = 0; i < dialog.leftToppings.Length; i++)
        {
            for(int o = 0; o < pricing.Length; o++)
            {
                if(dialog.leftToppings[i].topping == pricing[o].topping)
                {
                    bill += pricing[o].fullPriceTopping / 2f;
                    break;
                }
            }
        }

        for(int i = 0; i < dialog.rightToppings.Length; i++)
        {
            for(int o = 0; o < pricing.Length; o++)
            {
                if(dialog.rightToppings[i].topping == pricing[o].topping)
                {
                    bill += pricing[o].fullPriceTopping / 2f;
                    break;
                }
            }
        }

        Debug.Log("Bill: " + bill.ToString());
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

    void OnCollisionEnter(Collision col) //When order is ready
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
                CheckSide(box);
                bool checkIngrediants = false;
                for(int i = 0; i < pizzaSides.Length; i++)
                {
                    if(pizzaSides[i].left || pizzaSides[i].right)
                    {
                        checkIngrediants = true;
                    }
                    else
                    {
                        checkIngrediants = false;
                    }
                }

                if(checkIngrediants)
                {
                    CheckIngrediants(box);
                }
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
        for(int i = 0; i < pizzaSides.Length; i++)
        {
            if(pizzaSides[i].left)
            {
                if(i == 0)
                {
                    for(int q = 0; q < leftToppings.Length; q++)
                    {
                        for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
                        {
                            if(leftToppings[q].topping == pizzabox.toppingInfo[o].ingrediant)
                            {
                                if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                {
                                    Upset();
                                    return;
                                }

                                if(pizzabox.toppingInfo[o].leftSideCount == leftToppings[q].amount)
                                {
                                    Debug.Log("Exact");

                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            for(int p = 0; p < pricing.Length; p++)
                                            {
                                                if(leftToppings[q].topping == pricing[p].topping)
                                                {
                                                    tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int difference = pizzabox.toppingInfo[o].leftSideCount - leftToppings[q].amount;
                                    Debug.Log("Not exact, difference: " + difference);
                                    if(difference <= leftToppings[q].maxDifferenceAccepted && difference >= leftToppings[q].minDifferenceAccepted)
                                    {
                                        Debug.Log("The difference is accepted.");

                                        PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                        for(int r = 0; r < tipAdditions.Length; r++)
                                        {
                                            if(rating == tipAdditions[r].rating)
                                            {
                                                for(int p = 0; p < pricing.Length; p++)
                                                {
                                                    if(leftToppings[q].topping == pricing[p].topping)
                                                    {
                                                        tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Upset();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for(int q = 0; q < rightToppings.Length; q++)
                    {
                        for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
                        {
                            if(rightToppings[q].topping == pizzabox.toppingInfo[o].ingrediant)
                            {
                                if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                {
                                    Upset();
                                    return;
                                }

                                if(pizzabox.toppingInfo[o].leftSideCount == rightToppings[q].amount)
                                {
                                    Debug.Log("Exact");

                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            for(int p = 0; p < pricing.Length; p++)
                                            {
                                                if(rightToppings[q].topping == pricing[p].topping)
                                                {
                                                    tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int difference = pizzabox.toppingInfo[o].leftSideCount - rightToppings[q].amount;
                                    Debug.Log("Not exact, difference: " + difference);
                                    if(difference <= rightToppings[q].maxDifferenceAccepted && difference >= rightToppings[q].minDifferenceAccepted)
                                    {
                                        Debug.Log("The difference is accepted.");

                                        PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                        for(int r = 0; r < tipAdditions.Length; r++)
                                        {
                                            if(rating == tipAdditions[r].rating)
                                            {
                                                for(int p = 0; p < pricing.Length; p++)
                                                {
                                                    if(rightToppings[q].topping == pricing[p].topping)
                                                    {
                                                        tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
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
                if(i == 0)
                {
                    for(int q = 0; q < leftToppings.Length; q++)
                    {
                        for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
                        {
                            if(leftToppings[q].topping == pizzabox.toppingInfo[o].ingrediant)
                            {
                                if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                {
                                    Upset();
                                    return;
                                }

                                if(pizzabox.toppingInfo[o].rightSideCount == leftToppings[q].amount)
                                {
                                    Debug.Log("Exact");

                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            for(int p = 0; p < pricing.Length; p++)
                                            {
                                                if(leftToppings[q].topping == pricing[p].topping)
                                                {
                                                    tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int difference = pizzabox.toppingInfo[o].rightSideCount - leftToppings[q].amount;
                                    Debug.Log("Not exact, difference: " + difference);
                                    if(difference <= leftToppings[q].maxDifferenceAccepted && difference >= leftToppings[q].minDifferenceAccepted)
                                    {
                                        Debug.Log("The difference is accepted.");

                                        PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                        for(int r = 0; r < tipAdditions.Length; r++)
                                        {
                                            if(rating == tipAdditions[r].rating)
                                            {
                                                for(int p = 0; p < pricing.Length; p++)
                                                {
                                                    if(leftToppings[q].topping == pricing[p].topping)
                                                    {
                                                        tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Upset();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for(int q = 0; q < rightToppings.Length; q++)
                    {
                        for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
                        {
                            if(rightToppings[q].topping == pizzabox.toppingInfo[o].ingrediant)
                            {
                                if(pizzabox.toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                {
                                    Upset();
                                    return;
                                }

                                if(pizzabox.toppingInfo[o].rightSideCount == rightToppings[q].amount)
                                {
                                    Debug.Log("Exact");

                                    PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                    for(int r = 0; r < tipAdditions.Length; r++)
                                    {
                                        if(rating == tipAdditions[r].rating)
                                        {
                                            for(int p = 0; p < pricing.Length; p++)
                                            {
                                                if(rightToppings[q].topping == pricing[p].topping)
                                                {
                                                    tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int difference = pizzabox.toppingInfo[o].rightSideCount - rightToppings[q].amount;
                                    Debug.Log("Not exact, difference: " + difference);
                                    if(difference <= rightToppings[q].maxDifferenceAccepted && difference >= rightToppings[q].minDifferenceAccepted)
                                    {
                                        Debug.Log("The difference is accepted.");

                                        PizzaRating rating = pizzabox.toppingInfo[o].toppingDistanceRating;
                                        for(int r = 0; r < tipAdditions.Length; r++)
                                        {
                                            if(rating == tipAdditions[r].rating)
                                            {
                                                for(int p = 0; p < pricing.Length; p++)
                                                {
                                                    if(rightToppings[q].topping == pricing[p].topping)
                                                    {
                                                        tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
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

        switch(cookTimes) //Sets the cookTimes
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

        if(pizzabox.cookedTimes != this.cookedTimes) //If the cook times isn't exact then check if it is 1 more than it.
        {
            if(pizzabox.cookedTimes == this.cookedTimes + 1 && this.cookedTimes + 1 != 4)
            {
                tip += 1f;
            }
            else
            {
                Upset();
                return;
            }
        }
        else
        {
            tip += 1f;
        }

        if(pizzabox.cuts.numberOfCuts != this.numberOfCuts) //Checks if cuts are exact and if not then less or equal to two more cuts.
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

    public void CheckSide(PizzaBox pizzabox)
    {
        bool[] leftChecks = new bool[dialog.leftToppings.Length];
        bool[] rightChecks = new bool[dialog.leftToppings.Length];

        for(int i = 0; i < dialog.leftToppings.Length; i++)
        {
            for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
            {
                if(dialog.leftToppings[i].topping == pizzabox.toppingInfo[o].ingrediant)
                {
                    if(pizzabox.toppingInfo[o].leftSideCount > 0)
                    {
                        leftChecks[i] = true;
                    }
                    if(pizzabox.toppingInfo[o].rightSideCount > 0)
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
            pizzaSides[0].left = true;
            pizzaSides[0].right = false;
        }

        if(continueCheck)
        {
            for(int i = 0; i < rightChecks.Length; i++)
            {
                if(!rightChecks[i])
                {
                    Debug.Log("Right Failed For LeftSide");
                    Upset();
                    return;
                }

                Debug.Log("Right succeeded For LeftSide");
                pizzaSides[0].left = false;
                pizzaSides[0].right = true;
            }
        }

        //Right Side
        bool[] leftChecks1 = new bool[dialog.rightToppings.Length];
        bool[] rightChecks1 = new bool[dialog.rightToppings.Length];

        for(int i = 0; i < dialog.rightToppings.Length; i++)
        {
            for(int o = 0; o < pizzabox.toppingInfo.Length; o++)
            {
                if(dialog.rightToppings[i].topping == pizzabox.toppingInfo[o].ingrediant)
                {
                    if(pizzabox.toppingInfo[o].leftSideCount > 0)
                    {
                        leftChecks1[i] = true;
                    }
                    if(pizzabox.toppingInfo[o].rightSideCount > 0)
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
            pizzaSides[1].left = true;
            pizzaSides[1].right = false;
        }

        if(continueCheck1)
        {
            for(int i = 0; i < rightChecks1.Length; i++)
            {
                if(!rightChecks1[i])
                {
                    Debug.Log("Right Failed For RightSide");
                    Upset();
                    return;
                }

                Debug.Log("Right succeeded For RightSide");
                pizzaSides[1].left = false;
                pizzaSides[1].right = true;
            }
        }
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
