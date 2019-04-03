using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    public Level level;
    // Start is called before the first frame update
    void Start()
    {
        EvidenceMono[] evidenceMonos = FindObjectsOfType<EvidenceMono>();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
