using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    [SerializeField]
    private Transform lookAt;

    [SerializeField]
    private Vector3 direction;
    private float currentRotationX;
    private float currentRotationY;

    [SerializeField]
    const float MIN_X = 0;
    [SerializeField]
    const float MAX_X = 20f;

    [SerializeField]
    const float MIN_Y = 0;
    [SerializeField]
    const float MAX_Y = 20f;





    void Start()
    {
        mainCam = Camera.main;

    }

    // Update is called once per frame
    private void Update()
    {
        currentRotationX += Input.GetAxis("Mouse X");
        currentRotationY += Input.GetAxis("Mouse Y");

        print("currentRotationX: " + currentRotationX);
        print("currentRotationX: " + currentRotationY);
        //currentRotationX = Mathf.Clamp(currentRotationX, MIN_X, MAX_X);
        //currentRotationY = Mathf.Clamp(currentRotationY, MIN_Y, MAX_Y);


    }
    void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);
        mainCam.transform.position = lookAt.position + rot * new Vector3(0f, 0f, -5f);
        mainCam.transform.LookAt(lookAt);
    }
}
