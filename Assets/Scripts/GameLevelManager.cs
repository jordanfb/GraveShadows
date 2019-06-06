using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public Level level;
    public List<Evidence> evidenceFoundThisDay = new List<Evidence>(); // this is used to keep track of the recipts of what's found this day
    private EvidenceMono[] evidenceMonos; // these are all the evidence in this level
    private List<Transform> evidenceLocations = new List<Transform>();

    private LevelOneEvidenceManager loem;

    // Start is called before the first frame update
    void Start()
    {
        if(EvidenceManager.instance == null) {
            //Debug.Log("EvidenceManager not found");
            return;
        }

        if (level == Level.Hub)
            return;

        if(level == Level.Apartment)
        {
            loem = GameObject.FindObjectOfType<LevelOneEvidenceManager>();
            if (loem == null)
            {
                Debug.LogError("There is no level one evidence manager in the scene!");
                return;
            }                
            CheckCollectedLevelOneEvidence();
            return;
        }

        evidenceMonos = FindObjectsOfType<EvidenceMono>();
        List<SerializedEvidence> allEvidence = new List<SerializedEvidence>();
        if (level == Level.Office)
        {
            allEvidence = EvidenceManager.instance.officeEv;
        } else if (level == Level.Factory)
        {
            allEvidence = EvidenceManager.instance.factoryEv;
        } else if (level == Level.Apartment)
        {
            Debug.LogWarning("Need to be able to spawn in the evidence!!!!");
        }

        // We need the transforms for all the placeholders
        GameObject[] placeholders = GameObject.FindGameObjectsWithTag("Evidence");
        foreach (GameObject go in placeholders)
        {
            evidenceLocations.Add(go.transform);
            go.SetActive(false);
        }

        //Debug.Log("Evidence: " + allEvidence.Count);
        for(int i = 0; i < allEvidence.Count; i++)
        {
            Evidence e = EvidenceManager.instance.ReferencedEntity(allEvidence[i]) as Evidence;
            if(e == null)
            {
                Debug.LogError("Suspect exists in the evidence list!");
                continue;
            }

            if(allEvidence[i].evidenceState == SerializedEvidence.EvidenceState.NotFound)
            {
                GameObject prefab = EvidenceManager.instance.GetAssociatedPrefab(e);
                if(prefab != null)
                {
                    GameObject go = Instantiate(prefab);
                    go.transform.position = evidenceLocations[i].position;
                    EvidenceMono em = go.GetComponent<EvidenceMono>();
                    if(em.EvidenceInfo == null)
                    {
                        em.EvidenceInfo = e;
                    }
                    // Something about waist level stuff
                    if (placeholders[i].GetComponent<EvidenceMono>().isWaistLevel)
                        em.isWaistLevel = true;
                }
                else
                {
                    Debug.LogError("Missing gameobject in prefab list. Does not exist for evidence: " + e.Name + "!"); // this gets stuff happening
                }
            }
        }
    }

    void CheckCollectedLevelOneEvidence()
    {
        if (loem.keyFound)
            evidenceFoundThisDay.Add(loem.key);
        if (loem.receiptFound)
            evidenceFoundThisDay.Add(loem.receipt);
    }

    public bool HasFoundEverything()
    {
        // returns if everything has been disabled
        EvidenceMono[] currentAliveEvidence = FindObjectsOfType<EvidenceMono>();
        for (int i = 0; i < currentAliveEvidence.Length; i++)
        {
            if (currentAliveEvidence[i].gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        // I'm pretty sure that if they're disabled they won't get returned by the find object call, so the length would be zero.
        // it may be interesting figuring that out
        return true;
    }

    public EvidenceMono[] GetEvidence()
    {
        return evidenceMonos;
    }
}
