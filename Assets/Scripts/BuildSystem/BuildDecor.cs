using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildOffset
{
    public WallDirection direction;
    public Vector3 positionOffset;
    public Quaternion rotationOffset;
}

public class BuildDecor : MonoBehaviour
{
    public DecorManager decorManager;
    public bool canBeStored = true;
    public BuildSystem build;
    public BuildOffset[] offset;
    public GameObject selected;
    public GameObject selectedBig;

    public Transform upCheck;
    public Transform downCheck;
    public Transform leftCheck;
    public Transform rightCheck;

    [Header("For DecorManager")]
    public int decorID;

    void Start()
    {
        if(decorManager == null)
        {
            decorManager = GameObject.Find("UniversalScripts").GetComponent<DecorManager>();
            CheckManager();
        }

        if(build == null)
        {
            build = GameObject.Find("UniversalScripts").GetComponent<BuildSystem>(); 
        }
    }

    public void CheckManager()
    {
        if(decorManager == null)
        {
            return;
        }

        if(canBeStored && !decorManager.decor.Contains(gameObject))
        {
            decorManager.decor.Add(gameObject);
        }
        else if(!canBeStored && decorManager.decor.Contains(gameObject))
        {
            decorManager.decor.Remove(gameObject);
        }
    }

    public void Activate()
    {
        build.heldDecor = gameObject;
        build.decorScript = this;
        selectedBig.SetActive(true);
    }

    public void Unactivate()
    {
        build.heldDecor = null;
        build.decorScript = null;
        selectedBig.SetActive(false);
    }

    public void CheckForBoundry(Transform up, Transform down, Transform left, Transform right, WallDirection direction)
    {
        if(upCheck.position.y > up.position.y)
        {
            float difference = upCheck.position.y - up.position.y;
            transform.position = new Vector3(transform.position.x, transform.position.y - difference, transform.position.z);
        }
        else if(downCheck.position.y < down.position.y)
        {
            float difference = down.position.y - downCheck.position.y;
            transform.position = new Vector3(transform.position.x, transform.position.y + difference, transform.position.z);
        }

        switch(direction)
        {
            case WallDirection.Forward:
                if(leftCheck.position.x < left.position.x)
                {
                    float difference = left.position.x - leftCheck.position.x;
                    transform.position = new Vector3(transform.position.x + difference, transform.position.y, transform.position.z);
                }
                else if(rightCheck.position.x > right.position.x)
                {
                    float difference = rightCheck.position.x - right.position.x;
                    transform.position = new Vector3(transform.position.x - difference, transform.position.y, transform.position.z);
                }
                break;

            case WallDirection.Backward:
                if(rightCheck.position.x < right.position.x)
                {
                    float difference = right.position.x - rightCheck.position.x;
                    transform.position = new Vector3(transform.position.x + difference, transform.position.y, transform.position.z);
                }
                else if(leftCheck.position.x > left.position.x)
                {
                    float difference = leftCheck.position.x - left.position.x;
                    transform.position = new Vector3(transform.position.x - difference, transform.position.y, transform.position.z);
                }
                break;

            case WallDirection.Left:
                if(leftCheck.position.z < left.position.z)
                {
                    float difference = left.position.z - leftCheck.position.z;
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + difference);
                }
                else if(rightCheck.position.z > right.position.z)
                {
                    float difference = rightCheck.position.z - right.position.z;
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - difference);
                }    
                break;

            case WallDirection.Right:
                if(rightCheck.position.z < left.position.z)
                {
                    float difference = left.position.z - rightCheck.position.z;
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + difference);
                }
                else if(leftCheck.position.z > right.position.z)
                {
                    float difference = leftCheck.position.z - right.position.z;
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - difference);
                }    
                break;
        }
    }
}
