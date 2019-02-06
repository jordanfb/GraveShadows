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
    const float MAX_X = 80f;

    [SerializeField]
    const float MIN_Y = 0;
    [SerializeField]
    const float MAX_Y = 80f;

    private float distance = 5f;

    private float scrollSpeedX = 2f;
    private float scrollSpeedY = 2f;





    void Start()
    {
        mainCam = Camera.main;

    }

    // Update is called once per frame
    private void Update()
    {
        currentRotationX += Input.GetAxis("Mouse X")*scrollSpeedX;
        currentRotationY += Input.GetAxis("Mouse Y")*scrollSpeedY;
        distance += Input.GetAxis("Mouse ScrollWheel");

        currentRotationY = Mathf.Clamp(currentRotationY, MIN_Y, MAX_Y);


    }
    void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);
        mainCam.transform.position = lookAt.position + rot * new Vector3(0f, 0f, -distance);
        mainCam.transform.LookAt(lookAt);
    }
}
