using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class MenuSwitchSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundManager)
        {
            soundManager.PlayMenuSwitchingSound();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (soundManager)
        {
            soundManager.PlayMenuSwitchingSound();
        }
    }
}
