using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera mainCam;

    private Vector3 lookAtVec;

    private Vector3 direction;
    private float currentRotationX;
    private float currentRotationY;


    private float min_x = 0;
    private float max_x = 80f;

    private float min_y = 0;
    private float max_y = 80f;

    [SerializeField]
    public float distance;
    private float STARTING_DISTANCE;

    private float scrollSpeedX = 2f;
    private float scrollSpeedY = 2f;

    ShadowRealmManager SRmanager;




    void Start()
    {
        SRmanager = GetComponent<ShadowRealmManager>();
        mainCam = Camera.main;
        lookAtVec = gameObject.transform.position;
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
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(debugPoint, 1f);
    //    //Gizmos.DrawSphere(camPos - 3 * mainCam.transform.forward, 0.5f);
    //}

    Vector3 camPos;
    Vector3 shadowModVec = Vector3.zero;
    void LateUpdate()
    {


        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");

        if (SRmanager.isInShadowRealm)
        {
            Vector3 camPlaneDis = (mainCam.transform.position - SRmanager.shadowPlane.transform.position);
            Vector3 newPos = Vector3.Scale(SRmanager.shadowPlane.transform.up, camPlaneDis);
            newPos = Vector3.Scale(SRmanager.shadowPlane.transform.up, newPos);
            newPos = (distance * SRmanager.shadowPlane.transform.up) - newPos;
            newPos = SRmanager.shadowPlane.transform.position + newPos;




            debugPoint = newPos;
            lookAtVec = SRmanager.shadowPlane.transform.position+ SRmanager.shadowPlane.transform.up*2f;
            shadowModVec = newPos;


        }
        else
        {
            lookAtVec = gameObject.transform.position;
            shadowModVec = lookAtVec;
        }


        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);

        camPos = lookAtVec + (rot * new Vector3(0f, 0f, -distance));



        if (Physics.Linecast(lookAtVec, camPos - mainCam.transform.forward, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            float wallHitDistance = -(lookAtVec - wallHit.point).magnitude + 1f;
            if (-wallHitDistance < -STARTING_DISTANCE)
            {
                wallHitDistance = -STARTING_DISTANCE;

            }

            camPos = lookAtVec + rot * new Vector3(0f, 0f, wallHitDistance);






        }

        //camPos -= mainCam.transform.forward*0.3f;
        
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, camPos, 0.5f);
        //make this lerp
        //Vector3 lookAtDir = player.transform.position - mainCam.transform.position;
        //Quaternion toRotation = Quaternion.FromToRotation(mainCam.transform.forward, lookAtDir);
        //mainCam.transform.rotation = mainCam.transform.position * Quaternion.Lerp(mainCam.transform.rotation, toRotation, 1.0f * Time.time);
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
