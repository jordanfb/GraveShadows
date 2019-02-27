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

    public Material wallSelectedMaterial;

    public Material wallNotSelectedMaterial;
    public LayerMask WallMask;
    private Collider wallToTeleportTo;

    public bool isInShadowRealm;
    //this should use the shadow plane variabe :/
    private float SHADOWPLANE_HEIGHT;

    private simplePlayerMovement spm;
    private ThirdPersonCamera tpc;

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

    }
    private void Update()
    {
        //if (!GetComponent<simplePlayerMovement>().getIsInShadowRealm()) {
        //    checkForShadows();
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            isChoosingWall = true;
            wallToTeleportTo = null;
            foreach (KeyValuePair<Collider, List<Vector3>> entry in checkForShadows())
            {
                if(entry.Key == null) {
                    Debug.Log("ENTRY IS NULL");
                    continue;
                }
                if (entry.Key.gameObject.transform.parent == null)
                {
                    Debug.Log("ENTRY PARENT IS NULL");
                    continue;
                }
                if(entry.Key.gameObject.transform.Find("selectionQuad") != null) {
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

                    }


                }

            }


        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isChoosingWall = false;
            if (isInShadowRealm)
            {
                print("get me out!");
                teleportFromShadowRealm();

            }
            else {
                foreach (KeyValuePair<Collider, List<Vector3>> entry in checkForShadows())
                {
                    if (entry.Key == null)
                    {
                        Debug.Log("ENTRY IS NULL");
                        continue;
                    }
                    //if (entry.Key.gameObject.transform.parent == null)
                    //{
                    //    Debug.Log("ENTRY PARENT IS NULL");
                    //    continue;
                    //}
                    if (entry.Key.gameObject.transform.Find("selectionQuad") != null)
                    {
                        entry.Key.gameObject.transform.Find("selectionQuad").gameObject.SetActive(false);
                    }
                }
                if (wallToTeleportTo == null)
                {
                    Debug.Log("ERROR, no wall selected, teleport to no walls");
                    return;
                }


                if (checkForShadows().ContainsKey(wallToTeleportTo))
                {
                    teleportToWall(wallToTeleportTo, checkForShadows()[wallToTeleportTo].Distinct().ToList());
                }

            }


            

        }
    }

    Dictionary<Collider, List<Vector3>> checkForShadows() {
        //List<WallData> wallsTouching = new List<WallData>();
        Dictionary<Collider, List<Vector3>> wallDic = new Dictionary<Collider, List<Vector3>>();

        int counter = 0;
        for (int i=0; i<lightsInScene.Count; i++) {

            for(int j = 0; j<spm.playerHitPoints.Count; j++) {
                Vector3 direction = lightsInScene[i].gameObject.transform.position - spm.playerHitPoints[j].position;
                if (lightsInScene[i].GetComponent<Light>().type == LightType.Spot) {
                    //print(Vector3.Angle(lightsInScene[i].transform.forward, -direction));
                    if(Vector3.Angle(lightsInScene[i].transform.forward, -direction)>= lightsInScene[i].GetComponent<Light>().spotAngle/2f)
                    {
                        continue;
                    }
                }

                RaycastHit hitWall;

                if (Physics.Raycast(spm.playerHitPoints[j].position, direction, out hitWall, Vector3.Distance(spm.playerHitPoints[j].position, lightsInScene[i].transform.position), WallMask))
                {

                    //Debug.DrawRay(playerHitPoints[j].position, direction, Color.red);
                    continue;
                }
                if (Physics.Raycast(spm.playerHitPoints[j].position, -direction, out hitWall, Mathf.Infinity, WallMask))
                {
                    counter += 1;
                    Debug.DrawRay(spm.playerHitPoints[j].position, -direction, Color.blue);
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
        }
        else {


            gameObject.transform.position = checkIfFreeCollider.transform.position;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            isInShadowRealm = !isInShadowRealm;
            shadowPlane.transform.position = shadowRealmTransform.position;
        }
    }

    void teleportToWall(Collider targetWall, List<Vector3> pointList) {


        Debug.DrawLine(transform.position, findMiddlePos(pointList), Color.green);
        Vector3 midPoint = findMiddlePos(pointList);
        //make constant for shadow plane height
        //teleport to average of all points
        shadowPlane.transform.position = new Vector3(midPoint.x, SHADOWPLANE_HEIGHT, midPoint.z);
        //adjust rotation to be thjat of the parent of the collider. i.e. the gameobject wall

        shadowPlane.transform.rotation = targetWall.transform.rotation;
        shadowPlane.transform.position -= targetWall.transform.right*0.11f;
        //moves it a bit away); = new Vector3(



        gameObject.transform.position = shadowRealmTransform.position;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.AngleAxis(90f, Vector3.up);
        isInShadowRealm = !isInShadowRealm;
        GetComponent<simplePlayerMovement>().setCurrentWallCollider(targetWall);




    }

    Vector3 findMiddlePos(List<Vector3> pointList) {
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

        

        return average/(pointList.Count);
    }

    

}
