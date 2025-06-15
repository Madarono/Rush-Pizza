using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public enum LicenseState
{
    Empty,
    Available,
    Bought,
}

[System.Serializable]
public class GuaranteedLicense
{
    public MerchantGoods license;
    public LicenseState licenseState;

    public bool doneAnimation;
}

[System.Serializable]
public class ObjectiveLicense
{
    public int dayRequired;
    public int pizzasRequired;
    public float moneyRequired;
    
    public string licenseNameEnglish;
    public string licenseNameDeutsch;

    public int idForLicense;
}

public class Mission : MonoBehaviour, IDataPersistence
{
    public GuaranteedLicense[] licenses;
    public Merchant merchantScript;
    public bool canSave = false;
    
    [Header("Objective")]
    public ObjectiveLicense[] objective;
    public Settings settings;
    public Stats stats; //Day
    public int pizzasMade;
    public float moneyGained;

    [Header("VisualObjective")]
    public GameObject objectiveWindow;
    public Transform[] objectivePlaces = new Transform[2];
    public float windowSpeed = 7f;
    private bool isOpen = false;

    public TextMeshProUGUI daysVisual;
    public TextMeshProUGUI pizzasVisual;
    public TextMeshProUGUI moneyVisual;

    public Image progressBar;
    public float fillSpeed = 2f;
    
    public TextMeshProUGUI licenseName;
    public Animator objectiveWindowAniamtor;
    // public float animationWait;

    private bool doneReward = false;
    
    private float percentage;
    public int objectiveId;

    private List<int> saveState = new List<int>(); //For LicenseState
    private int[] intSaveState;

    private List<bool> animationSave = new List<bool>();//For DoneAnimation
    private bool[] boolSave;


    public void SaveData(GameData data)
    {
        if(canSave)
        {
            ChangeStateIntoInt();
            ChangeAnimationIntoBool();
            data.animationSave = this.animationSave.ToArray();
            data.saveState = this.saveState.ToArray();
            data.pizzasMade = this.pizzasMade;
            data.moneyGained = this.moneyGained;
            data.percentage = this.percentage;
        }
    }

    public void LoadData(GameData data)
    {
        this.boolSave = data.animationSave;
        this.intSaveState = data.saveState;
        this.pizzasMade = data.pizzasMade;
        this.moneyGained = data.moneyGained;
        this.percentage = data.percentage;

        progressBar.fillAmount = percentage;

        ChangeIntIntoState();
        ChangeBoolIntoAnimation();
        CheckRequirements();
    }

    public void Refresh()
    {
        if(merchantScript == null)
        {
            return;
        }
        merchantScript.guarenteedGoods.Clear();

        List<MerchantGoods> availableLicenses = new List<MerchantGoods>();
        
        for(int i = 0; i < licenses.Length; i++)
        {
            if(licenses[i].licenseState == LicenseState.Available)
            {
                availableLicenses.Add(licenses[i].license);
            }
        }

        merchantScript.guarenteedGoods = availableLicenses;
    }

    public void BoughtItem(MerchantGoods goods)
    {
        for(int i = 0; i < licenses.Length; i++)
        {
            if(licenses[i].license == goods)
            {
                licenses[i].licenseState = LicenseState.Bought;
                break;
            }
        }

        Refresh();
    }

    void ChangeAnimationIntoBool()
    {
        foreach(GuaranteedLicense guaranteed in licenses)
        {
            animationSave.Add(guaranteed.doneAnimation);
        }
    }

    void ChangeBoolIntoAnimation()
    {
        if(boolSave.Length == 0)
        {
            return;
        }

        for(int i = 0; i < boolSave.Length; i++)
        {
            licenses[i].doneAnimation = boolSave[i];
        }
    }

    void ChangeStateIntoInt()
    {
        foreach(GuaranteedLicense guaranteed in licenses)
        {
            switch(guaranteed.licenseState)
            {
                case LicenseState.Empty:
                    saveState.Add(0);
                    break;

                case LicenseState.Available:
                    saveState.Add(1);
                    break;

                case LicenseState.Bought:
                    saveState.Add(2);
                    break;

                default:
                    Debug.Log("No compatible type");
                    break;
            }
        }
    }

