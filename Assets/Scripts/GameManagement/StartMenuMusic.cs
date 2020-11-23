using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    AudioSource audioSource;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
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
