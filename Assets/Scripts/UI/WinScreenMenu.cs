using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class WinScreenMenu : MonoBehaviour
{

    public LevelStats levelStats;
    public Transform content;

    public float recommendTime;

    public GameObject defaultOption;
    public GameObject slot;
  

    GameSettings gameSettings;

    Text score;
    Text rank;
    Text time;
    Text death;
    Text win;

    int maxLeaderBoard = 20;

    Color original;

    void Awake()
    {
        defaultOption = transform.GetChild(5).GetChild(0).gameObject;
        win = transform.GetChild(1).GetComponent<Text>();
        Transform stats = transform.GetChild(2);
        score = stats.GetChild(0).GetComponent<Text>();
        rank = stats.GetChild(1).GetComponent<Text>();
        time = stats.GetChild(2).GetComponent<Text>();
        death = stats.GetChild(3).GetComponent<Text>();
        original = death.color;
        CreateSlot();
    }
    void Start()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        getStats();

        Debug.Log("start winscreen");
        
    }

    public void SetupWinScreen()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultOption);
        
       
    }


   
    void getStats()
    {
        int deaths = levelStats.GetNumDeath();
        int playerScore = CalculateScore(deaths, levelStats.GetTime());
        string timeString = levelStats.GetTimeText();

        death.text = "Deaths: " + deaths.ToString();
        time.text = "Time: " + levelStats.GetTimeText();
        score.text = "Score: " + playerScore.ToString();
        rank.text = "Rank: " + CalculateRank(playerScore, timeString).ToString();
        win.text = "You won " + gameSettings.ReturnPlayerName() + "!";


    }
    int CalculateScore(int death, float time)
    {

        int timeScore = Mathf.CeilToInt(381000 * Mathf.Ceil(recommendTime - time) % 10000);

        int totalScore = 381000 + timeScore - (death * 1500);

       if (gameSettings.GetAutoAim())
        {
            totalScore = Convert.ToInt32(totalScore * 0.5);
        }

        return totalScore;
    }

    int CalculateRank(int score, string s)
    {
        Dictionary<int, List<string>> temp = gameSettings.getScores();

        var ordered = temp.OrderByDescending(x => x.Value[2]).ToDictionary(x => x.Key, x => x.Value);

        int i = 1;
        int rank = 1;

        bool add = false;

        if (ordered.Count > 0)
        {
            foreach (var pair in ordered)
            {

                if (int.Parse(pair.Value[2]) < score  && !add)
                {

                    CalculateRankHelper(i, gameSettings.ReturnPlayerName(), s, score.ToString(), true);
                    add = true;
                    rank = i;
                    i++;
                }

                CalculateRankHelper(i, pair.Value[0], pair.Value[1], pair.Value[2]);
                i++;


            }
        }

        if (!add)
        {

            CalculateRankHelper(i, gameSettings.ReturnPlayerName(), s, score.ToString(), true);
            rank = i;
            i++;
        }

        if (i < maxLeaderBoard)
        {
            for (int j = i; j < maxLeaderBoard; j++)
            {
                content.GetChild(j).gameObject.SetActive(false);
            }
        }

        gameSettings.AddScore(score.ToString(), s);
        return rank;
    }


    private void CalculateRankHelper(int i, string playerName, string time, string score, bool current = false)
    {

        Transform t = content.transform.GetChild(i-1);

        Text slotRank = t.GetChild(0).GetChild(0).GetComponent<Text>();
        Text slotName = t.GetChild(0).GetChild(1).GetComponent<Text>();
        Text slotTime = t.GetChild(0).GetChild(2).GetComponent<Text>();
        Text slotScore = t.GetChild(0).GetChild(3).GetComponent<Text>();

        slotRank.text = i.ToString();
        slotName.text = playerName;
        slotTime.text = time;
        slotScore.text = score.ToString();

        if (current)
        {

            Color c = new Color(212f / 255f, 175f / 255f, 55f / 255f);
            slotRank.color = c;
            slotName.color = c;
            slotTime.color = c;
            slotScore.color = c;
            slotRank.fontStyle = FontStyle.Bold;
            slotName.fontStyle = FontStyle.Bold;
            slotTime.fontStyle = FontStyle.Bold;
            slotScore.fontStyle = FontStyle.Bold;
        }
        else
        {
            slotRank.color = original;
            slotName.color = original;
            slotTime.color = original;
            slotScore.color = original;
            slotRank.fontStyle = FontStyle.Normal;
            slotName.fontStyle = FontStyle.Normal;
            slotTime.fontStyle = FontStyle.Normal;
            slotScore.fontStyle = FontStyle.Normal;
        }
        
        t.gameObject.SetActive(true);
    }
  


    private void CreateSlot()
    {
        for (int i = 0; i < maxLeaderBoard; i++)
        {
            slot = (GameObject)Instantiate(slot);
            slot.name = i.ToString();
            slot.transform.SetParent(content.transform);
            slot.SetActive(false);
        }

    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void OptionButtonNavi()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackButtonNavi()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ClickContinue()
    {
        string s = "";

        switch (SceneManager.GetActiveScene().name)
        {
            case "tutorial":
                s = "Level1";
                break;
            case "Level1":
                s = "Level2";
                break;
            case "Level3":
                s = "Level1";
                break;
            default:
                s = "Level3";
                break;
        }

        if (s.CompareTo("") != 0)
        {
            SceneManager.LoadScene(s);
        }
    }
}
