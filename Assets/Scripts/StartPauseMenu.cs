using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPauseMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // enable all the children
            for (int i = 0; i < transform.childCount; i++)
            {
                // toggle it on or off because it's a pause menu!
                transform.GetChild(i).gameObject.SetActive(!transform.GetChild(i).gameObject.activeInHierarchy);
            }
        }
    }
}
