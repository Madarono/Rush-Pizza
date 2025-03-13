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

    //Controls.cs
    public bool showFPS;
    public int choosingFPS;
    public bool[] volumeValues;
    public int quality;
    public float cameraFOV;
    public float master;
    public float background;

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

        //Controls.cs
        this.showFPS = false;
        this.choosingFPS = 0;
        this.volumeValues = new bool[5];
        this.quality = 5;
        this.cameraFOV = 60f;
        this.master = 1f;
        this.background = 1f;

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
    }
}
