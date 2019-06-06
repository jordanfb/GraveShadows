using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOneEvidenceManager : MonoBehaviour
{
    public bool keyFound = false;
    public bool receiptFound = false;
    public Evidence key;
    public Evidence receipt;

    private bool allEvidenceFoundLatch = false;

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
        SceneManager.sceneLoaded += OnLevelLoad; // add a listener to the level being loaded
    }


    ~LevelOneEvidenceManager()
    {
        // stop listening to levels being loaded
        SceneManager.sceneLoaded -= OnLevelLoad;
        instance = null;
    }

    private void OnLevelLoad(Scene s, LoadSceneMode m)
    {
        // this is so that the don't destroy on load item which is keeping track of what we've found can destroy things
        allEvidenceFoundLatch = false; // able to run that again
        FindObjects();
        if (keyFound && keyObject != null)
        {
            Debug.Log("Destroyed receipt");
            Destroy(keyObject);
            keyObject = null;
        }
        if (receiptFound && receiptObject != null)
        {
            Debug.Log("Destroyed receipt");
            Destroy(receiptObject);
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
                keyObject = m.gameObject;
            }
            if (m.EvidenceInfo == receipt)
            {
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
        if (!allEvidenceFoundLatch && AllEvidenceFound())
        {
            allEvidenceFoundLatch = true;
            // invoke the action
            //OnAllEvidenceFound.Invoke();
            LevelOneCustomHandler h = GameObject.FindObjectOfType<LevelOneCustomHandler>();
            if (h != null)
            {
                h.onFoundAllEvidence.Invoke();
            } else
            {
                Debug.LogError("ERROR: UNable to find level one custom handler this is probably game breaking since we can't leave the tutorial");
            }
        }
    }
}
