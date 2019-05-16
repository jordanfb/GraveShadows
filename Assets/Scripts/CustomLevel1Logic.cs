using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomLevel1Logic : MonoBehaviour
{
    public UnityEvent onAllEvidenceCollected;

    bool alreadyInvoked = false;
    LevelOneEvidenceManager evidenceManager;

    public void Start()
    {
        evidenceManager = FindObjectOfType<LevelOneEvidenceManager>();
        // check to see if you've already found all the evidence
        if (evidenceManager.AllEvidenceFound())
        {
            alreadyInvoked = true;
            onAllEvidenceCollected.Invoke();
        }
    }


    public void CollectedEvidence()
    {
        //numEvidenceFound++;
        //Debug.Log("COllected evidence already run: " + alreadyInvoked);
        if (!alreadyInvoked && evidenceManager.AllEvidenceFound())
        {
            alreadyInvoked = true;
            onAllEvidenceCollected.Invoke();
        }
    }

    private void Update()
    {
        if (!alreadyInvoked && evidenceManager.AllEvidenceFound())
        {
            alreadyInvoked = true;
            onAllEvidenceCollected.Invoke();
        }
    }
}
