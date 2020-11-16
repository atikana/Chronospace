using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    private int countdown = 3;
    public Text countdownDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(CountdownTo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CountdownTo()
    {
        while (countdown > 0) 
        {
            countdownDisplay.text = countdown.ToString();

            yield return new WaitForSeconds(1f);

            countdown--;
        }

        countdownDisplay.text = "START";

        yield return new WaitForSeconds(1f);

        countdownDisplay.gameObject.SetActive(false);
    }
}
