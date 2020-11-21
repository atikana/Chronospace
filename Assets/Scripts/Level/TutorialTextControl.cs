using UnityEngine;
using UnityEngine.UI;

public class TutorialTextControl : MonoBehaviour
{
    public Transform playerTransform;
    private Text text;
    private int textCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTransform.position.z > 70 && playerTransform.position.z < 95)
        {
            text.text = "Left joystick to walk (Xbox) or WASD (keyboard) to use move";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 95 && playerTransform.position.z < 130)
        {
            text.text = "Right joystick to look (Xbox) or Move the mouse to look (PC)";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 130 && playerTransform.position.z < 160)
        {
            text.text = "Walk over the checkpoint to activate it";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 160 && playerTransform.position.z < 240)
        {
            text.text = "Press X (Xbox) or Shift (keyboard) to use dash";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 250 && playerTransform.position.z < 295)
        {
            text.text = "Press B (Xbox) or Q (keyboard) for time warp";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 300 && playerTransform.position.z < 325)
        {
            text.text = "Hold RT (Xbox) or left-click (mouse) to shoot the grapple";
            textCounter = 300;
        }
        else if (playerTransform.position.z > 400 && playerTransform.position.z < 440)
        {
            text.text = "Hold LT or right-click to pull while grapple is attached";
            textCounter = 300;
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
