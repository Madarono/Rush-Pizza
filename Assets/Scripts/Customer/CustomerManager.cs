using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPosition;
    private GameObject customer;
    private Customer goScript;

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
    
    void Start()
    {
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
            
            if(goScript.state == States.Talking)
            {
                settings.money += goScript.bill;
            }


            if(goScript.state == States.Ending)
            {
                Destroy(goScript.gameObject);
                goScript = null;
                dialogWindow.SetActive(false);
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
        goScript.settings = settings;
        goScript.mouseCursor = mouseCursor;
        goScript.playerMovement = playerMovement;
        goScript.playerCam = playerCam;
        goScript.dialogWindow = dialogWindow;
        goScript.dialogContent = dialogContent;
        goScript.okayButton = okayButton;
        goScript.whatButton = whatButton;
        goScript.hintButton = hintButton;
    }

    public void DeleteCustomer()
    {
        if(customer != null)
        {
            Destroy(customer);
            Debug.Log("Deleted customer successfully.");
        }
    }
    
}
