using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomLevel1Logic : MonoBehaviour
{
    public UnityEvent onAllEvidenceCollected;

    int numCollected = 0;
    public int numEvidence = 0;

    bool alreadyInvoked = false;

    public void Start()
    {
        // check to see if you've already found all the evidence
        if (FindObjectOfType<LevelOneEvidenceManager>().AllEvidenceFound())
        {
            alreadyInvoked = true;
            onAllEvidenceCollected.Invoke();
        }
    }


    public void CollectedEvidence()
    {
        numCollected++;
        if (!alreadyInvoked && numCollected >= numEvidence)
        {
            onAllEvidenceCollected.Invoke();
        }
    }
}
