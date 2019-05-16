using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hubMonoSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject col1;
    public GameObject col2;

    void Start()
    {
        
        if (GameplayManager.instance.dayNum < 2) {
            if (col1 != null) {
                col1.SetActive(true);
            }
            if (col2 != null)
            {
                col2.SetActive(true);
            }
        }
        else {
            if (col1 != null)
            {
                col1.SetActive(false);
            }
            if (col2 != null)
            {
                col2.SetActive(false);
            }

        }
    }
}
