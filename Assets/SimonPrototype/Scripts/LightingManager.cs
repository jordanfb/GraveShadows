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
        List<WallData> wallsTouching = new List<WallData>();
        LayerMask mask = LayerMask.GetMask("WallLayer");
        for (int i=0; i<lightsInScene.Count; i++) {

            for(int j = 0; j<playerHitPoints.Count; j++) {
                Vector3 direction = lightsInScene[i].gameObject.transform.position - playerHitPoints[j].position;

                RaycastHit hitWall;

                if (Physics.Raycast(playerHitPoints[j].position, -direction, out hitWall, Mathf.Infinity, mask))
                {
                    WallData newData = new WallData(hitWall.collider.gameObject, hitWall.point);
                    

                    wallsTouching.Add(newData);
                    Debug.DrawRay(playerHitPoints[j].position, -direction, Color.red);
                }

            }

        }

        if (wallsTouching.Count == 1)
        {
            teleportToWall(wallsTouching[0]);
        }
        else { 
            //handle wall selection
        }
    }

    void teleportToWall(WallData wallData) {
        

    }

    private class WallData
    {

        public GameObject wallGO;
        public Vector3 hitPoint;

        public WallData(GameObject _wallGo, Vector3 _hitPoint) {
            wallGO = _wallGo;
            hitPoint = _hitPoint;
        }

    }
}
