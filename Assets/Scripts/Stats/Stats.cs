using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Globalization;
using TMPro;

[System.Serializable]
public class ToppingStats
{
    public PizzaOptions topping;
    public float moneySpent;
    public TextMeshProUGUI valueVisual;
    public bool showVisual = true;
}

[System.Serializable]
public class ShowText
{
    public GameObject info;
    public TextMeshProUGUI[] infoValue;
    public float delayToAppear;
}

public class Stats : MonoBehaviour, IDataPersistence
{
    public Settings settings;
    public Mission mission;
    public StartDays startDays;

    [Header("Stats")]
    public int day;
    public float moneyGained;
    public float tipGained;
    public float refundsLost;
    public float rent = 5f;
    public float dailyChallengeProfit;
    public float profits;
    public GameObject[] statesOfProfit;
    public ToppingStats[] toppingStats;
    public float doughSpent;

    [Header("Visual")]
    public GameObject paper;
    public float delayPaper;
    public ShowText[] information;

    [Header("Checking Supplies")]
    public float speedOfMoving;
    public Transform[] paperPositions = new Transform[2];
    public bool checkSupplies;
    public GameObject supplies;
    public Transform[] suppliesPositions = new Transform[2]; 

    [Header("Options")]
    public GameObject newDayScreen;
    public float newDayDelay = 0.9f;
    public GameObject nextDayScreen;
    public float nextDayDelay = 1f;

    public void SaveData(GameData data)
    {
        data.day = this.day;
    }

    public void LoadData(GameData data)
    {
        this.day = data.day;
        startDays.CheckTime();
    }

    void Start()
    {
        StartCoroutine(NewDay());
        paper.SetActive(false);
        for(int i = 0; i < information.Length; i++)
        {
            information[i].info.SetActive(false);
        }
    }

    public void ShowEndOfTheDay()
    {
        supplies.SetActive(true);
        settings.AddWithoutVisual(-rent);
        UpdateValues();
        day++;
        mission.CheckRequirements();
        mission.UpdateVisual();
        StartCoroutine(SmoothTransition());
    }

    public void UpdateValues()
    {
        float totalUsedSupplies = 0f;
        toppingStats[0].moneySpent = doughSpent;
        for(int i = 0; i < toppingStats.Length; i++)
        {
            totalUsedSupplies += toppingStats[i].moneySpent;
        }
        totalUsedSupplies = Mathf.Round(totalUsedSupplies * 100f) / 100f;
        
        profits = -totalUsedSupplies + -refundsLost + -rent + (moneyGained + tipGained);
        profits = Mathf.Round(profits * 100f) / 100f;
        paper.SetActive(true);
        information[0].infoValue[0].text = day.ToString();
        if(settings.english)
        {
            information[1].infoValue[0].text = "+" + moneyGained.ToString("F2");
            information[2].infoValue[0].text = "+" + tipGained.ToString("F2");
            information[3].infoValue[0].text = "-" + rent.ToString("F2");
            information[4].infoValue[0].text = "-" + refundsLost.ToString("F2");
            information[5].infoValue[0].text = "-" + totalUsedSupplies.ToString("F2");
        }
        else
        {
            information[1].infoValue[0].text = "+" + moneyGained.ToString("F2", new CultureInfo("de-DE"));
            information[2].infoValue[0].text = "+" + tipGained.ToString("F2", new CultureInfo("de-DE"));
            information[3].infoValue[0].text = "-" + rent.ToString("F2", new CultureInfo("de-DE"));
            information[4].infoValue[0].text = "-" + refundsLost.ToString("F2", new CultureInfo("de-DE"));
            information[5].infoValue[0].text = "-" + totalUsedSupplies.ToString("F2", new CultureInfo("de-DE"));
        }
        
        if(profits > 0)
        {
            if(settings.english)
            {
                information[7].infoValue[0].text = "+" + profits.ToString("F2");
            }
            else
            {
                information[7].infoValue[0].text = "+" + profits.ToString("F2", new CultureInfo("de-DE"));
            }
            information[7].infoValue[0].gameObject.SetActive(true);
            information[7].infoValue[1].gameObject.SetActive(false);
        }
        else
        {
            float absoluteProfit = -profits;
            if(settings.english)
            {
                information[7].infoValue[1].text = "-" + absoluteProfit.ToString("F2");
            }
            else
            {
                information[7].infoValue[1].text = "-" + absoluteProfit.ToString("F2", new CultureInfo("de-DE"));
            }
            information[7].infoValue[0].gameObject.SetActive(false);
            information[7].infoValue[1].gameObject.SetActive(true);
        }

        if(settings.english)
        {
            information[8].infoValue[0].text = "+" + dailyChallengeProfit.ToString("F2");
            toppingStats[0].valueVisual.text = "-" + doughSpent.ToString("F2");
        }
        else
        {
            information[8].infoValue[0].text = "+" + dailyChallengeProfit.ToString("F2", new CultureInfo("de-DE"));
            toppingStats[0].valueVisual.text = "-" + doughSpent.ToString("F2", new CultureInfo("de-DE"));
        }

        for(int i = 1; i < toppingStats.Length; i++)
        {
            if(!toppingStats[i].showVisual)
            {
                continue;
            }

            if(settings.english)
            {
                toppingStats[i].valueVisual.text = "-" + toppingStats[i].moneySpent.ToString("F2");
            }
            else
            {
                toppingStats[i].valueVisual.text = "-" + toppingStats[i].moneySpent.ToString("F2", new CultureInfo("de-DE"));
            }
        }
    } 

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            checkSupplies = !checkSupplies;
        }
    }

    void FixedUpdate()
    {
        if(checkSupplies)
        {
            paper.transform.position = Vector3.Lerp(paper.transform.position, paperPositions[1].position, Time.deltaTime * speedOfMoving);
            supplies.transform.position = Vector3.Lerp(supplies.transform.position, suppliesPositions[1].position, Time.deltaTime * speedOfMoving);
        }
        else
        {
            paper.transform.position = Vector3.Lerp(paper.transform.position, paperPositions[0].position, Time.deltaTime * speedOfMoving);
            supplies.transform.position = Vector3.Lerp(supplies.transform.position, suppliesPositions[0].position, Time.deltaTime * speedOfMoving);
        }
    }

    public void GoToNextDay()
    {
        StartCoroutine(NextDay());
    }

    IEnumerator SmoothTransition()
    {
        yield return new WaitForSeconds(delayPaper);
        for(int i = 0; i < information.Length; i++)
        {
            yield return new WaitForSeconds(information[i].delayToAppear);
            information[i].info.SetActive(true);
        }
    }

    IEnumerator NewDay()
    {
        newDayScreen.SetActive(true);
        yield return new WaitForSeconds(newDayDelay);
        newDayScreen.SetActive(false);
    }

    IEnumerator NextDay()
    {
        nextDayScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(nextDayDelay);
        DataPersistenceManager.instance.SaveGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}
