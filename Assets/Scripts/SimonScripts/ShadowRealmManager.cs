using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;

public class ShadowRealmManager : MonoBehaviour
{

    private static ShadowRealmManager _instance;

    public static ShadowRealmManager Instance { get { return _instance; } }


    public bool isChoosingWall { get; set; } = false;

    public GameObject shadowPlane;
    [SerializeField]
    private List<GameObject> lightsInScene;

    //this will eventually be a list of objects on the player

    public simplePlayerMovement player;


    public Transform shadowRealmTransform;

    public Collider checkIfFreeCollider;

    public LayerMask WallMask;
    private Collider wallToTeleportTo;

    public bool isInShadowRealm;
    //this should use the shadow plane variabe :/
    private float SHADOWPLANE_HEIGHT;

    private simplePlayerMovement spm;
    private ThirdPersonCamera tpc;

    public float fishEyeRemoveSpeed = .1f; // ten frames


    //public GameObject choosingRedicle;
    GameObject lightContainer;
    private bool abortIsChoosingWall = false;
    public GameObject particleSystemGO;
    public GameObject copContainer;
    public float shadowAppearSpeed = 0.5f;
    public float cameraWarpTimeFraction = .5f; // half the time
    public GameObject choosingUIPrefab;
    public GameObject notChoosingUIPrefab;
    private GameObject choosingUI;
    private GameObject notChoosingUI;

    float fishEyeDelta = 0f;

    public PostProcessProfile postProcessProfile; // this is used to warp the camera using fisheye

    FishEyeStateMachine fesm;

