using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public Level level;
    public List<EvidenceMono> evidenceFoundThisDay = new List<EvidenceMono>(); // this is used to keep track of the recipts of what's found this day
    private EvidenceMono[] evidenceMonos; // these are all the evidence in this level

    // Start is called before the first frame update
    void Start()
    {
        evidenceMonos = FindObjectsOfType<EvidenceMono>();
        List<SerializedEvidence> allEvidence = (level == Level.Office) ? EvidenceManager.instance.officeEv : EvidenceManager.instance.factoryEv;
        for (int i = 0; i < allEvidence.Count; i++)
        {
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
        for (int i = 0; i < GetEvidence().Length; i++)
        {
            if (GetEvidence()[i].gameObject.activeInHierarchy)
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
