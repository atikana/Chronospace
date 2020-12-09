using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class MenuSwitchSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private SoundManager soundManager;

    /* Use this bool to keep track of when the menu switching sound does not play due
     * to the sound manager being null.  In this case, the next call to play the menu
     * switching sound should override the changedMenus variable in SoundManager.cs.
     */
    private static bool overrideChangedMenus = false;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundManager)
        {
            soundManager.PlayMenuSwitchingSound(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (soundManager)
        {
            soundManager.PlayMenuSwitchingSound(overrideChangedMenus);
            overrideChangedMenus = false;
        }
        else
        {
            overrideChangedMenus = true;
        }
    }
}
