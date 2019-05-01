using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLevel1CustomLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (LevelOneEvidenceManager.instance != null)
        {
            LevelOneEvidenceManager.instance.keyFound = false;
            LevelOneEvidenceManager.instance.receiptFound = false;
            LevelOneEvidenceManager.instance = null;
        }

        // search for and destroy the custom level one stuff if they exist
        foreach (CustomLevel1Logic logic in GameObject.FindObjectsOfType<CustomLevel1Logic>())
        {
            Destroy(logic.gameObject);
        }
        foreach (LevelOneEvidenceManager evidenceManager in GameObject.FindObjectsOfType<LevelOneEvidenceManager>())
        {
            Destroy(evidenceManager.gameObject);
        }
    }
}
