using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //Settings.cs
    public float money;

    public bool holdCrouch;
    public bool english;
    public bool h24Format;
    
    //Keycodes
    public KeyCode crouch;
    public KeyCode throwKey;
    public KeyCode jump;
    public KeyCode sprint; 
    public KeyCode pause; 
    public KeyCode showBrief;
    public KeyCode zoom;
    public KeyCode buildMode;
    public KeyCode changeMode;

    //Controls.cs
    public bool showFPS;
    public bool vsync;
    public int choosingFPS;
    public bool[] volumeValues;
    public int quality;
    public float cameraFOV;
    public float cameraSens;
    public float master;
    public float background;
    public bool enableVoice;
    public int resolutionChoosing;

    //RecipeSystem.cs
    public bool[] isVisible = new bool[8];

    //Stats.cs
    public int day;

    //Supply.cs
    public int[] sumSupply;

    //License.cs
    public bool[] boxesPlaced;

    //Mission.cs
    public int[] saveState;
    public bool[] animationSave;
    public int pizzasMade;
    public float moneyGained;
    public float percentage;

    //DecorManager.cs
    public int[] decorID;
    public float[] decorVectors;
    public float[] decorRotation;

    //BuildSystem.cs
    public int[] buildID;

    //BuildMerchant.cs
    public bool[] decorSafe;

    //Tutorial.cs
    public bool hasCompleted;

    public GameData()
    {
        //Settings.cs
        this.money = 50f;
        this.holdCrouch = false;
        this.english = true;
        this.h24Format = false;

        //Keybinds
        this.crouch = KeyCode.C;
        this.throwKey = KeyCode.R;
        this.jump = KeyCode.None;
        this.sprint = KeyCode.LeftShift;
        this.pause = KeyCode.Tab;
        this.showBrief = KeyCode.B;
        this.zoom = KeyCode.Q;
        this.buildMode = KeyCode.None;
        this.changeMode = KeyCode.None;

        //Controls.cs
        this.showFPS = false;
        this.vsync = false;
        this.choosingFPS = 0;
        this.volumeValues = new bool[5];
        this.quality = 5;
        this.cameraFOV = 60f;
        this.cameraSens = 200f;
        this.master = 1f;
        this.background = 1f;
        this.enableVoice = true;
        this.resolutionChoosing = 6;

        //RecipeSystem.cs
        this.isVisible = new bool[8];

        //Supply.cs
        this.sumSupply = new int[8];

        //License.cs
        this.boxesPlaced = new bool[8];

        //Mission.cs
        this.saveState = new int[0];
        this.animationSave = new bool[0];
        this.pizzasMade = 0;
        this.moneyGained = 0;
        this.percentage = 0;
        
        //Stats.cs
        this.day = 0;

        //DecorManager.cs
        this.decorID = new int[0];
        this.decorVectors = new float[0];
        this.decorRotation = new float[0];

        //BuildSystem.cs
        this.buildID = new int[0];

        //BuildMerchant.cs
        this.decorSafe = new bool[0];

        //Tutorial.cs
        this.hasCompleted = false;
    }
}