    void ChangeIntIntoState()
    {
        if(intSaveState.Length == 0)
        {
            return;
        }

        for(int i = 0; i < licenses.Length; i++)
        {
            switch(intSaveState[i])
            {
                case 0:
                    licenses[i].licenseState = LicenseState.Empty;
                    break;

                case 1:
                    licenses[i].licenseState = LicenseState.Available;
                    break;

                case 2:
                    licenses[i].licenseState = LicenseState.Bought;
                    break;

                default:
                    Debug.Log("No compatible integer");
                    break;
            }
        }
    }

    public void CheckRequirements()
    {
        int days = stats.day;

        foreach(ObjectiveLicense obj in objective)
        {
            if(days >= obj.dayRequired && pizzasMade >= obj.pizzasRequired && moneyGained >= obj.moneyRequired)
            {
                if(licenses[obj.idForLicense].licenseState == LicenseState.Empty)
                {
                    licenses[obj.idForLicense].licenseState = LicenseState.Available;
                }
            }
        }

        Refresh();
    }
    
    public void BothObjectiveVisual()
    {
        isOpen = !isOpen;

        if(!isOpen || objectiveId == 0)
        {
            return;
        }

        if(licenses[objectiveId - 1].licenseState == LicenseState.Available && !licenses[objectiveId - 1].doneAnimation)
        {
            Reward();
        }
    }

    public void UpdateVisual()
    {
        int days = stats.day;

        for(int i = 0; i < objective.Length; i++)
        {
            if(days < objective[i].dayRequired || pizzasMade < objective[i].pizzasRequired || moneyGained < objective[i].moneyRequired)
            {
                objectiveId = i;
                break;
            }
        }

        daysVisual.text = days.ToString() + "/" + objective[objectiveId].dayRequired;
        pizzasVisual.text = pizzasMade.ToString() + "/" + objective[objectiveId].pizzasRequired;
        if(settings.english)
        {
            moneyVisual.text = moneyGained.ToString("F2") + "/" + objective[objectiveId].moneyRequired;
        }
        else
        {
            moneyVisual.text = moneyGained.ToString("F2", new CultureInfo("de-DE")) + "/" + objective[objectiveId].moneyRequired;
        }

        percentage = ((Mathf.Clamp(days / objective[objectiveId].dayRequired, 0f, 1f)) + (Mathf.Clamp(pizzasMade / objective[objectiveId].pizzasRequired, 0f, 1f)) + (Mathf.Clamp(moneyGained / objective[objectiveId].moneyRequired, 0f, 1f))) / 3f;
    }

    public void Reward()
    {
        UpdateLicenseName();
        objectiveWindowAniamtor.SetTrigger("NewLicense");
        StartCoroutine(ShowNewInformation());
        licenses[objectiveId - 1].doneAnimation = true;
    }

    IEnumerator ShowNewInformation()
    {
        yield return new WaitForSeconds(2f);
        UpdateVisual();
    }

    public void UpdateLicenseName()
    {
        if(objectiveId == 0)
        {
            return;
        }
        
        if(settings.english)
        {
            licenseName.text = "(" + objective[objectiveId - 1].licenseNameEnglish + ")";
        }
        else
        {
            licenseName.text = "(" + objective[objectiveId - 1].licenseNameDeutsch + ")";
        }
    }

    void FixedUpdate()
    {
        if(isOpen)
        {
            objectiveWindow.transform.position = Vector3.Lerp(objectiveWindow.transform.position, objectivePlaces[1].position, windowSpeed * Time.unscaledDeltaTime);
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, percentage, fillSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            objectiveWindow.transform.position = Vector3.Lerp(objectiveWindow.transform.position, objectivePlaces[0].position, windowSpeed * Time.unscaledDeltaTime);
        }
    }
}
