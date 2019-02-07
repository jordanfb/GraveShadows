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
        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");




        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);
        Vector3 camPos = lookAt.position + rot * new Vector3(0f, 0f, -distance);
       

        if (Physics.Linecast(lookAt.position, mainCam.transform.position-2*transform.forward, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            camPos = lookAt.position + rot * new Vector3(0f, 0f, -(lookAt.position - wallHit.point).magnitude);
           
        }

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos, 10f);
        mainCam.transform.LookAt(lookAt);

    }


   
}
