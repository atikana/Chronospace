using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    AudioSource audioSource;
    GameSettings gameSettings;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameSettings = FindObjectOfType<GameSettings>();
        SetVolume(gameSettings.GetMusic());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float f)
    {
        audioSource.volume = f;
    }

 
}
