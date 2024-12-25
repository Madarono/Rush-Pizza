using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Key
{
    public char character;
    public char overrideCharacter; //Overrides the character
}

[CreateAssetMenu(fileName = "Key", menuName = "Custom/EncryptionKey")]
public class EncryptionKey : ScriptableObject
{
    public Key[] keys;
}