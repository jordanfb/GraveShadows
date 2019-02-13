using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightingManager : MonoBehaviour
{

    private static LightingManager _instance;

    public static LightingManager Instance { get { return _instance; } }

    public List<GameObject> lightsInScene;
    //this will eventually be a list of objects on the player
    public List<Transform> playerHitPoints;
    public GameObject shadowPlane;
    public GameObject player;
    public Transform shadowRealmTransform;
    public Collider checkIfFreeCollider;


    
    const float SHADOWPLANE_HEIGHT = 1.3f;


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

    private void Update()
    {
        checkForShadows();
    }

    void checkForShadows() {
        //List<WallData> wallsTouching = new List<WallData>();
        Dictionary<Collider, List<Vector3>> wallDic = new Dictionary<Collider, List<Vector3>>();
        LayerMask mask = LayerMask.GetMask("WallLayer");
        int counter = 0;
        for (int i=0; i<lightsInScene.Count; i++) {

            for(int j = 0; j<playerHitPoints.Count; j++) {
                Vector3 direction = lightsInScene[i].gameObject.transform.position - playerHitPoints[j].position;
                if (lightsInScene[i].GetComponent<Light>().type == LightType.Spot) {
                    //print(Vector3.Angle(lightsInScene[i].transform.forward, -direction));
                    if(Vector3.Angle(lightsInScene[i].transform.forward, -direction)>= lightsInScene[i].GetComponent<Light>().spotAngle/2f)
                    {
                        continue;
                    }
                }

                RaycastHit hitWall;


                if (Physics.Raycast(playerHitPoints[j].position, -direction, out hitWall, Mathf.Infinity, mask))
                {
                    counter += 1;
                    Debug.DrawRay(playerHitPoints[j].position, -direction, Color.blue);
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GetComponent<simplePlayerMovement>().getIsInShadowRealm())
            {
                teleportFromShadowRealm();
            }
            else {
                if (wallDic.Keys.Count == 1)
                {

                    teleportToWall(wallDic.First().Key, wallDic.First().Value.Distinct().ToList());

                }
                else
                {
                    print("NEED TO PROGRAM FOR MULTIPLE WALLS AT ONCE");
                }

            }

        }
    }


    void teleportFromShadowRealm()
    {
        if (checkIfFreeCollider.GetComponent<checkIfFreeColliderScript>().isColliding) {
            Debug.Log("ERROR, player trying to enter real world but would be colliding with something");
        }
        else {
            GetComponent<ThirdPersonCamera>().setLookAt(player);
            GetComponent<ThirdPersonCamera>().printCurrentRotationX();

            player.transform.position = checkIfFreeCollider.transform.position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<simplePlayerMovement>().toggleIsInShadowRealm();
        }
    }

    void teleportToWall(Collider targetWall, List<Vector3> pointList) {


        Debug.DrawLine(transform.position, findMiddlePos(pointList), Color.green);
        Vector3 midPoint = findMiddlePos(pointList);
        //make constant for shadow plane height
        //teleport to average of all points
        shadowPlane.transform.position = new Vector3(midPoint.x, SHADOWPLANE_HEIGHT, midPoint.z);
        //adjust rotation to be thjat of the parent of the collider. i.e. the gameobject wall
        shadowPlane.transform.rotation = targetWall.transform.parent.transform.rotation;
        //moves it a bit away
        shadowPlane.transform.position += shadowPlane.transform.up * 0.01f;
        //sets the camera to focus on the player
        GetComponent<ThirdPersonCamera>().setLookAt(shadowPlane);
        GetComponent<ThirdPersonCamera>().printCurrentRotationX();

        player.transform.position = shadowRealmTransform.position;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        GetComponent<simplePlayerMovement>().toggleIsInShadowRealm();
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
