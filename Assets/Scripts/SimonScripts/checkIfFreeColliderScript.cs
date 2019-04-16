using System.Collections;
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
        if (other.gameObject.layer == LayerMask.GetMask("Ignore Raycast") || other.gameObject.layer == 2)
        {
            return; // don't collide with the conversation starter collider
        }
        isColliding = true;
    }


    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
}
