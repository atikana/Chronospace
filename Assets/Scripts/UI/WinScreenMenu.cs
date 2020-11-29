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

        Transform t;

        if (ordered.Count > 0)
        {
            foreach (var pair in ordered)
            {
                t = content.GetChild(i);

                t.GetChild(0).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();

                if (int.Parse(pair.Value[2]) > score)
                {
                    t.GetChild(0).GetChild(1).GetComponent<Text>().text = gameSettings.ReturnPlayerName();
                    t.GetChild(0).GetChild(2).GetComponent<Text>().text = s;
                    t.GetChild(0).GetChild(3).GetComponent<Text>().text = score.ToString();
                    t.gameObject.SetActive(true);
                    i++;
                    rank = i;
                    t = content.GetChild(i);
                }

                t.GetChild(0).GetChild(0).GetComponent<Text>().text = i.ToString();
                t.GetChild(0).GetChild(1).GetComponent<Text>().text = pair.Value[0];
                t.GetChild(0).GetChild(2).GetComponent<Text>().text = pair.Value[1];
                t.GetChild(0).GetChild(3).GetComponent<Text>().text = pair.Value[2];
                t.gameObject.SetActive(true);
                i++;
            }
        }
        else
        {
            t = content.transform.GetChild(0);
            t.GetChild(0).GetChild(0).GetComponent<Text>().text = 1.ToString();
            t.GetChild(0).GetChild(1).GetComponent<Text>().text = gameSettings.ReturnPlayerName();
            t.GetChild(0).GetChild(2).GetComponent<Text>().text = s;
            t.GetChild(0).GetChild(3).GetComponent<Text>().text = score.ToString();
            t.gameObject.SetActive(true);
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
