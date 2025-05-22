using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public enum MerchantStates
{
    Static,
    Talking
}

[System.Serializable]
public class Occupation
{
    public Transform place;
    public bool occupied = false;
}

[System.Serializable]
public class ItemTypeVisual
{
    public ItemType itemType;
    public Color visualColor;
    public string infoEnglish;
    public string infoDeutch;
}

public class Merchant : MonoBehaviour
{
    public MerchantStates state;
    [HideInInspector]public BuildSystem buildSystem;
    [HideInInspector]public BuildMerchant buildMerchant;
    [HideInInspector]public Mission mission;
    [HideInInspector]public License license;
    [HideInInspector]public Supply supply;
    [HideInInspector]public Pausing pause;
    [HideInInspector]public CustomerManager manager;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;
    
    [Header("Speaking")]
    public Lines[] merchantLines;
    public float speedOfTalk = 0.03f;
    public float delayOfVoice = 0.1f;

    private string cacheLines;
    private bool skipLines = false; 

    [Header("Visual")]
    public GameObject dialogWindow;
    private DialogBox dialogBox;
    public TextMeshProUGUI dialogContent;
    public GameObject okayButton;
    public GameObject noButton;
    
    public Settings settings;
    public SpecialStrings[] specialCharcters;

    [Header("MercahantWindow")]
    public GameObject merchantWindow;
    public Animator[] merchantContent;
    public Transform parentSpawn;
    public Animator windowAnim;
    public float delayOfLeaving = 0.9f;

    [Header("Generating Goods")]
    public MerchantGoods[] goods;
    [Range(0, 4)] public int range;
    public List<MerchantGoods> guarenteedGoods = new List<MerchantGoods>();
    public GameObject goodsPrefab;


    [Header("Item Types")]
    public ItemTypeVisual[] itemVisual;

    [Header("Cart")]
    public GameObject displayPrefab;
    public Occupation[] places;
    public Transform startingPlace;
    public float cartPrice;
    public TextMeshProUGUI priceVisual;
    public List<MerchantItem> inCartItems;
    public List<MerchantDisplay> inCartDisplay;

    [HideInInspector]public ButtonListener buyButton;
    [HideInInspector]public SoundManager sound;

    void Start()
    {
        RefreshPrice();
        dialogBox = dialogWindow.GetComponent<DialogBox>();
    }
    
    void Update()
    {
        if(skipLines && Input.GetMouseButtonDown(0))
        {
            SkipTalk();
        }

        if(settings.money >= cartPrice && cartPrice > 0)
        {
            buyButton.overrideClip = sound.buyMerchant;
        }
        else
        {
            buyButton.overrideClip = null;
        }
    }

