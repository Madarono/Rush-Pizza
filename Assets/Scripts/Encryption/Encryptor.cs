using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SetOfStrings
{
    public char character;
    public bool used;
}

public class Encryptor : MonoBehaviour
{
    [Header("Set Encryption")]
    public EncryptionKey key;

    [Header("Encryption")]
    public string inputEcryption;
    public string outputEncryption;
    [Header("Decryption")]
    public string inputDecryption;
    public string outputDecryption;

    [Header("Randomized Encryption")]
    public bool randomize = false;
    public List<Key> keys;
    private string inputforSet = "qwertyuiopasdfghjklzxcvbnm ,";
    private string examplekeys = "the quick brown fox jumps over the lazy dog,";
    private List<SetOfStrings> setOfStrings;

    void Start()
    {
        if(!randomize)
        {
            return;
        }

        if (setOfStrings == null)
        {
            setOfStrings = new List<SetOfStrings>();
        }

        for (int i = 0; i < inputforSet.Length; i++)
        {
            SetOfStrings newSet = new SetOfStrings();
            newSet.character = inputforSet[i];
            newSet.used = false;

            setOfStrings.Add(newSet);
        }

        PopulateKeys();
    }

    void PopulateKeys()
    {
        if (keys == null)
        {
            keys = new List<Key>();
        }
        keys.Clear();

        foreach (var set in setOfStrings)
        {
            set.used = false;
        }

        List<SetOfStrings> shuffledSetOfStrings = new List<SetOfStrings>(setOfStrings);
        System.Random rng = new System.Random();
        int n = shuffledSetOfStrings.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            SetOfStrings value = shuffledSetOfStrings[k];
            shuffledSetOfStrings[k] = shuffledSetOfStrings[n];
            shuffledSetOfStrings[n] = value;
        }

        foreach (char exampleChar in examplekeys)
        {
            if (keys.Exists(k => k.character == exampleChar))
            {
                continue;
            }

            //Find an unused character from the shuffled list
            SetOfStrings availableSet = shuffledSetOfStrings.Find(s => !s.used);

            if (availableSet == null)
            {
                Debug.LogError("Not enough unique characters in setOfStrings to map all examplekeys.");
                break;
            }

            availableSet.used = true;

            Key newKey = new Key
            {
                character = exampleChar,
                overrideCharacter = availableSet.character
            };

            keys.Add(newKey);

            // Debug.Log($"Mapped: {exampleChar} -> {availableSet.character}");
        }
    }




    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Encrypt();
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            Decrypt();
        }

    }

    public void Encrypt()
    {
        string encrypted = "";

        for(int i = 0; i < inputEcryption.Length; i++)
        {
            char e = inputEcryption[i];

            for(int o = 0; o < key.keys.Length; o++)
            {
                if(e == key.keys[o].character)
                {
                    encrypted = encrypted + key.keys[o].overrideCharacter.ToString();
                    break;
                }
            }
        }

        outputEncryption = encrypted;
    }

    public void Decrypt()
    {
        string decrypted = "";

        for(int i = 0; i < inputDecryption.Length; i++)
        {
            char e = inputDecryption[i];

            for(int o = 0; o < key.keys.Length; o++)
            {
                if(e == key.keys[o].overrideCharacter)
                {
                    decrypted = decrypted + key.keys[o].character.ToString();
                    break;
                }
            }
        }

        outputDecryption = decrypted;
    }
}