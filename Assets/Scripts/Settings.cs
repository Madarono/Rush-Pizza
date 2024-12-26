using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public bool english = true;
    public float money;

    public bool holdCrouch;
    public KeyCode crouch = KeyCode.C;
    public KeyCode throwKey = KeyCode.R;
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public float throwForce = 10f;
    public float lookRange = 7f;

    public bool oneTimeCut = true;
}
