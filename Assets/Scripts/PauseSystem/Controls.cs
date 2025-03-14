using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

[System.Serializable]
public class Keybinds
{
    public KeyCode keybind;
    public TextMeshProUGUI visual;
}

[System.Serializable]
public class LanguageVisual
{
    public string english;
    public string deutsch;
}

public class Controls : WindowOpening, IDataPersistence
{
    [Header("Scripts")]
    public RecipeSystem RecipeSystem;
    public Mission mission;
    public Fps fpsScript;

    [Header("Keybinds")]
    public Pausing pause;
    public Keybinds[] keybinds;
    public bool change;
    public int index;
    private KeyCode key;

    [Header("Graphics")]
    public PostProcessVolume volume; //Post processing volume
    public TextMeshProUGUI[] volumeVisual = new TextMeshProUGUI[5];

    private Bloom bloom;
    private AmbientOcclusion ao;
    private ColorGrading cg;
    private MotionBlur blur;
    private DepthOfField field;

    public bool[] volumeValues = new bool[5];
    [Range (0,5)] public int quality = 5;
    public TextMeshProUGUI qualityVisual;
    public LanguageVisual[] qualityString;

    [Header("Audio")]
    public float master;
    public Slider masterSlider;
    public TextMeshProUGUI masterVisual;

    public float background;
    public Slider backgroundSlider;
    public TextMeshProUGUI backgroundVisual;

    [Header("Extras")]
    public Settings settings;
    public TimeChanges timeChanges;
    public bool english;
    public bool holdCrouch;
    public TextMeshProUGUI crouchVisual;
    public bool h24Format;
    public TextMeshProUGUI formatVisual;

    public Camera camera;
    public Slider fovSlider;
    public float minFov = 30f;
    public float maxFov = 120f;
    public float currentFov = 60f;
    public TextMeshProUGUI fovVisual;
    
    [Header("FPS")]
    public bool showFPS;
    public TextMeshProUGUI fpsVisual;
    public int choosingFPS;
    public int[] limit;
    public TextMeshProUGUI limitVisual;


    public void SaveData(GameData data)
    {
        data.english = this.english;
        data.crouch = this.keybinds[0].keybind;
        data.throwKey = this.keybinds[1].keybind;
        data.sprint = this.keybinds[2].keybind;
        data.pause = this.keybinds[3].keybind;
        data.zoom = this.keybinds[4].keybind;
        data.h24Format = this.h24Format;
        data.holdCrouch = this.holdCrouch;
        data.showFPS = this.showFPS;
        data.choosingFPS = this.choosingFPS;
        data.volumeValues = this.volumeValues;
        data.volumeValues = this.volumeValues;
        data.quality = this.quality;
        data.cameraFOV = this.currentFov;
        data.master = this.master;
        data.background = this.background;
    }

    public void LoadData(GameData data)
    {
        this.english = data.english;
        this.keybinds[0].keybind = data.crouch;
        this.keybinds[1].keybind = data.throwKey;
        this.keybinds[2].keybind = data.sprint;
        this.keybinds[3].keybind = data.pause;
        this.keybinds[4].keybind = data.zoom;
        this.h24Format = data.h24Format;
        this.holdCrouch = data.holdCrouch;
        this.showFPS = data.showFPS;
        this.choosingFPS = data.choosingFPS;
        this.volumeValues = data.volumeValues;
        this.quality = data.quality;
        this.currentFov = data.cameraFOV;
        this.master = data.master;
        this.background = data.background;
        
        if(timeChanges != null)
        {
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
        
        ApplyToSettings();
        UpdateKeybinds();
        UpdateModifications();
        GetPostProcessing();
        UpdatePostProcessing();
        UpdateAudioSliders();
        // UpdateAllPPVisual();
        UpdateCameraFOV();
        UpdateQualitySettngs(quality);
    }

    void GetPostProcessing()
    {
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out ao);
        volume.profile.TryGetSettings(out cg);
        volume.profile.TryGetSettings(out blur);
        volume.profile.TryGetSettings(out field);
    }

    void UpdatePostProcessing()
    {
        bloom.enabled.value = volumeValues[0];
        ao.enabled.value = volumeValues[1];
        cg.enabled.value = volumeValues[2];
        blur.enabled.value = volumeValues[3];
        field.enabled.value = volumeValues[4];
    }

    public void UpdateAllPPVisual()
    {
        for(int i = 0; i < volumeValues.Length; i++)
        {
            if(settings.english)
            {
                volumeVisual[i].text = volumeValues[i] ? "On" : "Off";
            }
            else
            {
                volumeVisual[i].text = volumeValues[i] ? "Ein" : "Aus";
            }   
        }
    }

    public void UpdateQualitySettngs(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        
        if(settings.english)
        {
            qualityVisual.text = qualityString[quality].english;
        }
        else
        {
            qualityVisual.text = qualityString[quality].deutsch;
        }
    }

    void ApplyToSettings()
    {
        if(settings == null)
        {
            return;
        }

        settings.english = this.english;
        mission.UpdateLicenseName();
        settings.crouch = this.keybinds[0].keybind;
        settings.throwKey = this.keybinds[1].keybind;
        settings.sprint = this.keybinds[2].keybind;
        settings.pause = this.keybinds[3].keybind;
        settings.zoom = this.keybinds[4].keybind;
        settings.h24Format = this.h24Format;
        settings.holdCrouch = this.holdCrouch;
        settings.RefreshMoneyCounter();
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        RecipeSystem.CloseWindow();
    }

