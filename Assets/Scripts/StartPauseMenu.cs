using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPauseMenu : MonoBehaviour
{
    public GameObject[] pauseMenuItems;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // enable all the children
            foreach(GameObject g in pauseMenuItems)
            {
                // toggle it on or off because it's a pause menu!
                g.SetActive(!g.activeInHierarchy);
            }
        }
    }
}
