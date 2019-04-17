using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomLevel1Logic : MonoBehaviour
{
    public UnityEvent onAllEvidenceCollected;

    int numCollected = 0;
    public int numEvidence = 0;

    public void CollectedEvidence()
    {
        numCollected++;
        if (numCollected >= numEvidence)
        {
            onAllEvidenceCollected.Invoke();
        }
    }
}
