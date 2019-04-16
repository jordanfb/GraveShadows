using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cabinetOpeningScript : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject assocDrawer;
    void Start()
    {
        for(int i=0; i<transform.parent.transform.childCount; i++) {
            if (transform.parent.GetChild(i).name.Contains("Drawer4")) {
                assocDrawer = transform.parent.GetChild(i).gameObject;
            }
        }

    }

   

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            assocDrawer.transform.position += assocDrawer.transform.right;
        }


    }



}
