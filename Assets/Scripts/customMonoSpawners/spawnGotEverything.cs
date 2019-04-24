using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnGotEverything : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ev1;
    public GameObject ev2;
    public GameObject gotEverythingCollider;

    void Start()
    {
        gotEverythingCollider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(ev1 == null && ev2 == null) {
            gotEverythingCollider.SetActive(true);
        }
    }
}
