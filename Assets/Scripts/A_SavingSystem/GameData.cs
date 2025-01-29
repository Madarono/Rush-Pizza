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
    public KeyCode showStats; 

    public GameData()
    {
        //Settings.cs
        this.money = 50f;
        this.holdCrouch = true;
        this.english = true;
        this.h24Format = false;

        this.crouch = KeyCode.C;
        this.throwKey = KeyCode.R;
        this.jump = KeyCode.None;
        this.sprint = KeyCode.LeftShift;
        this.pause = KeyCode.Tab;
        this.showStats = KeyCode.Q;
    }
}