    private void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        WallMask = LayerMask.GetMask("WallLayer");
        spm = GetComponent<simplePlayerMovement>();
        tpc = GetComponent<ThirdPersonCamera>();
        SHADOWPLANE_HEIGHT = shadowPlane.transform.GetChild(0).GetComponent<Renderer>().bounds.size.y/2f;
        lightContainer = GameObject.Find("lightContainer");
        //if(lightContainer == null) {
        //    print("ERROR: light container not found");
        //}
        for(int i = 0; i< lightContainer.transform.childCount; i++) {
            lightsInScene.Add(lightContainer.transform.GetChild(i).gameObject);
        }
        choosingUI = Instantiate(choosingUIPrefab);
        choosingUI.SetActive(false);
        notChoosingUI = Instantiate(notChoosingUIPrefab);
        notChoosingUI.SetActive(false);
        fesm = Instantiate(new GameObject()).AddComponent<FishEyeStateMachine>();
        fesm.setPPP(postProcessProfile);
        fesm.setState(State.thirdPerson);

    }
    private void Update()
    {
        //if (!GetComponent<simplePlayerMovement>().getIsInShadowRealm()) {
        //    checkForShadows();
        //}

        if (Input.GetKeyDown(KeyCode.I)) {
            fesm.setState(State.wallSelection);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            fesm.setState(State.thirdPerson);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            fesm.setState(State.onWall);
        }

        Dictionary<Collider, List<Vector3>> _shadowsThisFrame;
        if (Input.GetKeyDown(KeyCode.Space)) {
            tpc.wallSortReferenceVector = gameObject.transform.forward;
        }
        if (Input.GetKey(KeyCode.Space))
        {

            player.isAllowedToWalk = false;
            // in the shadow mode so slow your velocity
            


            _shadowsThisFrame = checkForShadows();

            if (isChoosingWall && _shadowsThisFrame.Count > 1)
            {
                choosingUI.SetActive(true);
                notChoosingUI.SetActive(false);
            }
            else if (isChoosingWall && _shadowsThisFrame.Count == 1)
            {
                choosingUI.SetActive(false);
                notChoosingUI.SetActive(true);
            }
            else
            {
                choosingUI.SetActive(false);
                notChoosingUI.SetActive(false);
            }
            //if you do something that makes you stop choosing the wall
            if (abortIsChoosingWall) {
                return;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Escape))
            {
                fesm.setState(State.thirdPerson);
                tpc.resetCurrentWallToChooseFrom();
                foreach (KeyValuePair<Collider, List<Vector3>> entry in _shadowsThisFrame)
                {
                    if (entry.Key == null)
                    {
                        //Debug.Log("ENTRY IS NULL");
                        continue;
                    }
                    if (entry.Key.gameObject.transform.parent == null)
                    {
                        //Debug.Log("ENTRY PARENT IS NULL");
                        continue;
                    }

                    if (entry.Key.gameObject.transform.Find("selectionQuad") != null)
                    {
                        entry.Key.gameObject.transform.Find("selectionQuad").gameObject.SetActive(false);
                    }

                }
                abortIsChoosingWall = true;
                isChoosingWall = false;

                return;
            }
            //if there are no shadoes to choose from
            if (checkForShadows().Keys.Count == 0)
            {
                return;
            }


            isChoosingWall = true;
            fesm.setState(State.wallSelection);
            foreach (KeyValuePair<Collider, List<Vector3>> entry in _shadowsThisFrame)
            {
                if(entry.Key == null) {
                    //Debug.Log("ENTRY IS NULL");
                    continue;
                }
                if (entry.Key.gameObject.transform.parent == null)
                {
                    //Debug.Log("ENTRY PARENT IS NULL");
                    continue;
                }

                if (entry.Key.gameObject.transform.Find("selectionQuad") != null) {
                    entry.Key.gameObject.transform.Find("selectionQuad").gameObject.SetActive(false);
                }

            }
            
            RaycastHit hitWall;

            if(Physics.Raycast(tpc.mainCam.transform.position, tpc.mainCam.transform.forward, out hitWall, Mathf.Infinity, WallMask)) {
                
                wallToTeleportTo = hitWall.collider;

            }
            if (wallToTeleportTo != null) {
                if (_shadowsThisFrame.ContainsKey(wallToTeleportTo))
                {
                    if (wallToTeleportTo.gameObject.transform.Find("selectionQuad") != null) {
                        
                        wallToTeleportTo.gameObject.transform.Find("selectionQuad").gameObject.SetActive(true);
                        Debug.DrawRay(transform.position, findMiddlePos(checkForShadows()[wallToTeleportTo].Distinct().ToList()), Color.magenta);

                    }


                }

            }


        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.isAllowedToWalk = true;

            choosingUI.SetActive(false);
            notChoosingUI.SetActive(false);
            _shadowsThisFrame = checkForShadows();
            tpc.resetCurrentWallToChooseFrom();
            if (abortIsChoosingWall) {
                abortIsChoosingWall = false;
                return;
            }


            isChoosingWall = false;

            if (isInShadowRealm)
            {
                teleportFromShadowRealm();

            }

            else {
                foreach (KeyValuePair<Collider, List<Vector3>> entry in _shadowsThisFrame)
                {
                    if (entry.Key == null)
                    {
                        //Debug.Log("ENTRY IS NULL");
                        continue;
                    }

                    if (entry.Key.gameObject.transform.Find("selectionQuad") != null)
                    {

                        entry.Key.gameObject.transform.Find("selectionQuad").gameObject.SetActive(false);
                    }
                }
                if (wallToTeleportTo == null)
                {
                    //Debug.Log("ERROR, no wall selected, teleport to no walls");
                    return;
                }


                if (_shadowsThisFrame.ContainsKey(wallToTeleportTo))
                {
                    teleportToWall(wallToTeleportTo, checkForShadows()[wallToTeleportTo].Distinct().ToList());
                }

            }


            

        }
    }

    public Dictionary<Collider, List<Vector3>> checkForShadows() {
        //List<WallData> wallsTouching = new List<WallData>();


        Dictionary<Collider, List<Vector3>> wallDic = new Dictionary<Collider, List<Vector3>>();

        int counter = 0;
        for (int i=0; i<lightsInScene.Count; i++) {
            if(lightsInScene[i].activeInHierarchy == false) {
                continue;
            }
            for (int j = 0; j<spm.playerHitPoints.Count; j++) {
                Vector3 direction = lightsInScene[i].gameObject.transform.position - spm.playerHitPoints[j].position;
                if (lightsInScene[i].GetComponent<Light>().type == LightType.Spot) {


                    //continue to the next player hit point if the current hit point falls out of the spt angle.
                    if(Vector3.Angle(lightsInScene[i].transform.forward, -direction)>= lightsInScene[i].GetComponent<Light>().spotAngle/2f)
                    {
                        continue;
                    }
                }

                RaycastHit hitWall;


                //if the raycast distance
                if (Physics.Raycast(spm.playerHitPoints[j].position, direction, out hitWall, Vector3.Distance(spm.playerHitPoints[j].position, lightsInScene[i].transform.position), WallMask))
                {
                    continue;
                }
                if (Physics.Raycast(spm.playerHitPoints[j].position, -direction, out hitWall, lightsInScene[i].GetComponent<Light>().range, WallMask))
                {

                    if (hitWall.collider.gameObject.CompareTag("Impossible"))
                    {
                        //if we deemed this light as impossible, continue
                        continue;
                    }
                    counter += 1;
                    //draw the ray to the player and the ray from the player to the wall
                    Debug.DrawRay(spm.playerHitPoints[j].position, spm.playerHitPoints[j].position - lightsInScene[i].transform.position, Color.blue);
                    Debug.DrawRay(spm.playerHitPoints[j].position, spm.playerHitPoints[j].position - hitWall.point, Color.red);
                    if (wallDic.ContainsKey(hitWall.collider))
                    {
                        wallDic[hitWall.collider].Add(hitWall.point);
                    }
                    else
                    {
                        wallDic.Add(hitWall.collider, new List<Vector3>());
                        //i am a dumbass
                        wallDic[hitWall.collider].Add(hitWall.point);
                    }

                }

            }



        }

        return wallDic;

    }



    void teleportFromShadowRealm()
    {
        //StartCoroutine(MakeCameraWarp());

        checkIfFreeColliderScript checkFreeScript = checkIfFreeCollider.GetComponent<checkIfFreeColliderScript>();
        if (checkFreeScript.CheckExpandedCollisionsIsColliding())
        {
            //Debug.Log("ERROR, player trying to enter real world but would be colliding with something");
            return;
        }
        foreach (GameObject ev in GameObject.FindGameObjectsWithTag("Evidence"))
        {
            if (ev.GetComponent<EvidenceMono>() == null)
            {
                continue;
            }
            ev.GetComponent<EvidenceMono>().setMatsToReg();
        }


        for (int i = 0; i < copContainer.transform.childCount; i++)
        {
            copContainer.transform.GetChild(i).gameObject.GetComponent<GuardScript>().ResetMaterials();
        }
        StartCoroutine(spawnParticleSystem(shadowPlane.transform.position, checkIfFreeCollider.transform.position, shadowPlane.transform.position - checkIfFreeCollider.transform.position));
        gameObject.transform.position = checkFreeScript.safeSpace;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isInShadowRealm = !isInShadowRealm;
        shadowPlane.transform.position = shadowRealmTransform.position;
    }

    void teleportToWall(Collider targetWall, List<Vector3> pointList) {
        fesm.setState(State.onWall);
        foreach(GameObject ev in GameObject.FindGameObjectsWithTag("Evidence")) {
            if(ev.GetComponent<EvidenceMono>() == null) {
                continue;
            }
            ev.GetComponent<EvidenceMono>().setMatsToOutline();
        }
        Debug.DrawLine(transform.position, findMiddlePos(pointList), Color.magenta);

        Vector3 midPoint = findMiddlePos(pointList);
        //make constant for shadow plane height
        //teleport to average of all points
        float yPos = targetWall.transform.position.y - (targetWall.bounds.size.y / 2f) + SHADOWPLANE_HEIGHT;
        //we need to check if the shadow goes off of the screen
        shadowPlane.transform.position = new Vector3(midPoint.x, yPos, midPoint.z);
        //adjust rotation to be thjat of the parent of the collider. i.e. the gameobject wall

        shadowPlane.transform.rotation = targetWall.transform.rotation;
        shadowPlane.transform.position -= targetWall.transform.right*0.01f;
        //moves it a bit away); = new Vector3(

        Vector3 partDir = midPoint- gameObject.transform.position;
        StartCoroutine(spawnParticleSystem(gameObject.transform.position, midPoint, partDir));

        gameObject.transform.position = shadowRealmTransform.position;

        //StartCoroutine(MakeCameraWarp());
        StartCoroutine(moveBodyToWall());
        //gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.AngleAxis(90f, Vector3.up);
        isInShadowRealm = !isInShadowRealm;
        GetComponent<simplePlayerMovement>().setCurrentWallCollider(targetWall);

        // change over the cops to use a glow shader so they're visible through walls
        Shader outlineShader = Shader.Find("Outlined/Pure Glow");
        for (int i = 0; i < copContainer.transform.childCount; i++)
        {
            copContainer.transform.GetChild(i).gameObject.GetComponent<GuardScript>().setShader(outlineShader);
        }
    }

    



    

    IEnumerator moveBodyToWall() {
        float alpha = 0.0f;
        while(alpha<1f) {
            alpha += shadowAppearSpeed * Time.deltaTime;
            shadowPlane.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Transparency", alpha);
            yield return null;
        }
        yield return 0;
    }

    Vector3 findMiddlePos(List<Vector3> pointList) {
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

        

        return average/(pointList.Count);
    }

    IEnumerator spawnParticleSystem(Vector3 startLoc, Vector3 endLoc, Vector3 direction) {
        GameObject partSys = Instantiate(particleSystemGO);
        partSys.transform.position = startLoc;
        partSys.transform.rotation = Quaternion.LookRotation(direction);
        while ((partSys.transform.position - endLoc).magnitude > 0.01f) {
            partSys.transform.position = Vector3.Lerp(partSys.transform.position, endLoc, 0.1f);
            yield return null;
        }
        Destroy(partSys);



        yield return 0;

    }



}

