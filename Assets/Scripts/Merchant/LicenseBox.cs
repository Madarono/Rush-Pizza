using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LicenseBox : MonoBehaviour
{
    public License license;
    public int licenseId; //for his array id
    public GameObject highlight;
    public GameObject toppingBox;

    void Update()
    {
        if(highlight != null)
        {
            highlight.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == highlight)
        {
            highlight.SetActive(false);
            toppingBox.SetActive(true);
            license.UpdateValues(licenseId);
            Destroy(gameObject);
        }
    }

}