using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class WinScreenMenu : MonoBehaviour
{

    public LevelStats levelStats;
    public Transform content;

    public float recommendTime;

    public GameObject slot;

    GameSettings gameSettings;

    Text score;
    Text rank;
    Text time;
    Text death;
    Text win;

    int maxLeaderBoard = 20;
    

    void Awake()
    {
        win = transform.GetChild(1).GetComponent<Text>();
        Transform stats = transform.GetChild(2);
        score = stats.GetChild(0).GetComponent<Text>();
        rank = stats.GetChild(1).GetComponent<Text>();
        time = stats.GetChild(2).GetComponent<Text>();
        death = stats.GetChild(3).GetComponent<Text>();
        CreateSlot();
    }
    void Start()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        getStats();
    }

    void getStats()
    {
        int deaths = levelStats.GetNumDeath();
        int playerScore = CalculateScore(deaths, levelStats.GetTime());
        string timeString = levelStats.GetTimeText();

        death.text = "Deaths: " + deaths.ToString();
        time.text = "Time: " + levelStats.GetTimeText();
        score.text = "Score: " + playerScore.ToString();
        rank.text = "Rank: " +CalculateRank(playerScore, timeString).ToString();


    }
    int CalculateScore(int death, float time)
    {

        int timeScroe = Mathf.CeilToInt(381000 * Mathf.Ceil(recommendTime - time) % 10000);

        return 381000 + timeScroe - (death * 1000);
    }

    int CalculateRank(int score, string s)
    {
        Dictionary<int, List<string>> temp = gameSettings.getScores();

        var ordered = temp.OrderBy(x => x.Value[2]).ToDictionary(x => x.Key, x => x.Value);

        int i = 0;
        int rank = 0;

        
        if (ordered.Count > 0)
        {
            foreach (var pair in ordered)
            {

                if (int.Parse(pair.Value[2]) > score)
                {


                    CalculateRankHelper(i, gameSettings.ReturnPlayerName(), s, score.ToString());
                    i++;
                    rank = i;

                }

 
                CalculateRankHelper(i, pair.Value[0], pair.Value[1], pair.Value[2]);
                i++;
            }
        }
        else
        {
            CalculateRankHelper(0, gameSettings.ReturnPlayerName(), s, score.ToString());
            i = 1;
            rank = 1;
        }


        for (int j = i; j < maxLeaderBoard; j++)
        {
            content.GetChild(j).gameObject.SetActive(false);
        }

        gameSettings.AddScore(score.ToString(), s);
        return rank;
    }


    private void CalculateRankHelper(int i, string playerName, string time, string score)
    {

        Transform t = content.transform.GetChild(i);
        t.GetChild(0).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
        t.GetChild(0).GetChild(1).GetComponent<Text>().text = playerName;
        t.GetChild(0).GetChild(2).GetComponent<Text>().text = time;
        t.GetChild(0).GetChild(3).GetComponent<Text>().text = score.ToString();
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
}
