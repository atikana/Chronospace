using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{

    private static GameSettings _instance = null;
    public static GameSettings Instance
    {
        get { return _instance; }
    }


    private float volume = 0.5f;
    private float mouseSensitivity = 7f;
    private float playerScore = 0;
    private float music = 0.5f;
    private bool autoAim = false;

    bool first;
    void Awake()
    {
 
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
      

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetVolume()
    {
        return volume;
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }

    public float GetScore()
    {
        return playerScore;
    }

    public bool GetAutoAim()
    {
        return autoAim;
    }

    public float GetMusic()
    {
        return music;
    }

    public void SetVolume(float f)
    {
        volume = f;
    }
   
    public void SetSensitivity(float f)
    {
        mouseSensitivity = f;
    }

    public void SetPlayerScore(float f)
    {
        playerScore = f;
    }
    public void SetAutoAim(bool b)
    {
        autoAim = b;
    }

    public void SetMusic(float f)
    {
        music = f;
    }

    public bool CheckStartMenu()
    {
        return first;
    }

    public void SetGameSettings()
    {
        first = true;
    }


}
