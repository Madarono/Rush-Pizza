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
    public float patienceInSeconds;
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
    public bool left = false;
    public bool right = false;
}

[System.Serializable]
public class EmotionRequirements
{
    public Sprite emotion;
    public float maxPatienceRequired;
    public float minPatienceRequired;
}

public class Customer : MonoBehaviour
{
    public States state;
    public IngrediantPricing[] pricing;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;
    [HideInInspector]public Stats stats;
    [HideInInspector]public RecipeSystem recipeSys;

    [Header("Ordering")]
    public Dialog dialog;
    public PizzaCook cookTimes;
    public List<SideToCheck> pizzaSides; 

    public float pizzasNeeded;
    public List<PizzaBox> pizzaBoxes;

    public int cookedTimes;
    public int numberOfCuts;

    public PizzaRating overallPizzaRating;
    public TipAdditions[] tipAdditions;
    public float tip;
    public float bill;

    [Header("Lines")]
    public Lines[] happyLines;
    public Lines[] upsetLines;

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
        bill = dialog.pizzas.Length * 5f;

        for(int p = 0; p < dialog.pizzas.Length; p++)
        {
            for(int i = 0; i < dialog.pizzas[p].leftToppings.Length; i++)
            {
                for(int o = 0; o < pricing.Length; o++)
                {
                    if(dialog.pizzas[p].leftToppings[i].topping == pricing[o].topping)
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
                    if(dialog.pizzas[p].rightToppings[i].topping == pricing[o].topping)
                    {
                        bill += pricing[o].fullPriceTopping / 2f;
                        break;
                    }
                }
            }
        }

