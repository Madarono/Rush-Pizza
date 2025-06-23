using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTrashcan : MonoBehaviour
{
    public SoundManager sounds;

    void Start()
    {
        if(sounds == null)
        {
            sounds = GameObject.Find("UniversalScripts").GetComponent<SoundManager>();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Trashcan"))
        {
            sounds.GenerateSound(transform.position, sounds.trashCan, true, 1f);
            Destroy(gameObject);
        }
    }
}
