using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{

    private static GameSettings _instance = null;
    public static GameSettings Instance
    {
        get { return _instance; }
    }

    private Dictionary<int, List<string>> tutorials = new Dictionary<int, List<string>>();
    private Dictionary<int, List<string>> level1 = new Dictionary<int, List<string>>();
    private Dictionary<int, List<string>> level2 = new Dictionary<int, List<string>>();
    private Dictionary<int, List<string>> level3 = new Dictionary<int, List<string>>();
    private static string playerName;

    private float volume = 0.5f;
    private float mouseSensitivity = 7f;
    private float playerScore = 0;
    private float music = 0.5f;
    private bool autoAim = false;

    private static readonly string[] tags = { "Cyb3rRunn3r", "Sm00thBrain", "Reflex_Action", "N00bKiller", "Le_shit"};

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

    public void AddScore(string score, string time)
    {


        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
                AddScoreHelper(level1, score, time);
                break;
            case "level2":
                AddScoreHelper(level2, score, time);
                break;
            case "Level3":
                AddScoreHelper(level3, score, time);
                break;
            default:
                AddScoreHelper(tutorials, score, time);
                break;

        }

      

       
    }

    private bool AddScoreHelper(Dictionary<int, List<string>> dict, string score, string time)
    {
        dict.Add(dict.Count, new List<string> { playerName, time, score });
        return true;
    }


    public Dictionary<int, List<string>> getScores()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
                return level1;
            case "level2":
                return level2;
            case "Level3":
                return level3;
            default:
                return tutorials;

        }


    }

    public void SelectNewPlayerName()
    {
       playerName = tags[Random.Range(0, tags.Length)] + "#" + Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10);
    }

    public string ReturnPlayerName()
    {
        if (playerName == null)
        {
            SelectNewPlayerName();
        }
        return playerName;
    }



}
