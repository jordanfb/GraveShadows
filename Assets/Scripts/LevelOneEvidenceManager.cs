using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneEvidenceManager : MonoBehaviour
{
    public bool keyFound = false;
    public bool receiptFound = false;
    public Evidence key;
    public Evidence receipt;
    private GameObject keyObject;
    private GameObject receiptObject;

    public static LevelOneEvidenceManager instance;

    private SerializedEvidence keySE;
    private SerializedEvidence receiptSE;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject); // this gets destroyed once you reach the hubworld
        instance = this;
        keySE = EvidenceManager.instance.apartmentEV[1];
        receiptSE = EvidenceManager.instance.apartmentEV[0];
    }

    private void OnLevelWasLoaded(int level)
    {
        // this is so that the don't destroy on load item which is keeping track of what we've found can destroy things
        FindObjects();
        if (keyFound && keyObject != null)
        {
            Destroy(keyObject);
            keyObject = null;
            Debug.Log("Destroyed key");
        }
        if (receiptFound && receiptObject != null)
        {
            Destroy(receiptObject);
            Debug.Log("Destroyed receipt");
            receiptObject = null;
        }
    }

    public int NumEvidenceFound()
    {
        return (keyFound ? 1 : 0) + (receiptFound ? 1 : 0);
    }

    public bool AllEvidenceFound()
    {
        return keyFound && receiptFound;
    }

    private void FindObjects()
    {
        foreach(EvidenceMono m in FindObjectsOfType<EvidenceMono>()) {
            if (m.EvidenceInfo == key)
            {
                Debug.Log("Found key");
                keyObject = m.gameObject;
            }
            if (m.EvidenceInfo == receipt)
            {
                Debug.Log("Found reciept");
                receiptObject = m.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (keySE.evidenceState != SerializedEvidence.EvidenceState.NotFound)
            keyFound = true;
        if (receiptSE.evidenceState != SerializedEvidence.EvidenceState.NotFound)
            receiptFound = true;
    }
}
