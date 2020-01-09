using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerShadowMovementScript : MonoBehaviour
{
    // Start is called before the first frame update


    
    private Camera mainCamera;
    public Transform shadowTeleportPoint;
    private Rigidbody rb;
    public bool isInShadowRealm = false;
    public GameObject shadowPlane;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        setCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        int layerMask = 1 << 8;
        if (Physics.Raycast(mainCamera.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            //print(hit.collider.gameObject.name);
            if(hit.collider.gameObject.tag == "Wall")
            {

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isInShadowRealm = true;
                    setCameraPosition();
                    gameObject.transform.position = shadowTeleportPoint.position;
                    //rb.useGravity = false;
                    //rb.isKinematic = true;



                }
            }
        }

    }

    void setCameraPosition()
    {
        if (!isInShadowRealm) {
            mainCamera.gameObject.transform.SetParent(gameObject.transform);
            mainCamera.transform.localPosition = new Vector3(0.0f, 2.0f, -1.0f);
            mainCamera.transform.localRotation = Quaternion.Euler(30.0f, 0.0f, 0.0f);
        }
        else {
            mainCamera.gameObject.transform.SetParent(shadowPlane.transform);
            mainCamera.transform.localPosition = new Vector3(0.0f, 30.0f, -15.0f);
            mainCamera.transform.localRotation = Quaternion.Euler(65.0f, 0.0f, -180.0f);
        }

    }

    void teleportToShadowRealm() { 
        
    }
}
