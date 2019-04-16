using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainCam;

    private Vector3 lookAtVec;

    private Vector3 direction;
    private float currentRotationX;
    private float currentRotationY;


    private float min_x = 0;
    private float max_x = 80f;

    private float min_y = 0;
    private float max_y = 80f;

    [SerializeField]
    public float SHADOW_CAMERA_DISTANCE;
    public float REGULAR_CAMERA_DISTANCE;
    private float currentDistance;

    public float scrollSpeedX;
    public float scrollSpeedY;



    //choosing a wall variables
    int currentWallToChooseFrom = 0;
    public float cameraWhileChoosingLerpMoveSpeed;
    public float cameraWhileChoosingLerpRotateSpeed;

    ShadowRealmManager SRmanager;

    Material playerMat;
    public GameObject materialedObjectsParent;
    public Transform headTransform;



    //void setAllMaterialTransparency(float newTransparency) { 
    //    for(int i = 0; i< materialedObjectsParent.transform.childCount; i++) {
    //        materialedObjectsParent.transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("_Transparency", newTransparency);


    //    }

    //}

    void Start()
    {

        SRmanager = GetComponent<ShadowRealmManager>();
        if (mainCam == null) {
            Debug.Log("Picked default main camera for third person camera controller");
            // this is so that we can set a gameobject in its place instead of the main camera for the hubworld camera system to lerp between
            mainCam = Camera.main.gameObject;
        }
        lookAtVec = gameObject.transform.position;
        currentDistance = REGULAR_CAMERA_DISTANCE;

    }

    // Update is called once per frame
    private void Update()
    {



        currentRotationX += Input.GetAxis("Mouse X") * scrollSpeedX;
        currentRotationY -= Input.GetAxis("Mouse Y") * scrollSpeedY;
        //distance += Input.GetAxis("Mouse ScrollWheel");
        currentRotationY = Mathf.Clamp(currentRotationY, min_y, max_y);
        





    }

    Vector3 debugPoint = Vector3.zero;
    void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(camPos - 3 * mainCam.transform.forward, 0.5f);
    }

    Vector3 newCamPos;
    Vector3 lastLegalCamPos;
    Vector3 shadowModVec = Vector3.zero;
    Vector3 wallRaycastVec;
    void LateUpdate()
    {
        if (SRmanager.isChoosingWall)
        {
            handleIsChoosingWall(SRmanager.checkForShadows());
            return;
        }

        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");
        if (SRmanager.isInShadowRealm)
        {

            GameObject shadowPlaneChild = SRmanager.shadowPlane.transform.GetChild(0).gameObject;


            lookAtVec = shadowPlaneChild.transform.position + shadowPlaneChild.transform.up;
            debugPoint = lookAtVec;
            wallRaycastVec = lookAtVec;
            currentDistance = SHADOW_CAMERA_DISTANCE;

        }
        else
        {

           
            Vector3 modVec = Vector3.Scale(mainCam.transform.forward, new Vector3(1f, 0f, 1f));
          

            lookAtVec = gameObject.transform.position + modVec * 2f + mainCam.transform.right*0.2f;
            wallRaycastVec = gameObject.transform.position;
            debugPoint = lookAtVec;
            currentDistance = REGULAR_CAMERA_DISTANCE;
        }


        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);

        newCamPos = lookAtVec + (rot * (new Vector3(0f, 0f, -currentDistance)));



        if (Physics.Linecast(wallRaycastVec, newCamPos - mainCam.transform.forward*0.0f, out wallHit, mask)) {
            Vector3 hitPoint = wallHit.point;
            float wallHitDistance = -(lookAtVec - wallHit.point).magnitude + 0.1f;
            if (-wallHitDistance < -currentDistance)
            {
                wallHitDistance = -currentDistance;

            }
           

            newCamPos = lookAtVec + rot * new Vector3(0f, 0f, wallHitDistance);






        }
        Vector3 velocity = Vector3.zero;
        mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, newCamPos, ref velocity, 0.01f);

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


    
    void handleIsChoosingWall(Dictionary<Collider, List<Vector3>> wallDict)
    {


        List<Collider> keyList = new List<Collider>(wallDict.Keys);


        if (keyList.Count == 0) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            currentWallToChooseFrom -= 1;

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentWallToChooseFrom += 1;

        }
        if (currentWallToChooseFrom < 0) {
            currentWallToChooseFrom = keyList.Count - 1;
        }
        if (currentWallToChooseFrom >= keyList.Count)
        {
            currentWallToChooseFrom = 0;
        }


        Vector3 targetPos = findMiddlePos(wallDict[keyList[currentWallToChooseFrom]]);
        Vector3 dir = targetPos - mainCam.transform.position;

        //camera rotation
        Quaternion lookAngle = Quaternion.LookRotation(dir, Vector3.up);
        Debug.DrawRay(mainCam.transform.position, dir, Color.red);
        Quaternion newCamLook = Quaternion.RotateTowards(mainCam.transform.rotation, lookAngle, cameraWhileChoosingLerpRotateSpeed * Time.deltaTime);
        mainCam.transform.rotation = newCamLook;

        //camera position

        Vector3 headToWallDir = targetPos - headTransform.position;

        Vector3 startPos = Quaternion.LookRotation(headToWallDir, Vector3.up) * (headTransform.transform.position + headTransform.transform.forward);


        Debug.DrawLine(headTransform.position, headTransform.position + startPos*0.1f, Color.yellow);
        

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, headTransform.position + startPos*0.01f, cameraWhileChoosingLerpMoveSpeed * Time.deltaTime);


    }

    Vector3 findMiddlePos(List<Vector3> pointList)
    {
        if (pointList.Count == 0)
        {
            Debug.Log("ERROR: Length of list is 0");
            return Vector3.negativeInfinity;
        }
        Vector3 average = Vector3.zero;

        for (int i = 0; i < pointList.Count; i++)
        {
            average += pointList[i];
        }



        return average / (pointList.Count);
    }

    public void resetCurrentWallToChooseFrom() {
        currentWallToChooseFrom = 0;
    }


}