        Debug.Log("Bill: " + bill.ToString());
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
                    if(dialog.pizzas[p].leftToppings[i].topping == pricing[o].topping)
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
                    if(dialog.pizzas[p].rightToppings[i].topping == pricing[o].topping)
                    {
                        patience += pricing[o].patienceInSeconds;
                        break;
                    }
                }
            }
        }

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

        if(patience <= 0)
        {
            Upset();
            return;
        }

        if(box != null)
        {
            Debug.Log("Found Box!");

            if(box.ingrediants.Length > 0)
            {
                Debug.Log("Box has information");
                pizzaBoxes.Add(box);
                obj.SetActive(false);
                if(pizzaBoxes.Count < pizzasNeeded)
                {
                    return;
                }

                CheckSide(pizzaBoxes);
                bool checkIngrediants = false;
                for(int i = 0; i < pizzaSides.Count; i++)
                {
                    if(pizzaSides[i].left || pizzaSides[i].right)
                    {
                        checkIngrediants = true;
                        continue;
                    }

                    checkIngrediants = false;
                    break;
                }

                if(checkIngrediants)
                {
                    CheckIngrediants(pizzaBoxes);
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
            maxSidesChecked = minSidesChecked;
            for(int i = minSidesChecked; i < maxSidesChecked; i++)
            {
                if(pizzaSides[i].left)
                {
                    if(i == 0)
                    {
                        for(int q = 0; q < dialog.pizzas[w].leftToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].leftToppings[q].topping == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                        {
                                            Upset();
                                            return;
                                        }

                                        if(pizzabox[k].toppingInfo[o].leftSideCount == dialog.pizzas[w].leftToppings[q].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].leftToppings[q].topping == pricing[p].topping)
                                                        {
                                                            tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].leftSideCount - dialog.pizzas[w].leftToppings[q].amount;
                                            Debug.Log("Not exact, difference: " + difference);
                                            if(difference <= dialog.pizzas[w].leftToppings[q].maxDifferenceAccepted && difference >= dialog.pizzas[w].leftToppings[q].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].leftToppings[q].topping == pricing[p].topping)
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
                        for(int q = 0; q < dialog.pizzas[w].rightToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].rightToppings[q].topping == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                        {
                                            Upset();
                                            return;
                                        }

                                        if(pizzabox[k].toppingInfo[o].leftSideCount == dialog.pizzas[w].rightToppings[q].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].rightToppings[q].topping == pricing[p].topping)
                                                        {
                                                            tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].leftSideCount - dialog.pizzas[w].rightToppings[q].amount;
                                            Debug.Log("Not exact, difference: " + difference);
                                            if(difference <= dialog.pizzas[w].rightToppings[q].maxDifferenceAccepted && difference >= dialog.pizzas[w].rightToppings[q].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].rightToppings[q].topping == pricing[p].topping)
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
                else
                {
                    if(i == 0)
                    {
                        for(int q = 0; q < dialog.pizzas[w].leftToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].leftToppings[q].topping == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                        {
                                            Upset();
                                            return;
                                        }

                                        if(pizzabox[k].toppingInfo[o].rightSideCount == dialog.pizzas[w].leftToppings[q].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].leftToppings[q].topping == pricing[p].topping)
                                                        {
                                                            tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].rightSideCount - dialog.pizzas[w].leftToppings[q].amount;
                                            Debug.Log("Not exact, difference: " + difference);
                                            if(difference <= dialog.pizzas[w].leftToppings[q].maxDifferenceAccepted && difference >= dialog.pizzas[w].leftToppings[q].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].leftToppings[q].topping == pricing[p].topping)
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
                        for(int q = 0; q < dialog.pizzas[w].rightToppings.Length; q++)
                        {
                            for(int k = 0; k < pizzabox.Count; k++)
                            {
                                for(int o = 0; o < pizzabox[k].toppingInfo.Length; o++)
                                {
                                    if(dialog.pizzas[w].rightToppings[q].topping == pizzabox[k].toppingInfo[o].ingrediant)
                                    {
                                        if(pizzabox[k].toppingInfo[o].toppingDistanceRating == PizzaRating.Terrible)
                                        {
                                            Upset();
                                            return;
                                        }

                                        if(pizzabox[k].toppingInfo[o].rightSideCount == dialog.pizzas[w].rightToppings[q].amount)
                                        {
                                            Debug.Log("Exact");

                                            PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                            for(int r = 0; r < tipAdditions.Length; r++)
                                            {
                                                if(rating == tipAdditions[r].rating)
                                                {
                                                    for(int p = 0; p < pricing.Length; p++)
                                                    {
                                                        if(dialog.pizzas[w].rightToppings[q].topping == pricing[p].topping)
                                                        {
                                                            tip = pricing[p].fullPriceTopping / tipAdditions[r].percentageOfTip;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int difference = pizzabox[k].toppingInfo[o].rightSideCount - dialog.pizzas[w].rightToppings[q].amount;
                                            Debug.Log("Not exact, difference: " + difference);
                                            if(difference <= dialog.pizzas[w].rightToppings[q].maxDifferenceAccepted && difference >= dialog.pizzas[w].rightToppings[q].minDifferenceAccepted)
                                            {
                                                Debug.Log("The difference is accepted.");

                                                PizzaRating rating = pizzabox[k].toppingInfo[o].toppingDistanceRating;
                                                for(int r = 0; r < tipAdditions.Length; r++)
                                                {
                                                    if(rating == tipAdditions[r].rating)
                                                    {
                                                        for(int p = 0; p < pricing.Length; p++)
                                                        {
                                                            if(dialog.pizzas[w].rightToppings[q].topping == pricing[p].topping)
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
                    if(pizzabox[p].cookedTimes == cookedTimes[i] + 1 && cookedTimes[i] + 1 != 4)
                    {
                        tip += 1f;
                        break;
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
            }
        }

        Satisfied();
    }

    public void CheckSide(List<PizzaBox> pizzabox)
    {
        while(pizzaSides.Count < (pizzaBoxes.Count * 2))
        {
            SideToCheck sides = new SideToCheck();
            pizzaSides.Add(sides);
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
                        if(dialog.pizzas[q].leftToppings[i].topping == pizzabox[p].toppingInfo[o].ingrediant)
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
                        if(dialog.pizzas[q].rightToppings[i].topping == pizzabox[p].toppingInfo[o].ingrediant)
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
            return;
        }
    }

    void Upset()
    {
        SetEmotion(2);
        settings.AddToMoney(-bill);
        stats.refundsLost += bill;
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

        if(dialog.giveToRecipe)
        {
            recipeSys.VisibleRecipe(dialog.indexOfRecipe);
        }
    }

    void Satisfied()
    {
        SetEmotion(0);
        float endingTip = Mathf.Round((tip * (percentageOfPatience / 100f)) * 100f) / 100f;      
        settings.AddToMoney(endingTip);
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
        StartCoroutine(ShowText(line));

        if(dialog.giveToRecipe)
        {
            recipeSys.VisibleRecipe(dialog.indexOfRecipe);
        }
    }
}
