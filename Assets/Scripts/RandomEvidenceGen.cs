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

    //Because we don't care about indexing and the random generation ignores duplicates,
    //I figured a set is the most efficient structure to use here.
    private HashSet<Evidence> _evidenceToGen;
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize some necessary fields
        _evidenceToGen = new HashSet<Evidence>();
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

    private void GatherRandomEvidence()
    {
        if(_evidenceToGen.Count >= _evidenceTotal)
        {
            Debug.LogError("There's already more evidence than can be generated!");
            return;
        }

        //Apparently not using the System namespace calls a vestigial UnityEngine Random class instead.
        //Also didn't want to use all of System AND either way I still needed to specify bc C# got angry.
        System.Random rng = new System.Random();

        //Is this efficient??? God I hope so.
        //Need to refactor
        while ( _evidenceToGen.Count < _evidenceTotal)
        {
            int next = rng.Next(0, _allEvidence.Count - 1);
            Evidence e = _allEvidence[next];
            _allEvidence.Add(e);
            
        }
    }
}
