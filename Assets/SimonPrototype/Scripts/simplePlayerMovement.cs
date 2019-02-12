using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplePlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    public GameObject player;
    private Rigidbody rb;
    public float speed;
    private Animator anim;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        anim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        anim.SetFloat("playerVelocity", rb.velocity.magnitude);
    }

    void movement() {
        float moveDirX = Input.GetAxis("Vertical");
        float moveDirY = Input.GetAxis("Horizontal");
        rb.velocity = ((new Vector3(mainCam.transform.forward.x, 0f, mainCam.transform.forward.z).normalized * moveDirX) + (mainCam.transform.right.normalized * moveDirY)) * speed;

       




    }
}
