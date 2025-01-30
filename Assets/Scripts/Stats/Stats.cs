using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public class Stats : MonoBehaviour
{
    public Settings settings;

    [Header("Stats")]
    public int day;
    public float moneyGained;
    public float tipGained;
    public float refundsLost;
    public float rent = 5f;
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

    void Start()
    {
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
        float totalUsedSupplies = 0f;
        for(int i = 0; i < toppingStats.Length; i++)
        {
            totalUsedSupplies += toppingStats[i].moneySpent;
        }
        totalUsedSupplies = Mathf.Round(totalUsedSupplies * 100f) / 100f;
        
        profits = -totalUsedSupplies + -refundsLost + (moneyGained + tipGained);
        profits = Mathf.Round(profits * 100f) / 100f;
        paper.SetActive(true);
        information[0].infoValue[0].text = day.ToString();
        information[1].infoValue[0].text = "+" + moneyGained.ToString();
        information[2].infoValue[0].text = "+" + tipGained.ToString();
        information[3].infoValue[0].text = "-" + rent.ToString();
        information[4].infoValue[0].text = "-" + refundsLost.ToString();
        information[5].infoValue[0].text = "-" + totalUsedSupplies.ToString();
        
        if(profits > 0)
        {
            information[7].infoValue[0].text = "+" + profits.ToString();
            information[7].infoValue[0].gameObject.SetActive(true);
            information[7].infoValue[1].gameObject.SetActive(false);
        }
        else
        {
            information[7].infoValue[1].text = "-" + profits.ToString();
            information[7].infoValue[0].gameObject.SetActive(false);
            information[7].infoValue[1].gameObject.SetActive(true);
        }

        toppingStats[0].valueVisual.text = "-" + doughSpent.ToString("F2");
        for(int i = 1; i < toppingStats.Length; i++)
        {
            if(!toppingStats[i].showVisual)
            {
                continue;
            }


            toppingStats[i].valueVisual.text = "-" + toppingStats[i].moneySpent.ToString("F2");
        }

        StartCoroutine(SmoothTransition());
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

    IEnumerator SmoothTransition()
    {
        yield return new WaitForSeconds(delayPaper);
        for(int i = 0; i < information.Length; i++)
        {
            yield return new WaitForSeconds(information[i].delayToAppear);
            information[i].info.SetActive(true);
        }
    }
}
