using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{

    public GameObject soundManager;
    public PlayerControl playerControl;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = (AudioSource)soundManager.GetComponent("soundEffectsAudioSource");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float volume) 
    {
        audioSource.volume = volume;
        Debug.Log(volume);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        playerControl.mouseSensitivity = sensitivity;
        Debug.Log(sensitivity);
    }        

}
