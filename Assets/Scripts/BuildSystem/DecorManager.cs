using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DecorInformation
{
    public GameObject decorPrefab;
    public Sprite decorIcon;
}

public class DecorManager : MonoBehaviour, IDataPersistence
{
    public DecorInformation[] decorPrefabs;
    public List<GameObject> decor;
    public List<int> decorID;
    public List<float> decorVectors;
    public List<float> decorRotation;

    public int[] a_decorID;
    public float[] a_decorVectors;
    public float[] a_decorRotation;

    public bool canSave;

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            ConvertIntoFloat();
            data.decorID = this.a_decorID;
            data.decorVectors = this.a_decorVectors;
            data.decorRotation = this.a_decorRotation;
        }
    }

    public void LoadData(GameData data)
    {
        if(data.decorID.Length > 0)
        {
            this.a_decorID = data.decorID;
            this.a_decorVectors = data.decorVectors;
            this.a_decorRotation = data.decorRotation;
            ApplyChanges();
        }
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.U))
    //     {
    //         ConvertIntoFloat();
    //     }
    // }
    

    public void ConvertIntoFloat()
    {
        decorVectors.Clear();
        decorRotation.Clear();
        decorID.Clear();

        a_decorID = decorID.ToArray();
        a_decorVectors = decorVectors.ToArray();
        a_decorRotation = decorRotation.ToArray();
        
        for(int i = decor.Count - 1; i >= 0; i--)
        {
            if(decor[i] == null)
            {
                decor.RemoveAt(i);
            }
        }


        if(decor.Count == 0)
        {
            return;
        }

        for(int i = 0; i < decor.Count; i++)
        {
            decorVectors.Add(decor[i].transform.position.x);
            decorVectors.Add(decor[i].transform.position.y);
            decorVectors.Add(decor[i].transform.position.z);

            decorRotation.Add(decor[i].transform.eulerAngles.x);
            decorRotation.Add(decor[i].transform.eulerAngles.y);
            decorRotation.Add(decor[i].transform.eulerAngles.z);

            BuildDecor script = decor[i].GetComponent<BuildDecor>();
            decorID.Add(script.decorID);
        }

        a_decorID = decorID.ToArray();
        a_decorVectors = decorVectors.ToArray();
        a_decorRotation = decorRotation.ToArray();
    }

    public void ApplyChanges()
    {
        List<Vector3> decorPosition = new List<Vector3>();
    
        for (int i = 0; i < a_decorVectors.Length; i += 3)
        {
            Vector3 position = new Vector3(a_decorVectors[i], a_decorVectors[i + 1], a_decorVectors[i + 2]);
            decorPosition.Add(position);
        }
    
        List<Vector3> decorRot = new List<Vector3>();
    
        for (int i = 0; i < a_decorRotation.Length; i += 3)
        {
            Vector3 rotation = new Vector3(a_decorRotation[i], a_decorRotation[i + 1], a_decorRotation[i + 2]);
            decorRot.Add(rotation);
        }
    
        for (int i = 0; i < a_decorID.Length; i++)
        {
            GameObject go = Instantiate(decorPrefabs[a_decorID[i]].decorPrefab, decorPosition[i], Quaternion.Euler(decorRot[i]));
        }
    }

}