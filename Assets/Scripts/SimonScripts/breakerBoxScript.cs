using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakerBoxScript : MonoBehaviour
{

    public List<GameObject> affiliatedLights;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject l in affiliatedLights)
        {
            l.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (Input.GetKeyDown(KeyCode.E)) {
                foreach (GameObject l in affiliatedLights)
                {
                    l.SetActive(true);
                }
            }

        }
    }
}