    void Update()
    {
        if(Input.anyKeyDown && change)
        {
            KeyCode detectedKey = KeyCode.None;

            // Detect special keys
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    detectedKey = keyCode;
                    break;
                }
            }

            if (detectedKey != KeyCode.None)
            {
                key = detectedKey;
                // Debug.Log("Pressed Key: " + key);
                ChangeKey(key);
                change = false;
            }
        }
    }

    //Extras Window
    public void ChangeLanguage(bool makeEnglish)
    {
        if(makeEnglish)
        {
            english = true;
        }
        else
        {
            english = false;
        }

        UpdateModifications();
        ApplyToSettings();

        if(timeChanges != null)
        {
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
    }

    public void UpdateModifications()
    {
        if(english)
        {
            crouchVisual.text = holdCrouch ? "Hold" : "Toggle";
            formatVisual.text = h24Format ? "24 Hour" : "12 Hour";
            fpsVisual.text = showFPS ? "Show" : "Hide";
        }
        else
        {
            formatVisual.text = h24Format ? "24 Stunde" : "12 Stunde";
            crouchVisual.text = holdCrouch ? "Halt" : "Umschalten";
            fpsVisual.text = showFPS ? "Anzeigen" : "Ausblenden";
        }

        fpsScript.showFPS = showFPS;
        limitVisual.text = limit[choosingFPS].ToString();
        fpsScript.ChangeFpsLimit(limit[choosingFPS]);
    }
    void UpdateCameraFOV()
    {
        if(camera != null)
        {
            camera.fieldOfView = currentFov;
        }

        fovSlider.value = (currentFov - minFov) / (maxFov - minFov);
    }
    void UpdateAudioSliders()
    {
        masterSlider.value = master;
        backgroundSlider.value = background;
    }

    public void ChangeHoldCrouch()
    {
        holdCrouch = !holdCrouch;
        if(english)
        {
            crouchVisual.text = holdCrouch ? "Hold" : "Toggle";
        }
        else
        {
            crouchVisual.text = holdCrouch ? "Halt" : "Umschalten";
        }
        ApplyToSettings();
    }
    public void ChangeFormat()
    {
        h24Format = !h24Format;
        if(english)
        {
            formatVisual.text = h24Format ? "24 Hour" : "12 Hour";
        }
        else
        {
            formatVisual.text = h24Format ? "24 Stunde" : "12 Stunde";
        }
        ApplyToSettings();
    }
    public void ChangePostProcessing(int index)
    {
        volumeValues[index] = !volumeValues[index];

        if(settings.english)
        {
            volumeVisual[index].text = volumeValues[index] ? "On" : "Off";
        }
        else
        {
            volumeVisual[index].text = volumeValues[index] ? "Ein" : "Aus";
        }

        UpdatePostProcessing();
    }
    public void ChangeQuality()
    {
        int c_quality = quality;
        c_quality++;
        if(c_quality > 5)
        {
            c_quality = 0;
        }
        quality = c_quality;

        UpdateQualitySettngs(quality);
    }
    public void ChangeFPS()
    {
        showFPS = !showFPS;
        if(english)
        {
            fpsVisual.text = showFPS ? "Show" : "Hide";
        }
        else
        {
            fpsVisual.text = showFPS ? "Anzeigen" : "Ausblenden";
        }

        fpsScript.showFPS = this.showFPS;
    }
    public void ChangeFPSLimit()
    {
        choosingFPS++;
        if(choosingFPS >= limit.Length)
        {
            choosingFPS = 0;
        }

        for(int i = 0; i < limit.Length; i++)
        {
            if(choosingFPS == i)
            {
                fpsScript.ChangeFpsLimit(limit[i]);
                limitVisual.text = limit[i].ToString();
                break;
            }
        }
    }
    public void ChangeFOV()
    {
        float sliderPercentage = fovSlider.value;

        currentFov = Mathf.Lerp(minFov, maxFov, sliderPercentage);
        fovVisual.text = currentFov.ToString("F0");
        
        UpdateCameraFOV();
    }
    public void ChangeMaster()
    {
        master = masterSlider.value;
        float perctenageVisual = master * 100f;
        masterVisual.text = perctenageVisual.ToString("F0");
    }
    public void ChangeBackground()
    {
        background = backgroundSlider.value;
        float perctenageVisual = background * 100f;
        backgroundVisual.text = perctenageVisual.ToString("F0");
    }

    //Keybinds Window
    public void ConfigureKeys(int id)
    {
        keybinds[id].visual.text = "...";
        index = id;
        change = true;

        if(pause != null)
        {
            pause.canPause = false;
        }
    }

    void ChangeKey(KeyCode key)
    {
        keybinds[index].visual.text = key.ToString();
        keybinds[index].keybind = key;
        ApplyToSettings();

        if(pause != null)
        {
            StartCoroutine(EnableCanPause());
        }
    }

    IEnumerator EnableCanPause()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        pause.canPause = true;
    }

    void UpdateKeybinds()
    {
        for(int i = 0; i < keybinds.Length; i++)
        {
            keybinds[i].visual.text = keybinds[i].keybind.ToString();
        }
    }
}
