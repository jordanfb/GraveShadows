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
    private List<Evidence> _evidenceToGen;
    // Start is called before the first frame update
    void Start()
    {
        //Initialize some necessary fields
        _evidenceToGen = new List<Evidence>();
        _suspectTotals = new int[5];

        //Gather culprit data to add to evidence to generate list
        int culpritIndex = _allSuspects.IndexOf(_culprit);
        GatherCulpritEvidence(culpritIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GatherCulpritEvidence(int index)
    {
        foreach(Evidence ev in _allEvidence)
        {
            //If evidence is assoc. with no one, ignore it
            if (ev.AssociatedSuspects.Count == 0)
                continue;
            if(ev.AssociatedSuspects.Contains(_culprit))
            {
                //Add evidence to generation queue if it belongs to culprit
                _evidenceToGen.Add(ev);
                _suspectTotals[index]++;
                //If it points to any other evidence too, add it to the totals
                if(ev.AssociatedSuspects.Count > 1)
                {
                    foreach(Suspect s in ev.AssociatedSuspects)
                    {
                        int i = _allSuspects.IndexOf(s);
                        _suspectTotals[i]++;
                    }
                }
            }
        }
    }
}
