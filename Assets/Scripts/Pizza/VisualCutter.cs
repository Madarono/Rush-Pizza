using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualCutter : MonoBehaviour
{
    public SoundManager sound;
    public float moveAmount = 5f;
    public GameObject cutPrefab;
    public Pizza pizza;

    void Start()
    {
        if(sound == null)
        {
            sound = GameObject.Find("UniversalScripts").GetComponent<SoundManager>();
        }
    }

    void Update()
    {
        MoveVisualCutter();

        if(Input.GetMouseButtonDown(1))
        {
            InsertCut();
        }
    }

    void MoveVisualCutter()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + moveAmount);
        }
        else if (scroll < 0f)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - moveAmount);
        }
    }

    void InsertCut()
    {
        GameObject go = Instantiate(cutPrefab, transform.position, transform.rotation);
        go.transform.SetParent(transform.parent);
        pizza.cuts.cuts.Add(go);
        pizza.UpdateCuts();
        sound.GenerateSound(transform.position, sound.cut, true, 1f);
    }
}
