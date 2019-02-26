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

    private bool nextCamPosIsLegal = true;
    Material playerMat;
    public GameObject materialedObjectsParent;




    void setAllMaterialTransparency(float newTransparency) { 
        for(int i = 0; i< materialedObjectsParent.transform.childCount; i++) {
            materialedObjectsParent.transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("_Transparency", newTransparency);


        }

    }

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


        currentRotationX += Input.GetAxis("Mouse X") * scrollSpeedX;
        currentRotationY += Input.GetAxis("Mouse Y") * scrollSpeedY;
        distance += Input.GetAxis("Mouse ScrollWheel");
        currentRotationY = Mathf.Clamp(currentRotationY, min_y, max_y);
        





    }

    Vector3 debugPoint = Vector3.zero;
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(debugPoint, 0.1f);
        //Gizmos.DrawSphere(camPos - 3 * mainCam.transform.forward, 0.5f);
    }

    Vector3 newCamPos;
    Vector3 lastLegalCamPos;
    Vector3 shadowModVec = Vector3.zero;
    Vector3 wallRaycastVec;
    void LateUpdate()
    {


        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");
        if (SRmanager.isInShadowRealm)
        {
            GameObject shadowPlaneChild = SRmanager.shadowPlane.transform.GetChild(0).gameObject;
            //Vector3 camPlaneDis = (mainCam.transform.position - shadowPlaneChild.transform.position);
            //Vector3 newPos = Vector3.Scale(shadowPlaneChild.transform.up, camPlaneDis);

            //newPos = Vector3.Scale(shadowPlaneChild.transform.up, newPos);
            //newPos = (distance * shadowPlaneChild.transform.up) - newPos;
            //print(shadowPlaneChild.name);
            ////newPos = new Vector3(newPos.y, 0f, 0f);
            //newPos = shadowPlaneChild.transform.position + newPos;



            //debugPoint = newPos;

            //lookAtVec = shadowPlaneChild.transform.position+ shadowPlaneChild.transform.up * 2f;
            //shadowModVec = newPos;

            lookAtVec = shadowPlaneChild.transform.position + shadowPlaneChild.transform.up;
            debugPoint = lookAtVec;
            wallRaycastVec = lookAtVec;


        }
        else
        {

           
            Vector3 modVec = Vector3.Scale(mainCam.transform.forward, new Vector3(1f, 0f, 1f));
          

            lookAtVec = gameObject.transform.position + modVec*2f;
            wallRaycastVec = gameObject.transform.position;
            debugPoint = lookAtVec;
        }


        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);

        newCamPos = lookAtVec + (rot * new Vector3(0f, 0f, -distance));



        if (Physics.Linecast(wallRaycastVec, newCamPos - mainCam.transform.forward*0.0f, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            float wallHitDistance = -(lookAtVec - wallHit.point).magnitude + 0.1f;
            if (-wallHitDistance < -STARTING_DISTANCE)
            {
                wallHitDistance = -STARTING_DISTANCE;

            }
           

            newCamPos = lookAtVec + rot * new Vector3(0f, 0f, wallHitDistance);






        }
        //print((newCamPos - gameObject.transform.position).magnitude);
        if((newCamPos - gameObject.transform.position).magnitude < 2f) {

            setAllMaterialTransparency(Mathf.Max((newCamPos - gameObject.transform.position).magnitude - 1.2f, 0f));
        }
        else {
            setAllMaterialTransparency(1.0f);
        }

        //camPos -= mainCam.transform.forward*0.3f;
        lastLegalCamPos = newCamPos;
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, newCamPos, 0.5f);

        mainCam.transform.LookAt(lookAtVec);

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
