using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for(int i=0; i<lightsInScene.Count; i++) {

            for(int j = 0; j<playerHitPoints.Count; j++) {
                Vector3 direction = lightsInScene[i].gameObject.transform.position - playerHitPoints[j].position;

                RaycastHit hitWall;
                LayerMask mask = LayerMask.GetMask("WallLayer");
                Debug.DrawRay(playerHitPoints[j].position, -direction, Color.red);
                if (Physics.Raycast(playerHitPoints[j].position, -direction, out hitWall, Mathf.Infinity))
                {


                }

            }

        }
    }
}
