using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuildSystem : MonoBehaviour, IDataPersistence
{
    public DecorManager decorManager;
    public Settings settings;
    public Pausing pausing;
    public Transform camera;

    [Header("Decor")]
    public LayerMask decorLayer;
    [HideInInspector]public GameObject heldDecor;
    [HideInInspector]public BuildDecor decorScript;

    [Header("Changing Modes")]
    public bool selectingMode = false;
    public GameObject[] modeVisual; //0 for no select, 1 for select

    [Header("Wall")]
    public GameObject[] availableWalls;
    public bool checkWall;
    public float checkRange = 4f;
    public LayerMask checkLayer;

    [Header("Visual")]
    public Transform[] systemPositions;
    public GameObject inventory;
    public float inventorySpeed = 5f;

    public List<BuildVisual> inventoryVisual;
    public int inventoryChoosing;
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform centreOfScroll;
    public float multiplyer = 4f;
    public float systemMoveSpeed = 4f;
    private Vector3 systemPlacement;

    [Header("Selecting Decor")]
    public bool isSelecting;

    [Header("Saving")]
    public int[] buildID;

    [Header("Creating Visual")]
    public GameObject visualPrefab;
    public Transform systemParent;

    [Header("Notification")]
    public GameObject noti;
    public Image notiIcon;
    public float notiDuration;

    private GameObject cacheWall; 
    private int offsetID;

    public bool canSave;

    public void SaveData(GameData data)
    {
        if(canSave)
        {
            SaveVisuals();
            data.buildID = this.buildID;
        }
    }
    public void LoadData(GameData data)
    {
        this.buildID = data.buildID;
        LoadVisuals();
    }

    void SaveVisuals()
    {
        List<int> id = new List<int>();

        for(int i = 0; i < inventoryVisual.Count; i++)
        {
            id.Add(inventoryVisual[i].decorID);
        }

        buildID = id.ToArray();
    }
    void LoadVisuals()
    {
        if(inventoryVisual.Count > 0)
        {
            for(int i = inventoryVisual.Count - 1; i >= 0; i--)
            {
                RemoveFromInventory(i);
            }
        }

        for(int i = 0; i < buildID.Length; i++)
        {
            AddToInventory(buildID[i]);
        }
    }

    public void AddToInventory(int id)
    {
        GameObject go = Instantiate(visualPrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(systemParent);
        go.transform.rotation = systemParent.rotation;
        BuildVisual script = go.GetComponent<BuildVisual>();
        script.decorID = id;
        script.decorPrefab = decorManager.decorPrefabs[id].decorPrefab;
        if(decorManager.decorPrefabs[id].decorIcon != null)
        {
            script.icon.sprite = decorManager.decorPrefabs[id].decorIcon;
        }
        inventoryVisual.Add(script);
    }

    public void VisualizeNotification(int id)
    {
        if(decorManager.decorPrefabs[id].decorIcon != null)
        {
            notiIcon.sprite = decorManager.decorPrefabs[id].decorIcon;
        }
        StartCoroutine(ShowNotification());
    }

    IEnumerator ShowNotification()
    {
        //Icon.sprite = //
        noti.SetActive(true);
        yield return new WaitForSeconds(notiDuration);
        noti.SetActive(false);
    }
    
    public void RemoveFromInventory(int id)
    {
        Destroy(inventoryVisual[id].gameObject);
        inventoryVisual.RemoveAt(id);
    }

    void Start()
    {
        inventory.transform.position = systemPositions[0].position;
        CheckWalls();
        decorScript = null;
        heldDecor = null;
    }


    void Update()
    {
        if(pausing.isPausing)
        {
            return;
        }

        if(Input.GetKeyDown(settings.buildMode))
        {
            checkWall = !checkWall;
            CheckWalls();
            if(!checkWall)
            {
                selectingMode = false;
                ConfirmDecor();
            }
            RefreshShowVisual();
        }

        if(Input.GetKeyDown(settings.changeMode))
        {
            selectingMode = !selectingMode;
            RefreshShowVisual();
            if(!selectingMode)
            {
                RemoveSelection();
            }
        }

        if(checkWall && !selectingMode)
        {
            HighlightDoor();
        }

        if(Input.GetMouseButtonDown(0) && checkWall)
        {
            if(heldDecor == null && !selectingMode)
            {
                CheckForDecor(false);
            }
            else
            {
                ConfirmDecor();
            }
        }
        else if(Input.GetMouseButtonDown(1) && checkWall && !selectingMode)
        {
            if(heldDecor == null)
            {
                CheckForDecor(true);
            }
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(checkWall && scroll != 0 && selectingMode)
        {
            int normalizedScroll = (int)Mathf.Sign(scroll);
            SelectInventoryVisual(normalizedScroll);
        }
        
        BuildMode();
    }

    void FixedUpdate()
    {
        if(checkWall)
        {
            content.localPosition = Vector3.Lerp(content.localPosition, systemPlacement, systemMoveSpeed * Time.deltaTime);
            inventory.transform.position = Vector3.Lerp(inventory.transform.position, systemPositions[1].position, inventorySpeed * Time.deltaTime);
        }
        else
        {
            inventory.transform.position = Vector3.Lerp(inventory.transform.position, systemPositions[0].position, inventorySpeed * Time.deltaTime);
        }
    }


    //Inventory logic
    void SelectInventoryVisual(int change)
    {
        if(inventoryVisual.Count == 0)
        {
            return;
        }

        inventoryChoosing += change;

        if(inventoryChoosing < 0)
        {
            inventoryChoosing = 0;
            // inventoryChoosing = inventoryVisual.Count - 1;
        }
        else if(inventoryChoosing >= inventoryVisual.Count)
        {
            inventoryChoosing = inventoryVisual.Count - 1;
            // inventoryChoosing = 0;
        }

        CenterOnChild(inventoryVisual[inventoryChoosing].rect);
        
        for(int i = 0; i < inventoryVisual.Count; i++)
        {
            inventoryVisual[i].isSelected = false;
            inventoryVisual[i].Refresh();
        }
        inventoryVisual[inventoryChoosing].isSelected = true;
        inventoryVisual[inventoryChoosing].Refresh();

        SelectDecor();
    }

    void SelectDecor()
    {
        if(heldDecor != null)
        {
            Destroy(heldDecor);
            decorScript = null;
        }
        heldDecor = Instantiate(inventoryVisual[inventoryChoosing].decorPrefab, Vector3.zero, Quaternion.identity);
        decorScript = heldDecor.GetComponent<BuildDecor>();
        decorScript.canBeStored = false;
        decorScript.CheckManager();
        isSelecting = true;
    }

    public void CenterOnChild(RectTransform child)
    {
        float viewportWidth = scrollRect.viewport.rect.width;
        float viewportCenterX = viewportWidth / 2f;

        float childLocalX = child.localPosition.x + (child.rect.width / 2);

        float targetContentX = -childLocalX + (centreOfScroll.rect.width / multiplyer);

        systemPlacement = new Vector3(targetContentX, content.localPosition.y, content.localPosition.z);
    }

    public void RemoveSelection()
    {
        for(int i = 0; i < inventoryVisual.Count; i++)
        {
            inventoryVisual[i].isSelected = false;
            inventoryVisual[i].Refresh();
        }

        if(heldDecor != null)
        {
            Destroy(heldDecor);
            heldDecor = null;
        }
        decorScript = null;
        isSelecting = false;
    }

    void RefreshShowVisual()
    {
        if(!checkWall)
        {
            modeVisual[1].SetActive(false);
            modeVisual[0].SetActive(false);
            if(decorScript != null)
            {
                decorScript.selected.SetActive(false);
            }
            return;
        }

        if(selectingMode)
        {
            if(inventoryVisual.Count > 0)
            {
                SelectInventoryVisual(0);
            }
            
            if(decorScript != null)
            {
                decorScript.selected.SetActive(false);
            }
            
            modeVisual[1].SetActive(true);
            modeVisual[0].SetActive(false);
        }
        else
        {
            modeVisual[1].SetActive(false);
            modeVisual[0].SetActive(true);
        }
    }


    //Wall logic
    void CheckForDecor(bool remove)
    {
        RaycastHit hit;

        if(Physics.Raycast(camera.position, camera.forward, out hit, checkRange, decorLayer))
        {
            decorScript = hit.collider.gameObject.GetComponent<BuildDecor>();
            if(remove)
            {
                AddToInventory(decorScript.decorID);
                decorScript.canBeStored = false;
                decorScript.CheckManager();
                Destroy(decorScript.gameObject);
            }
            else
            {
                decorScript.Activate();
            }
        }
    }

    void HighlightDoor()
    {
        RaycastHit hit;

        if(Physics.Raycast(camera.position, camera.forward, out hit, checkRange, decorLayer))
        {
            decorScript = hit.collider.gameObject.GetComponent<BuildDecor>();
            decorScript.selected.SetActive(true);
        }
        else if(decorScript != null)
        {
            decorScript.selected.SetActive(false);
        }
    }
    
    void ConfirmDecor()
    {
        if(decorScript == null)
        {
            return;
        }

        if(isSelecting)
        {
            isSelecting = false;
            decorScript.canBeStored = true;
            decorScript.CheckManager();
            decorScript.Unactivate();
            RemoveFromInventory(inventoryChoosing);
        }
        else if(!selectingMode)
        {
            if(decorScript != null)
            {
                decorScript.selected.SetActive(false);
            }
            decorScript.Unactivate();
        }
    }

    void BuildMode()
    {
        if(!checkWall || heldDecor == null)
        {
            return;
        }

        RaycastHit hit;

        if(Physics.Raycast(camera.position, camera.forward, out hit, checkRange, checkLayer))
        {
            BuildWall wall = hit.collider.gameObject.GetComponent<BuildWall>();
            
            for(int i = 0; i < decorScript.offset.Length; i++)
            {
                if(wall.direction == decorScript.offset[i].direction)
                {
                    heldDecor.transform.position = new Vector3(hit.point.x + decorScript.offset[i].positionOffset.x, hit.point.y + decorScript.offset[i].positionOffset.y, hit.point.z + decorScript.offset[i].positionOffset.z);
                    heldDecor.transform.up = hit.normal;
                    heldDecor.transform.rotation = Quaternion.Euler(heldDecor.transform.rotation.x + decorScript.offset[i].rotationOffset.x,heldDecor.transform.rotation.y + decorScript.offset[i].rotationOffset.y, heldDecor.transform.rotation.z + decorScript.offset[i].rotationOffset.z);

                    if(cacheWall == null)
                    {
                        offsetID = i;
                        cacheWall = hit.collider.gameObject;
                        return;
                    }

                    decorScript.CheckForBoundry(wall.upCheck, wall.downCheck, wall.leftCheck, wall.rightCheck, wall.direction);
                    return;
                }
            }
        }
    }

    void CheckWalls()
    {
        if(checkWall)
        {
            foreach(GameObject wall in availableWalls)
            {
                wall.SetActive(true);
            }
        }
        else
        {
            foreach(GameObject wall in availableWalls)
            {
                wall.SetActive(false);
            }
        }

    }
}
