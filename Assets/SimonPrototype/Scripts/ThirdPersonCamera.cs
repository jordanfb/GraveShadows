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
    private float min_x = 0;
    [SerializeField]
    private float max_x = 80f;

    [SerializeField]
    private float min_y = 0;
    [SerializeField]
    private float max_y = 80f;

    private float distance = 2f;

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

        currentRotationY = Mathf.Clamp(currentRotationY, min_y, max_y);


    }
    void LateUpdate()
    {
        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");




        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);
        Vector3 camPos = lookAt.position + rot * new Vector3(0f, 0f, -distance);
       

        if (Physics.Linecast(lookAt.position, mainCam.transform.position, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            camPos = lookAt.position + rot * new Vector3(0f, 0f, -(lookAt.position - wallHit.point).magnitude);

           
        }

        camPos -= mainCam.transform.forward*0.3f;
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos, 10f);
        mainCam.transform.LookAt(lookAt);

    }

    public GameObject setLookAt(GameObject newLookAt) {
        lookAt = newLookAt.transform;
        return lookAt.gameObject;
    }

    public void setCameraClamps(float newMin_x, float newMax_x, float newMin_y, float newMax_y) {
        min_x = newMin_x;
        max_x = newMax_x;
        min_y = newMin_y;
        max_y = newMax_y;

    }
    public Vector4 getCameraClamps() {
        return new Vector4(min_x, max_x, min_y, max_y);
    }
    public void printCurrentRotationY() {
        print(currentRotationY);
    }
    public void printCurrentRotationX()
    {
        print(currentRotationX);
    }



}
