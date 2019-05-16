using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixAllColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(this.name);
        for (int i = 0; i< transform.childCount; i++) {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.GetChild(i).transform.position, transform.GetChild(i).transform.right, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log(transform.GetChild(i).name + ", " +hit.distance);
            }

        }
    }

}
