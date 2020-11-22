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
        if (playerTransform.position.z > 70 && playerTransform.position.z < 95)
        {
            ShowText(0);
        }
        else if (playerTransform.position.z > 95 && playerTransform.position.z < 130)
        {
            ShowText(1);
        }
        else if (playerTransform.position.z > 130 && playerTransform.position.z < 160)
        {
            ShowText(2);
        }
        else if (playerTransform.position.z > 170 && playerTransform.position.z < 200)
        {
            ShowText(3);
        }
        else if (playerTransform.position.z > 200 && playerTransform.position.z < 250)
        {
            ShowText(4);
        }
        else if (playerTransform.position.z > 250 && playerTransform.position.z < 300)
        {
            ShowText(5);
        }
        else if (playerTransform.position.z > 400 && playerTransform.position.z < 440)
        {
            ShowText(6);
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



