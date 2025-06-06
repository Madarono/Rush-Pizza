using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fps : MonoBehaviour
{
    public bool showFPS;
    public bool vsync;
    public GameObject fpsLock;
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;
    private int cacheAmount;

    void Update()
    {
        if(!showFPS)
        {
            fpsText.gameObject.SetActive(false);
            return;
        }

        fpsText.gameObject.SetActive(true);
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }

    public void ChangeFpsLimit(int amount)
    {
        Application.targetFrameRate = amount;
        cacheAmount = amount;
    }

    public void ToggleVsync()
    {
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        RefreshFPS();
    }

    void RefreshFPS()
    {
        if(vsync)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = cacheAmount;
        }
        
        fpsLock.SetActive(vsync);
    }
}
