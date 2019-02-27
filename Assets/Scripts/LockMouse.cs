using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockMouse : MonoBehaviour
{

    public bool lockMouse = true;
    public bool unlockOnEscape = true;
    // Start is called before the first frame update
    void Start()
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && unlockOnEscape)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
