using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Keybinds
{
    public KeyCode keybind;
    public TextMeshProUGUI visual;
}

public class Controls : WindowOpening, IDataPersistence
{
    [Header("Scripts")]
    public RecipeSystem RecipeSystem;
    public Fps fpsScript;

    [Header("Keybinds")]
    public Pausing pause;
    public Keybinds[] keybinds;
    public bool change;
    public int index;
    private KeyCode key;

    [Header("Extras")]
    public Settings settings;
    public TimeChanges timeChanges;
    public bool english;
    public bool holdCrouch;
    public TextMeshProUGUI crouchVisual;
    public bool h24Format;
    public TextMeshProUGUI formatVisual;
    
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
        data.h24Format = this.h24Format;
        data.holdCrouch = this.holdCrouch;
        data.showFPS = this.showFPS;
        data.choosingFPS = this.choosingFPS;
    }

    public void LoadData(GameData data)
    {
        this.english = data.english;
        this.keybinds[0].keybind = data.crouch;
        this.keybinds[1].keybind = data.throwKey;
        this.keybinds[2].keybind = data.sprint;
        this.keybinds[3].keybind = data.pause;
        this.h24Format = data.h24Format;
        this.holdCrouch = data.holdCrouch;
        this.showFPS = data.showFPS;
        this.choosingFPS = data.choosingFPS;
        if(timeChanges != null)
        {
            timeChanges.UpdateTime(timeChanges.cacheTime);
        }
        ApplyToSettings();
        UpdateKeybinds();
        UpdateModifications();
    }

    void ApplyToSettings()
    {
        if(settings == null)
        {
            return;
        }

        settings.english = this.english;
        settings.crouch = this.keybinds[0].keybind;
        settings.throwKey = this.keybinds[1].keybind;
        settings.sprint = this.keybinds[2].keybind;
        settings.pause = this.keybinds[3].keybind;
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
                Debug.Log("Pressed Key: " + key);
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
