using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPosition;
    public Dialog[] orders;
    public bool randomizeCustomerOrders = true;

    private GameObject customer;
    private Customer goScript;

    [Header("Custommer Spawn Rate")]
    public float minWait = 1f;
    public float maxWait = 4f;
    private float currentWait;

    [Header("Things for Customer script")]
    public MouseCursor mouseCursor;
    public PlayerMovement playerMovement;
    public Player_Cam playerCam;
    public Settings settings;
    public TextMeshProUGUI dialogContent;

    public GameObject dialogWindow;
    public GameObject okayButton;
    public TextMeshProUGUI okayText;
    public GameObject whatButton;
    public TextMeshProUGUI whatText;
    public GameObject hintButton;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI patienceCounter;
    public Image emotion;
    public GameObject emotionWindow;
    
    void Start()
    {
        currentWait = Random.Range(minWait, maxWait);
        emotionWindow.SetActive(false);
        if(settings.english)
        {
            okayText.text = "Okay";
            whatText.text = "What?";
            hintText.text = "Hint";
        }
        else
        {
            okayText.text = "Okay";
            whatText.text = "Was?";
            hintText.text = "Tipp";
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            InstantiateCustomer();
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            DeleteCustomer();
        }

        if(currentWait > 0f && customer == null)
        {
            currentWait -= Time.deltaTime;
        }
        else if(currentWait <= 0f && customer == null)
        {
            currentWait = 0f;
            InstantiateCustomer();
        }
    }

    public void AskOkay()
    {
        if(goScript != null)
        {
            mouseCursor.LockCusorState();
            playerMovement.canMove = true;
            playerCam.canMove = true;
            goScript.askedHint = false;
            goScript.askedWhat = false;
            
            if(goScript.state == States.Talking) //Initiate Order
            {
                settings.AddToMoney(goScript.bill);
                emotionWindow.SetActive(true);
                goScript.SetPatience();
            }


            if(goScript.state == States.Ending) //End Order
            {
                DeleteCustomer();
                goScript = null;
                dialogWindow.SetActive(false);
                emotionWindow.SetActive(false);
                return;
            }
            
            goScript.state = States.Waiting;
            dialogWindow.SetActive(false);
        }
    }

    public void AskWhat()
    {
        if(goScript != null)
        {
            goScript.askedWhat = true;
            goScript.InitiateTalk(TalkType.What);
        }
    }

    public void AskHint()
    {
        if(goScript != null)
        {
            goScript.askedHint = true;
            goScript.InitiateTalk(TalkType.Hint);
        }
    }

    public void InstantiateCustomer()
    {
        if(customer != null)
        {
            return;
        }

        GameObject go = Instantiate(customerPrefab, spawnPosition.position, Quaternion.identity);
        customer = go;
        goScript = go.GetComponent<Customer>();
        if(randomizeCustomerOrders)
        {
            goScript.dialog = orders[Random.Range(0, orders.Length)];
        }
        goScript.pizzasNeeded = goScript.dialog.pizzas.Length;

        goScript.settings = settings;
        goScript.mouseCursor = mouseCursor;
        goScript.playerMovement = playerMovement;
        goScript.playerCam = playerCam;
        goScript.dialogWindow = dialogWindow;
        goScript.dialogContent = dialogContent;
        goScript.okayButton = okayButton;
        goScript.whatButton = whatButton;
        goScript.hintButton = hintButton;
        goScript.patienceCounter = patienceCounter;
        goScript.emotion = emotion;
    }

    public void DeleteCustomer()
    {
        if(customer != null)
        {
            Destroy(customer);
            emotionWindow.SetActive(false);
            customer = null;
            currentWait = Random.Range(minWait, maxWait);
            Debug.Log("Deleted customer successfully.");
        }
    }
    
}
