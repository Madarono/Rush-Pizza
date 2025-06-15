using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public enum CameraRenderingPath
{
    Forward,
    Deferred
}

[System.Serializable]
public class Keybinds
{
    public KeyCode keybind;
    public TextMeshProUGUI visual;
}

[System.Serializable]
public class QualityVisual
{
    public string english;
    public string deutsch;
    public CameraRenderingPath rendering;
}

[System.Serializable]
public class ResolutionLevel
{
    public string visual;
    public int width;
    public int length;
}

public class Controls : WindowOpening, IDataPersistence
{
    [Header("Scripts")]
    public DailyChallenges dailyChallenge;
    public Tutorial tutorial;
    public Player_Cam playerCam;
    public Stats stats;
    public RecipeSystem RecipeSystem;
    public Mission mission;
    public Fps fpsScript;
    public RushHour rushHour;

    [Header("Leaving")]
    public GameObject sureWindow;
    public Animator sureAnim;
    public float sureLeaveDuration = 1f;

    [Header("Keybinds")]
    public Pausing pause;
    public Keybinds[] keybinds;
    public bool change;
    public int index;
    private KeyCode key;

    [Header("Duplicate Keybinds")]
    public Color[] visualColors;

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
    public QualityVisual[] qualityString;

    public int resolutionChoosing = 1;
    public ResolutionLevel[] resolutionLevels;
    public TextMeshProUGUI resolutionVisual;

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
    public bool enableVoice = true;
    public TextMeshProUGUI voiceVisual;

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
    
    [Header("Sensitivity")]
    public float minSens;
    public float maxSens;
    public float currentSens;
    public Slider sensSlider;
    public TextMeshProUGUI sensVisual;

    [Header("VSync")]
    public bool vsync;
    public TextMeshProUGUI vsyncVisual;


    public void SaveData(GameData data)
    {
        data.english = this.english;
        data.crouch = this.keybinds[0].keybind;
        data.throwKey = this.keybinds[1].keybind;
        data.sprint = this.keybinds[2].keybind;
        data.pause = this.keybinds[3].keybind;
        data.zoom = this.keybinds[4].keybind;
        data.buildMode = this.keybinds[5].keybind;
        data.changeMode = this.keybinds[6].keybind;
        data.dailyChallenge = this.keybinds[7].keybind;
        data.showBrief = this.keybinds[8].keybind;
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
        data.enableVoice = this.enableVoice;
        data.resolutionChoosing = this.resolutionChoosing;
        data.vsync = this.vsync;
        data.cameraSens = this.currentSens;
    }

