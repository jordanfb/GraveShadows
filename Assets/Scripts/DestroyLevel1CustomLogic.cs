using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLevel1CustomLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // destroy the level 1 custom logic
        LevelOneEvidenceManager level1CustomLogic = FindObjectOfType<LevelOneEvidenceManager>();
        if (level1CustomLogic != null)
        {
            Destroy(level1CustomLogic.gameObject);
            LevelOneEvidenceManager.instance = null;
        }
    }
}
