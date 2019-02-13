﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkIfFreeColliderScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isColliding;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Evidence")) {
            return;
        }
        isColliding = true;
    }


    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
}
