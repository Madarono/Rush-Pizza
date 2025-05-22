using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SoundManager sound;
    public AudioClip overrideClip;
    private Button button;

    void Awake()
    {
        if(sound == null)
        {
            sound = GameObject.Find("UniversalScripts").GetComponent<SoundManager>();
        }
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning("Button component missing on this GameObject.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sound.Generate2DSound(transform.position, sound.decorationSelection, true, 0.6f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    private void OnButtonClicked()
    {
        if(overrideClip)
        {
            sound.Generate2DSound(transform.position, overrideClip, true, 0.6f);
            return;
        }

        AudioClip clip = sound.buttonClick[Random.Range(0, sound.buttonClick.Length)];
        sound.Generate2DSound(transform.position, clip, true, 0.6f);
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
