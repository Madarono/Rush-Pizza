using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayRequirements
{
    public int day;
    public float dayInSeconds;
    public int pizzaMadeReq;
    public int customerHappyReq;
}

public class StartDays : MonoBehaviour
{
    public TimeChanges timeChanges;
    public DayRequirements[] requirements;
    public DailyChallenges dailyChallenges;
    public CustomerManager manager;
    public Stats stats;
    public float speedMultiplyIncrease = 30f;
    public GameObject skipVisual;
    public bool canSkip = false;
    private int indexForReq;
    public bool canCheck = false;

    void Start()
    {
        skipVisual.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canSkip)
        {
            ForceIncrease();
        }
    }

    public void CheckTime()
    {
        for(int i = 0; i < requirements.Length; i++)
        {
            if(stats.day == requirements[i].day)
            {
                timeChanges.ingameHoursInSeconds = requirements[i].dayInSeconds;
                indexForReq = i;
                canCheck = true;
                dailyChallenges.paper.SetActive(false);
                timeChanges.ResetTime();
                break;
            }
        }

        if(!canCheck) //To generate items of DailyChallenges when day 2 is reached, Rushhour also works by itself after Day 2
        {
            dailyChallenges.GenerateItems();
        }
    }

    public void CheckRequirements()//0, 6
    {
        if(!canCheck)
        {
            return;
        }

        int pizzasMade = (int)dailyChallenges.store[0].value;
        int customersHappy = (int)dailyChallenges.store[6].value;

        if(pizzasMade >= requirements[indexForReq].pizzaMadeReq && customersHappy >= requirements[indexForReq].customerHappyReq)
        {
            skipVisual.SetActive(true);
            canSkip = true;
        }
    }

    public void ForceIncrease()
    {
        timeChanges.timeMultiplyer = timeChanges.timeMultiplyer * speedMultiplyIncrease;
        manager.DeleteCustomer();
    }
    
}
