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

    //RecipeSystem.cs
    public bool[] isVisible = new bool[8];

    public GameData()
    {
        //Settings.cs
        this.money = 50f;
        this.holdCrouch = false;
        this.english = true;
        this.h24Format = false;

        this.crouch = KeyCode.C;
        this.throwKey = KeyCode.R;
        this.jump = KeyCode.None;
        this.sprint = KeyCode.LeftShift;
        this.pause = KeyCode.Tab;

        //RecipeSystem.cs
        this.isVisible = new bool[8];
    }
}
