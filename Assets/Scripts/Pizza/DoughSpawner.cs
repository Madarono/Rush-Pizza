using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoughSpawner : MonoBehaviour
{
    public GameObject doughPrefab;
    public Transform[] doughSpawns;
    public float checkDelay = 1f;
    public float checkRange = 0.1f;
    public Quaternion rotationOffset;
    private float o_checkDelay;

    [Header("Special modifications")]
    public bool isDough;
    public Settings settings;

    void Start()
    {
        o_checkDelay = checkDelay;
    }


    void Update()
    {
        if(checkDelay > 0)
        {
            checkDelay -= Time.deltaTime;
        }
        else
        {
            checkDelay = o_checkDelay;
            CheckDough();
        }
    }

    void CheckDough()
    {
        bool disturbance = false;
        for(int i = 0; i < doughSpawns.Length; i++)
        {
            RaycastHit hit;
            Debug.DrawRay(doughSpawns[i].position, Vector3.forward, Color.green, checkRange);
            Debug.DrawRay(doughSpawns[i].position, Vector3.forward * -1f, Color.blue, checkRange);
            Debug.DrawRay(doughSpawns[i].position, Vector3.up, Color.red, checkRange);

            if(Physics.Raycast(doughSpawns[i].position, Vector3.forward, out hit, checkRange))
            {
                disturbance = true;
                break;
            }
            else if(Physics.Raycast(doughSpawns[i].position, Vector3.forward * -1f, out hit, checkRange))
            {
                disturbance = true;
                break;
            }
            else if(Physics.Raycast(doughSpawns[i].position, Vector3.up, out hit, checkRange))
            {
                disturbance = true;
                break;
            }
        }

        if(disturbance)
        {
            return;
        }

        for(int i = 0; i < doughSpawns.Length; i++)
        {
            GameObject go = Instantiate(doughPrefab, new Vector3(doughSpawns[i].position.x, doughSpawns[i].position.y + 1f, doughSpawns[i].position.z), Quaternion.identity);
            go.transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
            if(isDough)
            {
                Interactable dough = go.GetComponent<Interactable>();
                dough.settings = this.settings;
            }
        }
    }
}
