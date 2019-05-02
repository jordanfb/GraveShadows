using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadowRealmManager : MonoBehaviour
{

    private static ShadowRealmManager _instance;

    public static ShadowRealmManager Instance { get { return _instance; } }


    public bool isChoosingWall { get; set; } = false;

    public GameObject shadowPlane;
    [SerializeField]
    private List<GameObject> lightsInScene;

    //this will eventually be a list of objects on the player


    
    public Transform shadowRealmTransform;

    public Collider checkIfFreeCollider;

    public LayerMask WallMask;
    private Collider wallToTeleportTo;

    public bool isInShadowRealm;
    //this should use the shadow plane variabe :/
    private float SHADOWPLANE_HEIGHT;

    private simplePlayerMovement spm;
    private ThirdPersonCamera tpc;


    //public GameObject choosingRedicle;
    GameObject lightContainer;
    private bool abortIsChoosingWall = false;
    public GameObject particleSystemGO;
    public GameObject copContainer;
    public float shadowAppearSpeed = 0.5f;
    public GameObject choosingUIPrefab;
    public GameObject notChoosingUIPrefab;
    private GameObject choosingUI;
    private GameObject notChoosingUI;

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

    }
    private void Update()
    {
        //if (!GetComponent<simplePlayerMovement>().getIsInShadowRealm()) {
        //    checkForShadows();
        //}

        if (isChoosingWall && checkForShadows().Count>1) {
            choosingUI.SetActive(true);
            notChoosingUI.SetActive(false);
        }
        else  if (isChoosingWall && checkForShadows().Count == 1)
        {
            choosingUI.SetActive(false);
            notChoosingUI.SetActive(true);
        }
        else {
            choosingUI.SetActive(false);
            notChoosingUI.SetActive(false);
        }

        Debug.DrawRay(checkIfFreeCollider.transform.position, -transform.up + -transform.up*0.1f);
        if (Input.GetKey(KeyCode.Space))
        {

            if (abortIsChoosingWall) {
                return;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Escape))
            {
                tpc.resetCurrentWallToChooseFrom();
                foreach (KeyValuePair<Collider, List<Vector3>> entry in checkForShadows())
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
            if (checkForShadows().Keys.Count == 0)
            {
                return;
            }
            isChoosingWall = true;
            foreach (KeyValuePair<Collider, List<Vector3>> entry in checkForShadows())
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
                if (checkForShadows().ContainsKey(wallToTeleportTo))
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
                foreach (KeyValuePair<Collider, List<Vector3>> entry in checkForShadows())
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


                if (checkForShadows().ContainsKey(wallToTeleportTo))
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
        if (checkIfFreeCollider.GetComponent<checkIfFreeColliderScript>().isColliding) {
        Debug.Log("ERROR, player trying to enter real world but would be colliding with something");
            return;
        }
        foreach (GameObject ev in GameObject.FindGameObjectsWithTag("Evidence"))
        {
            if(ev.GetComponent<EvidenceMono>() == null) {
                continue;
            }
            ev.GetComponent<EvidenceMono>().setMatsToReg();
        }


        for (int i = 0; i< copContainer.transform.childCount; i++) {
            copContainer.transform.GetChild(i).gameObject.GetComponent<GuardScript>().ResetMaterials();
        }
        StartCoroutine(spawnParticleSystem(shadowPlane.transform.position, checkIfFreeCollider.transform.position, shadowPlane.transform.position - checkIfFreeCollider.transform.position));
        gameObject.transform.position = checkIfFreeCollider.transform.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        isInShadowRealm = !isInShadowRealm;
        shadowPlane.transform.position = shadowRealmTransform.position;

    }

    void teleportToWall(Collider targetWall, List<Vector3> pointList) {

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
        shadowPlane.transform.position = new Vector3(midPoint.x, yPos, midPoint.z);
        //adjust rotation to be thjat of the parent of the collider. i.e. the gameobject wall

        shadowPlane.transform.rotation = targetWall.transform.rotation;
        shadowPlane.transform.position -= targetWall.transform.right*0.01f;
        //moves it a bit away); = new Vector3(

        Vector3 partDir = midPoint- gameObject.transform.position;
        StartCoroutine(spawnParticleSystem(gameObject.transform.position, midPoint, partDir));

        gameObject.transform.position = shadowRealmTransform.position;
        StartCoroutine(moveBodyToWall());
        //gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.AngleAxis(90f, Vector3.up);
        isInShadowRealm = !isInShadowRealm;
        GetComponent<simplePlayerMovement>().setCurrentWallCollider(targetWall);

        for (int i = 0; i < copContainer.transform.childCount; i++)
        {
            copContainer.transform.GetChild(i).gameObject.GetComponent<GuardScript>().setShader(Shader.Find("Outlined/Silhouetted Diffuse"));

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
