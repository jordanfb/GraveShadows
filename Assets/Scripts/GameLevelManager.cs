using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public Level level;
    public List<Evidence> evidenceFoundThisDay = new List<Evidence>(); // this is used to keep track of the recipts of what's found this day
    private EvidenceMono[] evidenceMonos; // these are all the evidence in this level
    private List<Transform> evidenceLocations;


    // Start is called before the first frame update
    void Start()
    {
        if(EvidenceManager.instance == null) {
            Debug.Log("EvidenceManager not found");
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
            evidenceLocations.Add(go.transform);

        for (int i = 0; i < allEvidence.Count; i++)
        {
            if (i >= evidenceMonos.Length)
            {
                Debug.LogWarning("Not enough evidence monos in this scene for the evidence");
                break; // we can't do anything we don't have enough evidence monos so I guess we just die
            }
            evidenceMonos[i].EvidenceInfo = EvidenceManager.instance.ReferencedEntity(allEvidence[i]) as Evidence;
            if (allEvidence[i].evidenceState == SerializedEvidence.EvidenceState.NotFound)
            {
                evidenceMonos[i].gameObject.SetActive(true);
            }
            else
            {
                evidenceMonos[i].gameObject.SetActive(false);
            }
        }
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
        return true;
    }

    public EvidenceMono[] GetEvidence()
    {
        return evidenceMonos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
