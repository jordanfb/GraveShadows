using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplePlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    private Rigidbody rb;
    public float speed;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement() {
        float moveDirX = Input.GetAxis("Vertical");
        float moveDirY = Input.GetAxis("Horizontal");
        rb.velocity = ((new Vector3(mainCam.transform.forward.x, 0f, mainCam.transform.forward.z).normalized * moveDirX) + (mainCam.transform.right.normalized * moveDirY)) * speed;

       




    }
}
