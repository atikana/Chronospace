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


    private float volume;
    private float mouseSensitivity;
    private float playerScore;
    private bool autoAim;

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

    public bool CheckStartMenu()
    {
        return first;
    }

    public void SetGameSettings()
    {
        first = true;
    }


}
