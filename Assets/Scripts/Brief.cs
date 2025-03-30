using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Requirements
{
    public int requiredLimit;
    public GameObject talkSize;
}

public class Brief : WindowOpening
{
    [Header("Scripts")]
    public Pausing pausing;
    public Settings settings;
    public MouseCursor mouse;
    public PlayerMovement player;
    public DragAndDrop drag;
    public Player_Cam cam;

    
    [Header("Brief Script")]
    public bool canBrief = false;
    public Animator briefWindowAnim;
    public GameObject paper;
    public Animator paperAnim;
    public float hidePaperDuration = 0.5f;

    [Header("Brief Info")]
    public Dialog dialog;
    public bool askedWhat;
    public bool askedHint;
    public List<string> talk;
    public List<string> answer;

    [Header("Talk")]
    public GameObject answerPrefab; 
    public Requirements[] requirements;

    public Transform parentSpawn;
    private GameObject selectedPrefab;

    void Start()
    {
        paper.SetActive(false);
    }

    public void GatherInformationOfDialog()
    {
        talk.Clear();
        answer.Clear();

        if(!askedWhat && !askedHint)
        {
            if(settings.english)
            {
                talk.Add(dialog.talk[0].content);
                answer.Add("Okay");
            }
            else
            {
                talk.Add(dialog.talk[0].contentDeutsch);
                answer.Add("Okay");
            }
        }
        else if(askedHint && !askedHint)
        {
            if(settings.english)
            {
                talk.Add(dialog.talk[0].content);
                answer.Add("What?");
                talk.Add(dialog.talk[1].content);
                answer.Add("Okay");
            }
            else
            {
                talk.Add(dialog.talk[0].contentDeutsch);
                answer.Add("Was?");
                talk.Add(dialog.talk[1].contentDeutsch);
                answer.Add("Okay");
            }
        }
        else if(askedHint && askedHint)
        {
            if(settings.english)
            {
                talk.Add(dialog.talk[0].content);
                answer.Add("What?");
                talk.Add(dialog.talk[1].content);
                answer.Add("Hint");
                talk.Add(dialog.talk[2].content);
                answer.Add("Okay");
            }
            else
            {
                talk.Add(dialog.talk[0].contentDeutsch);
                answer.Add("Was?");
                talk.Add(dialog.talk[1].contentDeutsch);
                answer.Add("Hint");
                talk.Add(dialog.talk[2].contentDeutsch);
                answer.Add("Okay");
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(settings.showBrief) && canBrief && Time.timeScale == 1)
        {
            BothWindow();
        }
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        drag.can_drag = false;
        mouse.FreeCusorState();
        player.canMove = false;
        cam.canMove = false;
        GatherInformationOfDialog();
        GenerateBrief();
        pausing.lockMouse = false;
    }

    public override void CloseWindow()
    {
        if(!isOpen)
        {
            return;
        }
        
        base.CloseWindow();
        briefWindowAnim.SetTrigger("Close");
        if(Time.timeScale == 1)
        {
            mouse.LockCusorState();
        }
        drag.can_drag = true;
        player.canMove = true;
        cam.canMove = true;

        if(isOpen)
        {
            pausing.lockMouse = true;
        }
    }

    public void ShowPaper()
    {
        paper.SetActive(true);
    }

    public void HidePaper()
    {
        StartCoroutine(HidingPaper());
    }

    IEnumerator HidingPaper()
    {
        paperAnim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(hidePaperDuration);
        paper.SetActive(false);
    }

    public void GenerateBrief()
    {
        if(parentSpawn.childCount > 0)
        {
            foreach(Transform child in parentSpawn)
            {
                Destroy(child.gameObject);
            }
        }


        for(int i = 0; i < talk.Count; i++)
        {
            if(talk[i] != null)
            {
                int length = talk[i].Length;
                foreach(Requirements req in requirements)
                {
                    if(length <= req.requiredLimit)
                    {
                        selectedPrefab = req.talkSize;
                        break;
                    }
                }

                if(selectedPrefab != null)
                {
                    GameObject go = Instantiate(selectedPrefab, Vector3.one, Quaternion.identity);
                    go.transform.SetParent(parentSpawn);
                    go.transform.localScale = Vector3.one;
                    go.transform.position = parentSpawn.position;
                    go.transform.rotation = parentSpawn.rotation;
                    TextMeshProUGUI goText = go.GetComponent<TextMeshProUGUI>();
                    goText.text = talk[i]; 
                }

                if(answer[i] != null)
                {
                    GameObject go = Instantiate(answerPrefab, Vector3.one, Quaternion.identity);
                    go.transform.SetParent(parentSpawn);
                    go.transform.localScale = Vector3.one;
                    go.transform.position = parentSpawn.position;
                    go.transform.rotation = parentSpawn.rotation;
                    TextMeshProUGUI goText = go.GetComponent<TextMeshProUGUI>();
                    goText.text = answer[i]; 
                }
            }
        }
    } 
}
