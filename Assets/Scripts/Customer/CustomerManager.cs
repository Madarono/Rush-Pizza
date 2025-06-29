using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    public RushHour rushHour;
    public MainMenu main;
    public Pausing pausing;
    public Brief brief;
    public Stats stats;
    public GameObject customerPrefab;
    public GameObject merchantPrefab;
    public Transform spawnPosition;
    public Dialog[] orders;
    public bool randomizeCustomerOrders = true;

    [HideInInspector]public bool abortCustomerChecking = false;
    [HideInInspector]public GameObject customer;
    [HideInInspector]public GameObject merchant;
    [HideInInspector]public Customer goScript;
    [HideInInspector]public Merchant merchantScript;

    [Header("Custommer Spawn Rate")]
    public float minWait = 1f;
    public float maxWait = 4f;
    private float currentWait;

    [Header("Things for Customer script")]
    public DailyChallenges dailyChallenges;
    public Mission mission;
    public RecipeSystem recipeSys;
    public MouseCursor mouseCursor;
    public PlayerMovement playerMovement;
    public Player_Cam playerCam;
    public Settings settings;
    public TextMeshProUGUI dialogContent;

    public GameObject dialogWindow;
    public GameObject okayButton;
    public GameObject whatButton;
    public GameObject hintButton;
    public GameObject noButton;
    public TextMeshProUGUI patienceCounter;
    public Image emotion;
    public GameObject emotionWindow;

    [Header("Things for Merchant script")]
    public SoundManager sound;
    public BuildSystem buildSystem;
    public BuildMerchant buildMerchant;
    public Supply supply;
    public License license;
    public GameObject merchantWindow;
    public Animator[] merchantContent;
    public Transform parentSpawn;
    public Transform startingPlace;
    public Occupation[] places;
    public Animator windowAnim;
    public TextMeshProUGUI priceVisual;
    public ButtonListener buyButton;

    private bool hasInstantiatedMerchant = false;

    void Start()
    {
        currentWait = Random.Range(minWait, maxWait);
        emotionWindow.SetActive(false);
        merchantWindow.SetActive(false);
    }

    void Update()
    {
        if(main.gameState == PizzaGameState.MainMenu)
        {
            return;
        }

        if(abortCustomerChecking)
        {
            return;
        }

        if(currentWait > 0f && customer == null)
        {
            currentWait -= Time.deltaTime;
        }
        else if(currentWait <= 0f && customer == null)
        {
            currentWait = 0f;
            if(!hasInstantiatedMerchant)
            {
                InstantiateMerchant();
            }
            else
            {
                InstantiateCustomer();
            }
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
                settings.RegisterGetSound();
                stats.moneyGained += goScript.bill;
                emotionWindow.SetActive(true);
                dailyChallenges.store[1].value += goScript.bill;
                brief.canBrief = true;
                brief.ShowPaper();
            }


            if(goScript.state == States.Ending) //End Order
            {
                DeleteCustomer();
                goScript = null;
                dialogWindow.SetActive(false);
                emotionWindow.SetActive(false);
                brief.canBrief = false;
                brief.HidePaper();
                brief.dialog = null;
                pausing.lockMouse = true;
                return;
            }
            
            goScript.state = States.Waiting;
            dialogWindow.SetActive(false);
            pausing.lockMouse = true;
        }
        else if(merchantScript != null)
        {
            merchantScript.OpenMerchantWindow();
        }
    }

    public void AskWhat()
    {
        if(goScript != null)
        {
            goScript.askedWhat = true;
            brief.askedWhat = true;
            goScript.patience = goScript.patience * 0.91f;
            goScript.InitiateTalk(TalkType.What);
            emotionWindow.SetActive(true);
            goScript.UpdatePatience();
        }
    }

    public void AskHint()
    {
        if(goScript != null)
        {
            goScript.askedHint = true;
            brief.askedHint = true;
            goScript.patience = goScript.patience * 0.88f;
            goScript.InitiateTalk(TalkType.Hint);
            emotionWindow.SetActive(true);
            goScript.UpdatePatience();
        }
    }

    public void AskNo()
    {
        DeleteMerchant();
        merchantScript = null;
        dialogWindow.SetActive(false);
        noButton.SetActive(false);
    }

    public void AskBuy()
    {
        if(merchantScript == null)
        {
            return;
        }
        if(merchantScript.cartPrice > settings.money) //Save what is bought at the end of the day, supply and license
        {
            return;
        }

        settings.AddWithoutVisual(-merchantScript.cartPrice);
        merchantScript.BuyCart();
        pausing.lockMouse = true;
    }
    
    public void AskCancel()
    {
        if(merchantScript == null)
        {
            return;
        }

        merchantScript.UnCartAll();
    }

    public void AskLeave()//For Merchant
    {
        if(merchantScript == null)
        {
            return;
        }

        merchantScript.CloseMerchantWindow();
        merchantScript.SelfDestruction();
        dialogWindow.SetActive(false);
        noButton.SetActive(false);
        merchantScript = null;
    }

    public void InstantiateCustomer()
    {
        if(customer != null || merchant != null)
        {
            currentWait = Random.Range(minWait, maxWait);
            return;
        }

        if(rushHour.canBeRushed && rushHour.rushDelay <= 0)
        {
            rushHour.StartRush();
            rushHour.canBeRushed = false;
        }

        GameObject go = Instantiate(customerPrefab, spawnPosition.position, Quaternion.identity);
        sound.Generate2DSound(go.transform.position, sound.customerEnter, true, .5f);
        customer = go;
        goScript = go.GetComponent<Customer>();
        Cosmetics cosmetics = go.GetComponent<Cosmetics>();
        cosmetics.ChangeOutfit();
        cosmetics.ChangeSkin();
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
        goScript.stats = stats;
        goScript.recipeSys = recipeSys;
        goScript.manager = this;
        goScript.brief = brief;
        goScript.mission = mission;
        goScript.pause = pausing;
        goScript.dailyChallenges = dailyChallenges; 

        if(rushHour.rush)
        {
            AdvancedTippingSystem tip = go.GetComponent<AdvancedTippingSystem>();
            rushHour.AddRushCustomer(goScript, tip);
        }
    }

    public void InstantiateMerchant()
    {
        if(merchant != null)
        {
            return;
        }

        GameObject go = Instantiate(merchantPrefab, spawnPosition.position, Quaternion.identity);
        sound.Generate2DSound(go.transform.position, sound.customerEnter, true, .5f);
        merchant = go;
        merchantScript = go.GetComponent<Merchant>();

        merchantScript.settings = settings;
        merchantScript.mouseCursor = mouseCursor;
        merchantScript.playerMovement = playerMovement;
        merchantScript.playerCam = playerCam;
        merchantScript.dialogWindow = dialogWindow;
        merchantScript.dialogContent = dialogContent;
        merchantScript.okayButton = okayButton;
        merchantScript.noButton = noButton;
        merchantScript.manager = this;
        merchantScript.pause = pausing;
        merchantScript.merchantWindow = merchantWindow;
        merchantScript.parentSpawn = parentSpawn;
        merchantScript.windowAnim = windowAnim;
        merchantScript.startingPlace = startingPlace;
        merchantScript.places = places;
        merchantScript.priceVisual = priceVisual;
        merchantScript.supply = supply;
        merchantScript.merchantContent = merchantContent;
        merchantScript.license = license;
        merchantScript.mission = mission;
        merchantScript.buildMerchant = buildMerchant;
        merchantScript.buildSystem = buildSystem;
        merchantScript.sound = sound;
        merchantScript.buyButton = buyButton;

        mission.merchantScript = merchantScript;
        mission.Refresh();

        hasInstantiatedMerchant = true;
        
        buildMerchant.merchant = merchantScript;
        buildMerchant.SendToMerchant();
        
        currentWait = Random.Range(minWait, maxWait);
    }

    public void DeleteCustomer()
    {
        if(customer != null)
        {
            StartCoroutine(animationDeleteCustomer());
        }
    }

    IEnumerator animationDeleteCustomer()
    {
        pausing.lockMouse = true;
        sound.Generate2DSound(customer.transform.position, sound.customerLeave, true, .7f);
        Animator anim = customer.GetComponent<Animator>();
        anim.SetTrigger("Leave");

        float length = 0;
        
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        foreach(AnimationClip clip in ac.animationClips)
        {
            if(clip.name == "CustomerLeave")
            {
                length = clip.length;
            }
        }

        yield return new WaitForSeconds(length);
        Destroy(customer);
        emotionWindow.SetActive(false);
        customer = null;
        currentWait = Random.Range(minWait, maxWait);
    }

    public void DeleteMerchant()
    {
        if(merchant != null)
        {
            StartCoroutine(animationDeleteMerchant());
        }
    }

    IEnumerator animationDeleteMerchant()
    {
        pausing.lockMouse = true;
        mouseCursor.LockCusorState();
        playerMovement.canMove = true;
        playerCam.canMove = true;
        sound.Generate2DSound(merchant.transform.position, sound.customerLeave, true, .7f);
        Animator anim = merchant.GetComponent<Animator>();
        anim.SetTrigger("Leave");

        float length = 0;
        
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        foreach(AnimationClip clip in ac.animationClips)
        {
            if(clip.name == "CustomerLeave")
            {
                length = clip.length;
            }
        }

        yield return new WaitForSeconds(length);
        Destroy(merchant);
        merchant = null;
        currentWait = Random.Range(minWait, maxWait);
    }
    
}