public enum State
{
    thirdPerson,
    wallSelection,
    onWall
}

public class FishEyeStateMachine : MonoBehaviour {
    public float currentFisheye = 0f;



    private State s;
    PostProcessProfile postProcessProfile;
    LensDistortion dist;

    private void Start()
    {
        s = State.thirdPerson;
        NextState();
    }

    public void setPPP(PostProcessProfile newPPP) {
        postProcessProfile = newPPP;
        dist = postProcessProfile.GetSetting<LensDistortion>();
    }
    private void setDist(float newDist) {
        if (postProcessProfile) {
            dist.intensity.Override(newDist);
        }
    }

    IEnumerator thirdPersonState()
    {
        Debug.Log("thirdpersonState enter");
        while (s == State.thirdPerson)
        {
            if (currentFisheye > 0f) {
                currentFisheye -= Time.deltaTime*20f;
                setDist(currentFisheye);
            }
            
            
            yield return 0;
        }
        Debug.Log("thirdpersonState exit");
        NextState();
    }
    IEnumerator wallSelectionState()
    {
        Debug.Log("wallSelectionState enter");
        while (s == State.wallSelection)
        {
            if (currentFisheye < 80f)
            {
                currentFisheye += Time.deltaTime * 40f;
                setDist(currentFisheye);
            }
            yield return 0;
        }
        Debug.Log("wallSelectionState exit");
        NextState();
    }
    IEnumerator onWallState()
    {
        Debug.Log("onWallState enter");
        while (s == State.onWall)
        {
            if (currentFisheye > 0f)
            {
                currentFisheye -= Time.deltaTime * 20f;
                setDist(currentFisheye);
            }
            yield return 0;
        }
        Debug.Log("onWallState exit");
        NextState();
    }

