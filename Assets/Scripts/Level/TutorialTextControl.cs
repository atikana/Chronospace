using UnityEngine;
using UnityEngine.UI;

public class TutorialTextControl : MonoBehaviour
{
    public Transform playerTransform;

    private GameObject text;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTransform.position.z > 0 && playerTransform.position.z < 40)
        {
            ShowText(0);
        }
        else if (playerTransform.position.z > 45 && playerTransform.position.z < 80)
        {
            ShowText(1);
        }
        else if (playerTransform.position.z > 85 && playerTransform.position.z < 120)
        {
            ShowText(2);
        }
        else if (playerTransform.position.z > 125 && playerTransform.position.z < 160)
        {
            ShowText(3);
        }
        else if (playerTransform.position.z > 165 && playerTransform.position.z < 200)
        {
            ShowText(4);
        }
        else if (playerTransform.position.z > 205 && playerTransform.position.z < 240)
        {
            ShowText(5);
        }
        else if (playerTransform.position.z > 265 && playerTransform.position.z < 300)
        {
            ShowText(6);
        }
        else if (playerTransform.position.z > 315 && playerTransform.position.z < 350)
        {
            ShowText(7);
        }
        else if (playerTransform.position.z > 435 && playerTransform.position.z < 470)
        {
            ShowText(8);
        }
    }


    private void ShowText(int index)
    {
        if (text != null && text.activeInHierarchy)
        {
            text.SetActive(false);
        }
        text = transform.GetChild(index).gameObject;
        text.SetActive(true);
    }
}



