using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EvidenceMono : MonoBehaviour
{
    [SerializeField]
    private Evidence _evidenceInfo;
    public bool isWaistLevel;
    [System.Serializable]


    public class EvidenceEvent : UnityEvent<Evidence>
    {

    }
    public EvidenceEvent onEvidenceCollected; // what gets invoked upon collecting this element of evidence


    // Update is called once per frame
    void Update()
    {
        
    }

    public Evidence EvidenceInfo
    {
        get { return _evidenceInfo; }
        set { _evidenceInfo = value; }
    }

    public void CollectThisEvidence()
    {
        onEvidenceCollected.Invoke(EvidenceInfo);
        PlayerManager.instance.CollectEvidence(EvidenceInfo);
    }
}