    public void LoadData(GameData data)
    {
        this.english = data.english;
        this.keybinds[0].keybind = data.crouch;
        this.keybinds[1].keybind = data.throwKey;
        this.keybinds[2].keybind = data.sprint;
        this.keybinds[3].keybind = data.pause;
        this.keybinds[4].keybind = data.zoom;
        this.keybinds[5].keybind = data.buildMode;
        this.keybinds[6].keybind = data.changeMode;
        this.keybinds[7].keybind = data.dailyChallenge;
        this.keybinds[8].keybind = data.showBrief;
        this.h24Format = data.h24Format;
        this.holdCrouch = data.holdCrouch;
        this.showFPS = data.showFPS;
        this.choosingFPS = data.choosingFPS;
        this.volumeValues = data.volumeValues;
        this.quality = data.quality;
        this.currentFov = data.cameraFOV;
        this.master = data.master;
        this.background = data.background;
        this.enableVoice = data.enableVoice;
        this.resolutionChoosing = data.resolutionChoosing;
        this.vsync = data.vsync;
        this.currentSens = data.cameraSens;
        
        if(timeChanges != null)
        {
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
        
        Screen.SetResolution(resolutionLevels[resolutionChoosing].width, resolutionLevels[resolutionChoosing].length, true);
        ApplyToSettings();
        UpdateKeybinds();
        UpdateModifications();
        GetPostProcessing();
        UpdatePostProcessing();
        UpdateAudioSliders();
        // UpdateAllPPVisual();
        UpdateCameraFOV();
        UpdateSensitivity();
        UpdateQualitySettngs(quality);
        fpsScript.vsync = this.vsync;
        fpsScript.ToggleVsync();
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

        switch(qualityString[quality].rendering)
        {
            case CameraRenderingPath.Forward:
                camera.renderingPath = RenderingPath.Forward;
                break;

            case CameraRenderingPath.Deferred:
                camera.renderingPath = RenderingPath.DeferredShading;
                break;

            default:
                camera.renderingPath = RenderingPath.Forward;
                break;
        }

        
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
        settings.buildMode = this.keybinds[5].keybind;
        settings.changeMode = this.keybinds[6].keybind;
        settings.dailyChallenge = this.keybinds[7].keybind;
        settings.showBrief = this.keybinds[8].keybind;
        settings.h24Format = this.h24Format;
        settings.holdCrouch = this.holdCrouch;
        settings.enableVoice = this.enableVoice;
        settings.RefreshMoneyCounter();

        if(timeChanges != null)
        {
            timeChanges.h24Format = settings.h24Format;
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
        if(!tutorial.hasCompleted)
        {
            tutorial.RefreshSubtitles();
        }
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
        english = makeEnglish;
        UpdateModifications();
        ApplyToSettings();

        rushHour.StopAllCoroutines();
        rushHour.RepeatGlitch(rushHour.repeat, rushHour.delayGlitch / 5f, rushHour.delayOriginal / 5f);

        dailyChallenge.RefreshItems();
        if(settings.canSaveMoney) //Just means that the day ended
        {
            stats.UpdateValues();
            mission.UpdateVisual();
        }

        if(timeChanges != null)
        {
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
    }
    public void ToggleVoice()
    {
        enableVoice = !enableVoice;
        UpdateModifications();
        ApplyToSettings();
    }
    public void ChangeResolution()
    {
        resolutionChoosing++;
        if(resolutionChoosing >= resolutionLevels.Length)
        {
            resolutionChoosing = 0;
        } 
        Screen.SetResolution(resolutionLevels[resolutionChoosing].width, resolutionLevels[resolutionChoosing].length, true);
        UpdateModifications();
    }

    public void UpdateModifications()
    {
        if(english)
        {
            crouchVisual.text = holdCrouch ? "Hold" : "Toggle";
            formatVisual.text = h24Format ? "24 Hour" : "12 Hour";
            fpsVisual.text = showFPS ? "Show" : "Hide";
            voiceVisual.text = enableVoice ? "On" : "Off";
            vsyncVisual.text = vsync ? "On" : "Off";
        }
        else
        {
            formatVisual.text = h24Format ? "24 Stunde" : "12 Stunde";
            crouchVisual.text = holdCrouch ? "Halt" : "Umschalten";
            fpsVisual.text = showFPS ? "Anzeigen" : "Ausblenden";
            voiceVisual.text = enableVoice ? "Ein" : "Aus";
            vsyncVisual.text = vsync ? "Ein" : "Aus";
        }

        resolutionVisual.text = resolutionLevels[resolutionChoosing].visual;
        fpsScript.showFPS = showFPS;
        limitVisual.text = limit[choosingFPS].ToString();
        sensVisual.text = currentSens.ToString("F0");
        fpsScript.ChangeFpsLimit(limit[choosingFPS]);
        dailyChallenge.RefreshItems();
    }
    void UpdateCameraFOV()
    {
        if(camera != null)
        {
            camera.fieldOfView = currentFov;
        }

        fovSlider.value = (currentFov - minFov) / (maxFov - minFov);
    }
    void UpdateSensitivity()
    {
        sensSlider.value = (currentSens - minSens) / (maxSens - minSens);
        playerCam.sensX = currentSens;
        playerCam.sensY = currentSens;
    }
    void UpdateAudioSliders()
    {
        masterSlider.value = master;
        backgroundSlider.value = background;
    }

    public void ChangeSensitivity()
    {
        float sliderPercentage = sensSlider.value;

        currentSens = Mathf.Lerp(minSens, maxSens, sliderPercentage);
        sensVisual.text = currentSens.ToString("F0");
        UpdateSensitivity();
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
        if(vsync)
        {
            return;
        }

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
    public void ChangeVSync()
    {
        vsync = !vsync;
        fpsScript.vsync = this.vsync;
        fpsScript.ToggleVsync();

        if(settings.english)
        {
            vsyncVisual.text = vsync ? "On" : "Off";
        }
        else
        {
            vsyncVisual.text = vsync ? "Ein" : "Aus";
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
        CheckKeybinds();
        ApplyToSettings();

        if(pause != null)
        {
            StartCoroutine(EnableCanPause());
        }
    }

    void CheckKeybinds()
    {
        foreach(Keybinds bind in keybinds)
        {
            bind.visual.color = visualColors[1];
        }


        for(int i = 0; i < keybinds.Length; i++)
        {
            for(int o = 0; o < keybinds.Length; o++)
            {
                if(o == i)
                {
                    continue;
                }

                if(keybinds[i].keybind == keybinds[o].keybind)
                {
                    Debug.Log(i + ", and " + o + " have the same Keybind");
                    keybinds[i].visual.color = visualColors[0];
                    keybinds[o].visual.color = visualColors[0];
                    break;
                }
            }
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
        CheckKeybinds();
    }

    public void Leave()
    {
        sureWindow.SetActive(true);
    }

    public void StopLeave()
    {
        StartCoroutine(TurnOffSureWindow());
    }

    public void ConfirmLeave()
    {
        StartCoroutine(LeaveGame());
    }

    IEnumerator LeaveGame()
    {
        stats.nextDayScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(stats.nextDayDelay);
        DataPersistenceManager.instance.SaveGame();
        Application.Quit();
    }

    IEnumerator TurnOffSureWindow()
    {
        sureAnim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(sureLeaveDuration);
        sureWindow.SetActive(false);
    }
}
