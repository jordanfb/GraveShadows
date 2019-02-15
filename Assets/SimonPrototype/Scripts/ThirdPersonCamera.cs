using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    [SerializeField]

    private Vector3 lookAtVec;

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

    public float distance;
    private float STARTING_DISTANCE;

    private float scrollSpeedX = 2f;
    private float scrollSpeedY = 2f;
    public GameObject shadowPlane;
    public GameObject shadowRealmLookAt;
    public GameObject player;





    void Start()
    {
        mainCam = Camera.main;
        lookAtVec = player.transform.position;
        STARTING_DISTANCE = distance;

    }

    // Update is called once per frame
    private void Update()
    {
        currentRotationX += Input.GetAxis("Mouse X")*scrollSpeedX;
        currentRotationY += Input.GetAxis("Mouse Y")*scrollSpeedY;
        distance += Input.GetAxis("Mouse ScrollWheel");

        currentRotationY = Mathf.Clamp(currentRotationY, min_y, max_y);


    }

    Vector3 debugPoint = Vector3.zero;
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(debugPoint, 1f);
    }

    Vector3 camPos;
    Vector3 shadowModVec = Vector3.zero;
    void LateUpdate()
    {


        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");

        if (GetComponent<simplePlayerMovement>().getIsInShadowRealm())
        {
            Vector3 camPlaneDis = (mainCam.transform.position - shadowPlane.transform.position);
            Vector3 newPos = Vector3.Scale(shadowPlane.transform.up, camPlaneDis);
            newPos = Vector3.Scale(shadowPlane.transform.up, newPos);
            newPos = (distance * shadowPlane.transform.up) - newPos;
            newPos = shadowPlane.transform.position + newPos;




            debugPoint = newPos;
            lookAtVec = shadowPlane.transform.position+ shadowPlane.transform.up*2f;
            shadowModVec = newPos;


        }
        else
        {
            lookAtVec = player.transform.position;
            shadowModVec = lookAtVec;
        }


        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);

        camPos = lookAtVec + (rot * new Vector3(0f, 0f, -distance));



        if (Physics.Linecast(lookAtVec, camPos, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            if (-(lookAtVec - wallHit.point).magnitude - 0.3f < -STARTING_DISTANCE)
            {
                distance = STARTING_DISTANCE;
                camPos = lookAtVec + rot * new Vector3(0f, 0f, -distance);
            }
            else {
                camPos = lookAtVec + rot * new Vector3(0f, 0f, -(lookAtVec - wallHit.point).magnitude - 0.3f);

            }





        }

        //camPos -= mainCam.transform.forward*0.3f;
        
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos, 10f);
        mainCam.transform.LookAt(shadowModVec);

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