    public void setState(State newState) {
        s = newState;
    }

    private void NextState()
    {
        string methodName = s.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        Debug.Log(methodName);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

}



//IEnumerator warpFisheyeBackOnExitCoroutine()
//{

//    while (fishEyeDelta > 0)
//    {
//        Debug.Log("warping in" + fishEyeDelta);
//        UnityEngine.Rendering.PostProcessing.LensDistortion dist = tpc.postProcessProfile.GetSetting<UnityEngine.Rendering.PostProcessing.LensDistortion>();
//        float currentValue = dist.intensity.value;
//        float deltaAmount = -Time.deltaTime * warpAnicipationSpeed;
//        fishEyeDelta += deltaAmount;
//        float newValue = currentValue + deltaAmount;

//        dist.intensity.Override(newValue);
//        yield return new WaitForSeconds(Time.deltaTime);

//    }
//    yield return 0;

//}

//float warpAnicipationSpeed = 100;
//private void warpFisheyeOutWhileSelecting()
//{
//    Debug.Log("warping out" + fishEyeDelta);
//    if (fishEyeDelta > 70)
//    {
//        return;
//    }
//    UnityEngine.Rendering.PostProcessing.LensDistortion dist = tpc.postProcessProfile.GetSetting<UnityEngine.Rendering.PostProcessing.LensDistortion>();
//    float currentValue = dist.intensity.value;
//    float deltaAmount = Time.deltaTime * warpAnicipationSpeed;
//    fishEyeDelta += deltaAmount;
//    float newValue = currentValue + deltaAmount;

//    dist.intensity.Override(newValue);

//}


//IEnumerator MakeCameraWarp(float additionalFraction = 1)
//{
//    tpc.isWarping = true;
//    if (shadowAppearSpeed > 0)
//    {
//        yield return new WaitForSeconds(1 / shadowAppearSpeed * cameraWarpTimeFraction * additionalFraction);
//    }
//    tpc.isWarping = false;
//    UnityEngine.Rendering.PostProcessing.LensDistortion dist = tpc.postProcessProfile.GetSetting<UnityEngine.Rendering.PostProcessing.LensDistortion>();
//    if (dist != null)
//    {
//        while (dist.intensity.value < 0)
//        {
//            dist.intensity.Override(dist.intensity.value - tpc.fishEyeIntensity * fishEyeRemoveSpeed * Time.deltaTime);
//            yield return null; // wait a frame
//        }
//        dist.intensity.Override(0);
//    }
//}