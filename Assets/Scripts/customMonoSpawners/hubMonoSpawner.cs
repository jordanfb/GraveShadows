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
            col1.SetActive(true);
            col2.SetActive(true);
        }
        else {
            col1.SetActive(false);
            col2.SetActive(false);

        }
    }
}