    public void InitiateTalk()
    {
        state = MerchantStates.Talking;
        pause.lockMouse = false;

        StopAllCoroutines();
        dialogWindow.SetActive(true);
        if(settings.english)
        {
            int random = Random.Range(0, merchantLines.Length);
            StartCoroutine(ShowText(merchantLines[random].englishVersion)); 
            if(settings.enableVoice)
            {
                StartCoroutine(MakeVoice(merchantLines[random].englishVersion));
            }
            StartCoroutine(EnableSkipLines());
            cacheLines = merchantLines[random].englishVersion;

            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
        else
        {
            int random = Random.Range(0, merchantLines.Length);
            StartCoroutine(ShowText(merchantLines[random].deutschVersion)); 
            if(settings.enableVoice)
            {
                StartCoroutine(MakeVoice(merchantLines[random].deutschVersion));
            }
            StartCoroutine(EnableSkipLines());
            cacheLines = merchantLines[random].deutschVersion;
              
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
    }

    IEnumerator ShowText(string content)
    {
        dialogWindow.SetActive(true);
        dialogBox.CheckDimentions(content);

        okayButton.SetActive(true);
        noButton.SetActive(true);

        dialogContent.text = "";
        for(int i = 0; i < content.Length; i++)
        {
            float delay = speedOfTalk;
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

        StopAllCoroutines();
        skipLines = false;
    }

    IEnumerator MakeVoice(string content)
    {
        for(int i = 0; i < content.Length; i++)
        {
            int randomVoice = Random.Range(0, manager.sound.customerVoice.Length);
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

    //Merchant
    public void GenerateItems()
    {
        if(parentSpawn.childCount > 0)
        {
            foreach(Transform child in parentSpawn)
            {
                Destroy(child.gameObject);
            }
        }

        int leftOfSpace = 4;

        if(guarenteedGoods.Count > 0)
        {
            while(guarenteedGoods.Count > range)
            {
                guarenteedGoods.RemoveAt(guarenteedGoods.Count - 1);
            }

            for(int i = 0; i < guarenteedGoods.Count; i++)
            {
                InstantiateItem(guarenteedGoods[i]);
                leftOfSpace--;
            }
        }

        if(leftOfSpace == 0)
        {
            Debug.Log("No space left for available goods");
            return;
        }

        List<MerchantGoods> availableGoods = new List<MerchantGoods>();

        foreach(MerchantGoods goodie in goods)
        {
            for(int i = 0; i < settings.ingrediantsAvailable.Length; i++)
            {
                if(goodie.ingrediantType == settings.ingrediantsAvailable[i] || goodie.neutral)
                {
                    availableGoods.Add(goodie);
                    break;
                }
            }
        }

        if(availableGoods.Count == 0)
        {
            Debug.Log("No Goodies");
            return;
        }

        for(int i = 0; i < leftOfSpace; i++)
        {
            int goodChoosing = Random.Range(0, availableGoods.Count);
            InstantiateItem(availableGoods[goodChoosing]);
            if(availableGoods[goodChoosing].putOnce)
            {
                availableGoods.Remove(availableGoods[goodChoosing]);
                
                if(availableGoods.Count == 0) //If the decor was the only one there, then break to avoid errors
                {
                    break;
                }
            }
        }
    }

    void InstantiateItem(MerchantGoods goodies)
    {
        GameObject go = Instantiate(goodsPrefab, Vector3.one, Quaternion.identity);
        go.transform.SetParent(parentSpawn);
        go.transform.position = parentSpawn.position;
        go.transform.rotation = parentSpawn.rotation;
        go.transform.localScale = Vector3.one;
        MerchantItem item = go.GetComponent<MerchantItem>();
        
        if(settings.english)
        {
            item.info.text = goodies.infoEnglish;
        }
        else
        {
            item.info.text = goodies.infoDeutsch;
        }
        
        for(int o = 0; o < itemVisual.Length; o++)
        {
            if(goodies.itemType == itemVisual[o].itemType)
            {
                if(settings.english)
                {
                    item.headerType.text = itemVisual[o].infoEnglish + " - $" + goodies.price.ToString("F2");
                }
                else
                {
                    item.headerType.text = itemVisual[o].infoDeutch + " - €" + goodies.price.ToString("F2", new CultureInfo("de-DE"));
                }
                item.headerType.color = itemVisual[o].visualColor;
                break;
            }
        }
        item.merchant = this;
        item.goods = goodies;
        item.icon.sprite = goodies.icon;
    }

    public void AddToCart(MerchantItem item)
    {
        Transform placeToGo = null;

        for(int i = 0; i < places.Length; i++)
        {
            if(places[i].occupied)
            {
                continue;
            }

            places[i].occupied = true;
            placeToGo = places[i].place;
            break;
        }

        if(placeToGo != null)
        {
            GameObject go = Instantiate(displayPrefab, startingPlace.position, Quaternion.identity);
            go.transform.SetParent(startingPlace);
            go.transform.position = startingPlace.position;
            go.transform.rotation = startingPlace.rotation;
            MerchantDisplay display = go.GetComponent<MerchantDisplay>();

            display.placeToGo = placeToGo;
            display.merchant = this;
            display.merchantItem = item;
            display.icon.sprite = item.icon.sprite;
            display.price = item.goods.price;
            display.icon.sprite = item.icon.sprite;

            item.gameObject.SetActive(false);
            inCartItems.Add(item);
            inCartDisplay.Add(display);
            RefreshPrice();
        }
    }

    public void UnCart(MerchantItem item, MerchantDisplay display)
    {
        inCartItems.Remove(item);
        inCartDisplay.Remove(display);
        item.gameObject.SetActive(true);
        Destroy(display.gameObject);
        RefreshCart();
        RefreshPrice();
    }
    
    public void UnCartAll()
    {
        for(int i = inCartDisplay.Count - 1; i >= 0; i--)
        {
            UnCart(inCartItems[i], inCartDisplay[i]);
        }
    }

    public void BuyCart()
    {
        if(inCartDisplay.Count == 0)
        {
            return;
        }

        for(int i = inCartDisplay.Count - 1; i >= 0; i--)
        {
            Destroy(inCartDisplay[i].gameObject);
            inCartDisplay.Remove(inCartDisplay[i]);

            switch(inCartItems[i].goods.itemType)
            {
                case ItemType.Supply:
                    supply.PutToSupply(inCartItems[i].goods.ingrediantType, inCartItems[i].goods.supplyCount);
                    break;

                case ItemType.License:
                    license.InstantiateBox(inCartItems[i].goods.licenseID);
                    mission.BoughtItem(inCartItems[i].goods);
                    break;

                case ItemType.Decoration:
                    buildSystem.AddToInventory(inCartItems[i].goods.decorID);
                    buildSystem.VisualizeNotification(inCartItems[i].goods.decorID);
                    buildMerchant.UpdateSold(inCartItems[i].goods);
                    break;

                default:
                    Debug.Log("No type detected");
                    break;
            }
            
            Destroy(inCartItems[i].gameObject);
            inCartItems.Remove(inCartItems[i]);
        }

        //Supply.cs for supply and license for license here

        RefreshPrice();
        RefreshCart();
    }

    public void RefreshPrice()
    {
        cartPrice = 0f;

        for(int i = 0; i < inCartDisplay.Count; i++)
        {
            cartPrice += inCartDisplay[i].price;
        }

        if(settings.english)
        {
            priceVisual.text = "$" + cartPrice.ToString("F2");
        }
        else
        {
            priceVisual.text = "€" + cartPrice.ToString("F2", new CultureInfo("de-DE"));
        }
    }

    public void RefreshCart()
    {
        for(int i = 0; i < places.Length; i++)
        {
            places[i].occupied = false;
        }

        foreach(MerchantDisplay display in inCartDisplay)
        {
            for(int i = 0; i < places.Length; i++)
            {
                if(!places[i].occupied)
                {
                    places[i].occupied = true;
                    display.placeToGo = places[i].place;
                    break;
                }
            }
        }
    }

    public void OpenMerchantWindow()
    {
        pause.lockMouse = false;
        playerMovement.canMove = false;
        playerCam.canMove = false;
        mouseCursor.FreeCusorState();
        GenerateItems();
        
        Time.timeScale = 0f;
        merchantWindow.SetActive(true);
    }

    public void CloseMerchantWindow()
    {
        mouseCursor.LockCusorState();
        Time.timeScale = 1f;
        pause.lockMouse = true;
        StartCoroutine(WaitForPauseClosing());
    }

    public void SelfDestruction()
    {
        StartCoroutine(Destruction());
    }

    IEnumerator Destruction()
    {
        yield return new WaitForSeconds(delayOfLeaving);
        manager.DeleteMerchant();
    }

    IEnumerator WaitForPauseClosing()
    {
        windowAnim.SetTrigger("Close");
        foreach(Animator anim in merchantContent)
        {
            anim.SetTrigger("Close");
        }
        yield return new WaitForSeconds(delayOfLeaving);
        merchantWindow.SetActive(false);
    }
}
