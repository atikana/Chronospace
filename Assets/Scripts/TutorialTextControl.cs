using UnityEngine;
using UnityEngine.UI;

public class TutorialTextControl : MonoBehaviour
{
    public Transform playerTransform;
    private Text text;
    private int textCounter = 0;
    private bool dashTextShown = false,
                 timeWarpTextShown = false,
                 grappleTextShown = false;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTransform.position.z > 120 && !timeWarpTextShown)
        {
            timeWarpTextShown = true;
            text.text = "Press B (Xbox) or T (keyboard) for time warp";
            textCounter = 200;
        }
        else if (playerTransform.position.z > 250 && !dashTextShown)
        {
            dashTextShown = true;
            text.text = "Press X (Xbox) or Shift (keyboard) to use dash";
            textCounter = 200;
        }
        else if (playerTransform.position.z > 420 && !grappleTextShown)
        {
            grappleTextShown = true;
            text.text = "Press RT (Xbox) or left-click (keyboard) to shoot the grapple";
            textCounter = 200;
        }
        else
        {
            if (textCounter > 0)
            {
                textCounter--;
            }
            else
            {
                text.text = "";
            }
        }
    }
}
