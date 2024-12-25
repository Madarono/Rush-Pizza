using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SpecialStrings
{
    public string character;
    public float addedWaitTime;
}

public enum States
{
    Static,
    Talking,
    Waiting
}

public class Customer : MonoBehaviour
{
    public Dialog dialog;
    public States state;
    [HideInInspector]public MouseCursor mouseCursor;
    [HideInInspector]public PlayerMovement playerMovement;
    [HideInInspector]public Player_Cam playerCam;
    public GameObject okayButton;
    public GameObject whatButton;
    public GameObject hintButton;
    public bool askedWhat = false;
    public bool askedHint = false;

    public GameObject dialogWindow;
    private DialogBox dialogBox;
    public TextMeshProUGUI dialogContent;
    
    public Settings settings;
    public SpecialStrings[] specialCharcters;

    void Start()
    {
        dialogBox = dialogWindow.GetComponent<DialogBox>();
    }

    public void InitiateTalk(TalkType talkType)
    {
        if(state == States.Waiting)
        {
            return;
        }

        state = States.Talking;

        dialogWindow.SetActive(true);
        if(settings.english)
        {
            for(int i = 0; i < dialog.talk.Length; i++)
            {
                if(dialog.talk[i].type == talkType)
                {           
                    StartCoroutine(ShowText(dialog.talk[i].content));   
                    break;
                }
            }
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
        else
        {
            for(int i = 0; i < dialog.talk.Length; i++)
            {
                if(dialog.talk[i].type == talkType)
                {                
                    StartCoroutine(ShowText(dialog.talk[i].contentDeutsch));
                    break;
                }
            }
            mouseCursor.FreeCusorState();
            playerMovement.canMove = false;
            playerCam.canMove = false;
        }
    }

    IEnumerator ShowText(string content)
    {
        dialogBox.CheckDimentions(content);
        whatButton.SetActive(false);
        hintButton.SetActive(false);

        okayButton.SetActive(true);
        if(!askedHint)
        {
            if(askedWhat)
            {
                hintButton.SetActive(true);
            }
            else
            {
                whatButton.SetActive(true);
            }
        }

        dialogContent.text = "";
        for(int i = 0; i < content.Length; i++)
        {
            float delay = dialog.speedOfTalk;
            char character = content[i];
            dialogContent.text = dialogContent.text + character.ToString();
            
            for(int o = 0; o < specialCharcters.Length; o++)
            {
                string characterString = character.ToString();
                if(characterString == specialCharcters[o].character)
                {
                    delay += specialCharcters[o].addedWaitTime;
                    break;
                }
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
