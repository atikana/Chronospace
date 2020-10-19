using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursor : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        Cursor.visible = true;

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}