using UnityEngine;
using UnityEngine.UI;

public class TutorialTextControl : MonoBehaviour
{
    public Transform playerTransform;

    private GameObject text;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (playerTransform.position.z > -420 && playerTransform.position.z < -392)
        {
            ShowText(0);
        }
        else if (playerTransform.position.z > -387 && playerTransform.position.z < -352)
        {
            ShowText(1);
        }
        else if (playerTransform.position.z > -347 && playerTransform.position.z < -309)
        {
            ShowText(2);
        }
        else if (playerTransform.position.z > -304 && playerTransform.position.z < -242)
        {
            ShowText(3);
        }
        else if (playerTransform.position.z > -88 && playerTransform.position.z < -41)
        {
            ShowText(4);
        }
        else if (playerTransform.position.z > 53 && playerTransform.position.z < 92)
        {
            ShowText(5);
        }
        else if (playerTransform.position.z > 139 && playerTransform.position.z < 188)
        {
            ShowText(6);
        }
        else if (playerTransform.position.z > 301 && playerTransform.position.z < 336)
        {
            ShowText(7);
        }
        else if (playerTransform.position.z > 371 && playerTransform.position.z < 409)
        {
            ShowText(8);
        }
        else if (playerTransform.position.z > 512 && playerTransform.position.z < 621)
        {
            ShowText(9);
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



