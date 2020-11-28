using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public int countdown = 3;
    public Text countdownDisplay;
    public Transform cameraTransform;
    public PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        // playerControl = FindObjectOfType<PlayerControl>().GetComponent(PlayerControl); 
        // StartCoroutine(CountdownTo());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CountdownTo()
    {
        Debug.Log("start countdown");
        int countdown_;
        countdown_ = countdown;
        countdownDisplay.gameObject.SetActive(true);
        playerControl.GetInput().Disable();
        cameraTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        while (countdown_ > 0)
        {
            Debug.Log("counting");
            countdownDisplay.text = countdown_.ToString();

            yield return new WaitForSecondsRealtime(1f);

            countdown_--;
        }
        Debug.Log("exit countdown");
        countdownDisplay.text = "START";

        yield return new WaitForSecondsRealtime(0.2f);

        //Time.timeScale = 1f;
        playerControl.GetInput().Enable();
        countdownDisplay.gameObject.SetActive(false);
    }
}
