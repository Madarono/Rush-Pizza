using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PizzaGameState
{
    MainMenu,
    InGame
}

public class MainMenu : MonoBehaviour
{
    public PizzaGameState gameState;
    public Animator anim;
    public float animationDuration;
    public GameObject button;
    public Outline buttonOutline;
    public InteractCheck check;

    [Header("Decorations Saving Before Game")]
    public BuildSystem build;
    public BuildMerchant buildMerchant;
    public DecorManager decorManager;

    void Start()
    {
        UpdateValues();
    }

    public void UpdateValues() //Change canSave
    {
        switch(gameState)
        {
            case PizzaGameState.MainMenu:
                build.canSave = true;
                buildMerchant.canSave = true;
                decorManager.canSave = true;
                break;

            case PizzaGameState.InGame:
                build.canSave = true;
                buildMerchant.canSave = true;
                decorManager.canSave = true;
                break;
        }
    }

    public void BeginGame()
    {
        DataPersistenceManager.instance.SaveGame();
        gameState = PizzaGameState.InGame;
        anim.SetTrigger("StartGame");
        buttonOutline.enabled = false;
        check.mainMenu = null;
        StartCoroutine(RemoveButton());
        UpdateValues();
    }

    IEnumerator RemoveButton()
    {
        yield return new WaitForSeconds(animationDuration);
        button.SetActive(false);
    }
}
