using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneEvidenceManager : MonoBehaviour
{
    public bool keyFound = false;
    public bool receiptFound = false;
    public Evidence key;
    public Evidence receipt;
    public GameObject keyObject;
    public GameObject receiptObject;

    private SerializedEvidence keySE;
    private SerializedEvidence receiptSE;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        keySE = EvidenceManager.instance.apartmentEV[1];
        receiptSE = EvidenceManager.instance.apartmentEV[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (keySE.evidenceState != SerializedEvidence.EvidenceState.NotFound)
            keyFound = true;
        if (receiptSE.evidenceState != SerializedEvidence.EvidenceState.NotFound)
            receiptFound = true;

        if(keyFound && keyObject != null)
        {
            Destroy(keyObject);
            keyObject = null;
        }

        if(receiptFound && receiptObject != null)
        {
            Destroy(receiptObject);
            receiptObject = null;
        }
    }
}
