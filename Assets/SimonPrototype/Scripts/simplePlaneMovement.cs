using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplePlaneMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public int direction;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == 0)
        {
            transform.position = new Vector3(Mathf.Sin(Time.time), transform.position.y, transform.position.z);
        }
        else if (direction == 1)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time), transform.position.z);
        }
        else {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Sin(Time.time));

        }

    }
}
