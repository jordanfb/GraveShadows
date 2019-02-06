using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceMono : MonoBehaviour
{
    [SerializeField]
    private Evidence _evidenceInfo;
    // Start is called before the first frame update
    void Start()
    {
        //Might be necessary
    }

    // Update is called once per frame
    void Update()
    {
        //Might be necessary
    }

    public Evidence EvidenceInfo
    {
        get { return _evidenceInfo; }
    }
}
