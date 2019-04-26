using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFuckBOI : MonoBehaviour
{
    public GameObject Head1;
    public GameObject Subhead1;
    public GameObject Head2;
    public GameObject Subhead2;
    public GameObject MrButton;
    public GameObject bigBad;
    // Start is called before the first frame update
    void Start()
    {
        string bBN = EvidenceManager.instance.culprit.Name;
        print(bBN);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
