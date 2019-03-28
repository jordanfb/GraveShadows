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
    private List<Evidence> _evidenceToGen;
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize some necessary fields
        _evidenceToGen = new List<Evidence>();
        _suspectTotals = new int[5];

        //Gather culprit data to add to evidence to generate list
        int culpritIndex = _allSuspects.IndexOf(_culprit);
        GatherCulpritAndConversationEvidence(culpritIndex);
        GatherRandomEvidence();
        foreach(Evidence e in _evidenceToGen)
        {
            Debug.Log("Evidence: " + e.Name);
            /*
            foreach(Suspect s in e.AssociatedSuspects)
            {
                Debug.Log("\tAssociated with: " + s.CodeName);
            }
            */
        }
        foreach (int i in _suspectTotals)
            Debug.Log(i);
        Debug.Log("Evidence Count: " + _evidenceToGen.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GatherCulpritAndConversationEvidence(int index)
    {
        foreach(Evidence ev in _allEvidence)
        {
            //If evidence is assoc. with no one and isn't a conversation, ignore it
            if (ev.AssociatedSuspects.Count == 0 && ev.GetEvidenceType != EvidenceType.Conversation)
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
                        _suspectTotals[_allSuspects.IndexOf(s)]++;
                    }
                }
            }
            //All conversations need to exist in any playthrough as well
            else if(ev.GetEvidenceType == EvidenceType.Conversation)
            {
                _evidenceToGen.Add(ev);
                foreach(Suspect s in ev.AssociatedSuspects)
                {
                    _suspectTotals[_allSuspects.IndexOf(s)]++;
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

        List<Evidence> remainingEv = new List<Evidence>(_allEvidence);
        foreach (Evidence e in _evidenceToGen)
            remainingEv.Remove(e);

        //Apparently not using the System namespace calls a vestigial UnityEngine Random class instead.
        //Also didn't want to use all of System AND either way I still needed to specify bc C# got angry.
        System.Random rng = new System.Random();

        while(_evidenceToGen.Count < _evidenceTotal)
        {
            int next = rng.Next(0, remainingEv.Count - 1);
            Evidence randEv = remainingEv[next];
            if(CheckIfPlaceable(randEv))
            {
                foreach (Suspect s in randEv.AssociatedSuspects)
                    _suspectTotals[_allSuspects.IndexOf(s)]++;
                _evidenceToGen.Add(randEv);
            }
        }
    }

    private bool CheckIfPlaceable(Evidence ev)
    {
        foreach(Suspect s in ev.AssociatedSuspects)
        {
            if (_suspectTotals[_allSuspects.IndexOf(s)] >= _otherSuspectMax)
                return false;
        }
        return true;
    }
}
