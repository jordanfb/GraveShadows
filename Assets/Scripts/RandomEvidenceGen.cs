using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvidenceGen : MonoBehaviour
{
    [SerializeField]
    private Suspect _culprit;
    [SerializeField]
    private List<Evidence> _allEvidence;
    [SerializeField]
    private List<Suspect> _allSuspects;
    [SerializeField]
    private int _evidenceTotal;
    [SerializeField]
    private int _otherSuspectMax;
    [SerializeField]
    private float _spawnPercentage;

    private int[] _suspectTotals; //Index corresponds to index of suspect in _allSuspects
    // Start is called before the first frame update
    void Start()
    {
        int culpritIndex = _allSuspects.IndexOf(_culprit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GatherCulpritEvidence(int index)
    {

    }
}
