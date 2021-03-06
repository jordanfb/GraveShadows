﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;

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
    private float max_y = 70f;

    [SerializeField]
    public float SHADOW_CAMERA_DISTANCE;
    public float REGULAR_CAMERA_DISTANCE;
    private float currentDistance;

    public float scrollSpeedX;
    public float scrollSpeedY;
    public float chooseWallSensitivity = 1f;


    //choosing a wall variables
    int currentWallToChooseFrom = 0;
    public float cameraWhileChoosingLerpMoveSpeed;
    public float cameraWhileChoosingLerpRotateSpeed;

    ShadowRealmManager SRmanager;

    Material playerMat;
    public GameObject materialedObjectsParent;
    public Transform headTransform;

    [Tooltip("Is the camera warping to focus on a shadow on the wall?")]
    public bool isWarping = false;

    public float warpingLerpSpeed = .25f;

    public float mouseDeltaOnChange = 3f;


    Vector3 newCamPos;

    Vector3 wallRaycastVec;
    Vector3 velocity = Vector3.zero;

    private Vector3 oldCameraRotateAround = Vector3.zero;


    public Vector3 wallSortReferenceVector;

    void Start()
    {
        SRmanager = GetComponent<ShadowRealmManager>();
        if (mainCam == null) {
            //Debug.Log("Picked default main camera for third person camera controller");
            // this is so that we can set a gameobject in its place instead of the main camera for the hubworld camera system to lerp between
            mainCam = Camera.main.gameObject;
        }
        lookAtVec = gameObject.transform.position;
        currentDistance = REGULAR_CAMERA_DISTANCE;
        try {
            if (Options.instance.InvertY)
            {
                scrollSpeedY = -scrollSpeedY; // invert the Y value! This is a terrible way to do it but it works
            }
        }catch(Exception e) {
            Debug.Log("ERROR: Options is null");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        currentRotationX += Input.GetAxis("Mouse X") * scrollSpeedX;
        currentRotationY -= Input.GetAxis("Mouse Y") * scrollSpeedY;
        if (currentRotationY > 180)
        {
            currentRotationY -= 360; // idk what's happening here but this is for when we warp to walls...
        }
        //distance += Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(currentRotationY + " : " + min_y + " min < > max " + max_y);
        currentRotationY = Mathf.Clamp(currentRotationY, min_y, max_y);
    }

    void LateUpdate()
    {
        Vector3 rotateAround;
        if (SRmanager.isChoosingWall)
        {
            handleIsChoosingWall(SRmanager.checkForShadows());
            return;
        }

        RaycastHit wallHit = new RaycastHit();
        LayerMask mask = LayerMask.GetMask("WallLayer");
        //if it is in the shadow realm then move it based on this logic
        if (SRmanager.isInShadowRealm)
        {

            GameObject shadowPlaneChild = SRmanager.shadowPlane.transform.GetChild(0).gameObject;

            if (isWarping)
            {
                // warping the camera into the wall
                //postProcessProfile.GetSetting < typeof(warpingEffect) > ();
                
                rotateAround = Vector3.Lerp(oldCameraRotateAround, shadowPlaneChild.transform.position + shadowPlaneChild.transform.up, warpingLerpSpeed);
                Vector3 dpos = rotateAround - oldCameraRotateAround;
                if (dpos.sqrMagnitude > .1f)
                {
                    // then we can turn the camera to look that way
                    Quaternion dLookPos = Quaternion.LookRotation(dpos, Vector3.up);
                    Quaternion lerpLookDirection = Quaternion.Slerp(Quaternion.Euler(currentRotationY, currentRotationX, 0.0f), dLookPos, 1);
                    Vector3 lerpLookAngles = dLookPos.eulerAngles;
                    currentRotationX = lerpLookAngles.y; // for some reason these are swapped
                    currentRotationY = lerpLookAngles.x;
                }
            }
            else
            {
                rotateAround = shadowPlaneChild.transform.position + shadowPlaneChild.transform.up;
            }
            wallRaycastVec = lookAtVec;
            currentDistance = SHADOW_CAMERA_DISTANCE;

        }
        else
        {
            // focused on the player

            Vector3 dirFromCamToPlayer = mainCam.transform.position - gameObject.transform.position;
            Vector3 modVec = Vector3.Scale(dirFromCamToPlayer, new Vector3(1f, 0f, 1f));

            if (isWarping)
            {
                rotateAround = Vector3.Lerp(oldCameraRotateAround, gameObject.transform.position, warpingLerpSpeed);
            }
            else
            {
                rotateAround = gameObject.transform.position;
            }
            wallRaycastVec = gameObject.transform.position;
            currentDistance = REGULAR_CAMERA_DISTANCE;
        }

        //
        Quaternion rot = Quaternion.Euler(currentRotationY, currentRotationX, 0.0f);

        oldCameraRotateAround = rotateAround;

        newCamPos = rotateAround + (rot * new Vector3(0f, 0f, -currentDistance));
        if (Physics.Linecast(wallRaycastVec, newCamPos, out wallHit, mask))
        {
            Vector3 hitPoint = wallHit.point;
            float wallHitDistance = -(rotateAround - wallHit.point).magnitude + 0.1f;
            if (-wallHitDistance < -currentDistance)
            {
                wallHitDistance = -currentDistance;

            }
            newCamPos = rotateAround + rot * new Vector3(0f, 0f, wallHitDistance);
        }

        //float lerpTime = isWarping ? .5f : 0.001f; // if the camera is warping to a wall then slow down the lerp to let the player notice
        //mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, newCamPos, ref velocity, lerpTime);
        mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, newCamPos, ref velocity, 0.001f);

        //
        if (SRmanager.isInShadowRealm)
        {

            GameObject shadowPlaneChild = SRmanager.shadowPlane.transform.GetChild(0).gameObject;


            lookAtVec = shadowPlaneChild.transform.position + shadowPlaneChild.transform.up;

            wallRaycastVec = lookAtVec;
        }
        else
        {


            Vector3 dirFromCamToPlayer = gameObject.transform.position - mainCam.transform.position;
            Vector3 modVec = Vector3.Scale(dirFromCamToPlayer, new Vector3(1f, 0f, 1f));


            lookAtVec = gameObject.transform.position + modVec * 2f + Vector3.up*0.1f;
            wallRaycastVec = gameObject.transform.position;
        }

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
        //print(currentRotationY);
    }
    public void printCurrentRotationX()
    {
        //print(currentRotationX);
    }


    private int compareCollidersBySignedAngle(Collider a, Collider b)
    {

        //I would just use camera instead of gameobject here.
        float aSA = Vector3.SignedAngle(a.gameObject.transform.right, wallSortReferenceVector, Vector3.up);
        float bSA = Vector3.SignedAngle(b.gameObject.transform.right, wallSortReferenceVector, Vector3.up);
        if (aSA < 0f) {
            aSA += 360f;
        }
        if (bSA < 0f) {
            bSA += 360f;
        }

        if (a == null) { 
            if(b == null) {
                return 0;
            }
            return 1;
        }
        if (aSA > bSA) {
            return -1;
        }else if(bSA> aSA) {
            return 1;
        }
        else {
            return 0;
        }
    }


    List<Collider> createColliderList(List<Collider> colliders) {
        List<Collider> keyList = new List<Collider>(colliders);
        List<Collider> signedSorted = new List<Collider>(keyList);

        //sorted list based on the signed angle
        signedSorted.Sort(compareCollidersBySignedAngle);

        //get first item of absolute and make it the first
        float signedFirst = Vector3.SignedAngle(signedSorted[0].gameObject.transform.right, gameObject.transform.forward, gameObject.transform.up);
        float signedLast = Vector3.SignedAngle(signedSorted[signedSorted.Count-1].gameObject.transform.right, gameObject.transform.forward, gameObject.transform.up);
        //if the last one is actually less:)
        if (Mathf.Abs(signedLast) < Mathf.Abs(signedFirst)) {
            signedSorted.Insert(0, signedSorted[signedSorted.Count - 1]);
            signedSorted.RemoveAt(signedSorted.Count - 1);
        }


        return signedSorted;

    }
    float lastMouseDelta = 0;
    float lastMousePos = 0;
    float currentMouseDelta = 0;
    Vector3 lastOffset;
    bool justChanged = false;
    void handleIsChoosingWall(Dictionary<Collider, List<Vector3>> wallDict)
    {

        if (justChanged) {
            currentMouseDelta = 0;
            if (lastMouseDelta < 1f) {
                justChanged = false;
            }

        }
        else {
            currentMouseDelta = currentRotationX - lastMousePos;
        }

        List<Collider> keyList = new List<Collider>(wallDict.Keys);

        //keyList.Sort(compareCollidersByDirectionOfPlayer);
        if (keyList.Count == 0)
        {
            return;
        }

        //creates a sorted list of the wall colliders based on their signed angle
        keyList = createColliderList(keyList);

        if (Input.GetKeyDown(KeyCode.A)) {
            currentWallToChooseFrom -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentWallToChooseFrom += 1;


        }

        //if(lastMouseDelta > mouseDeltaOnChange && !justChanged) {

        //    currentWallToChooseFrom += 1;
        //    justChanged = true;


        //}

        //if(lastMouseDelta < -mouseDeltaOnChange && !justChanged) {
        //    currentWallToChooseFrom -= 1;
        //    justChanged = true;
        //}


        if (currentWallToChooseFrom < 0) {
            currentWallToChooseFrom = keyList.Count - 1;
        }
        if (currentWallToChooseFrom >= keyList.Count)
        {
            currentWallToChooseFrom = 0;
        }





        //Vector3 offset = keyList[currentWallToChooseFrom].gameObject.transform.forward * lastMouseDelta;
        Vector3 offset = Vector3.zero;

        Vector3 targetPos = findMiddlePos(wallDict[keyList[currentWallToChooseFrom]]) - offset;

        
     

        Vector3 dir = targetPos - mainCam.transform.position;

        //camera rotation
        Quaternion lookAngle = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion newCamLook = Quaternion.RotateTowards(mainCam.transform.rotation, lookAngle, cameraWhileChoosingLerpRotateSpeed * Time.deltaTime);
        mainCam.transform.rotation = newCamLook;


        //// update the angle that the main camera is looking at so it's not jerky
        //Vector3 angles = lookAngle.eulerAngles;
        //currentRotationX = angles.x;
        //currentRotationY = angles.y;

        //camera position

        Vector3 headToWallDir = targetPos - headTransform.position;

        Debug.DrawRay(headTransform.position, headToWallDir, Color.yellow);
        Vector3 startPos = Quaternion.LookRotation(headToWallDir, Vector3.up) * (headTransform.transform.position + headTransform.transform.forward);



        //mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, headTransform.position - headToWallDir.normalized, cameraWhileChoosingLerpMoveSpeed * Time.deltaTime);
        mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, headTransform.position - headToWallDir.normalized, ref velocity, 1f);

        lastMouseDelta = Mathf.Lerp(lastMouseDelta, currentMouseDelta, Time.deltaTime* chooseWallSensitivity);
        lastMousePos = currentRotationX;
        lastOffset = offset;

    }

    Vector3 findMiddlePos(List<Vector3> pointList)
    {
        if (pointList.Count == 0)
        {
            //Debug.Log("ERROR: Length of list is 0");
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


