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

    private List<Evidence> _officeGenQueue;
    private List<Evidence> _factoryGenQueue;
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize some necessary fields
        _officeGenQueue = new List<Evidence>();
        _factoryGenQueue = new List<Evidence>();
        _suspectTotals = new int[5];

        //Gather culprit data to add to evidence to generate list
        int culpritIndex = _allSuspects.IndexOf(_culprit);
        GatherCulpritAndConversationEvidence(culpritIndex);
        GatherRandomEvidence();

        //Debug.Log("Office: " + _officeGenQueue.Count);
        //Debug.Log("Factory: " + _factoryGenQueue.Count);
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
                switch(ev.GetLevel)
                {
                    case Level.Factory:
                        _factoryGenQueue.Add(ev);
                        break;
                    case Level.Office:
                        _officeGenQueue.Add(ev);
                        break;
                }
                _suspectTotals[index]++;

                //If it points to any other evidence too, add it to the totals
                if(ev.AssociatedSuspects.Count > 1)
                {
                    foreach(Suspect s in ev.AssociatedSuspects)
                    {
                        if(s != _culprit)
                            _suspectTotals[_allSuspects.IndexOf(s)]++;
                    }
                }
            }
            //All conversations need to exist in any playthrough as well
            else if(ev.GetEvidenceType == EvidenceType.Conversation)
            {
                switch(ev.GetLevel)
                {
                    case Level.Factory:
                        _factoryGenQueue.Add(ev);
                        break;
                    case Level.Office:
                        _officeGenQueue.Add(ev);
                        break;
                }
                
                foreach(Suspect s in ev.AssociatedSuspects)
                {
                    _suspectTotals[_allSuspects.IndexOf(s)]++;
                }
            }
        }
    }

    private void GatherRandomEvidence()
    {
        if(_officeGenQueue.Count + _factoryGenQueue.Count >= _evidenceTotal)
        {
            Debug.LogError("There's already more evidence than can be generated!");
            return;
        }

        List<Evidence> remainingEv = new List<Evidence>(_allEvidence);
        foreach (Evidence e in _factoryGenQueue)
            remainingEv.Remove(e);
        foreach (Evidence e in _officeGenQueue)
            remainingEv.Remove(e);

        //Apparently not using the System namespace calls a vestigial UnityEngine Random class instead.
        //Also didn't want to use all of System AND either way I still needed to specify bc C# got angry.
        System.Random rng = new System.Random();

        while(_officeGenQueue.Count < _evidenceTotal / 2 || _factoryGenQueue.Count < _evidenceTotal / 2)
        {
            if (remainingEv.Count < 1)
                break;

            int next = rng.Next(0, remainingEv.Count - 1);
            Evidence randEv = remainingEv[next];
            if(CheckIfPlaceable(randEv))
            {
                foreach (Suspect s in randEv.AssociatedSuspects)
                    _suspectTotals[_allSuspects.IndexOf(s)]++;
                switch(randEv.GetLevel)
                {
                    case Level.Factory:
                        if(_factoryGenQueue.Count < _evidenceTotal / 2)
                            _factoryGenQueue.Add(randEv);
                        break;
                    case Level.Office:
                        if(_officeGenQueue.Count < _evidenceTotal / 2)
                            _officeGenQueue.Add(randEv);
                        break;
                }
            }
            remainingEv.Remove(randEv);
        }
    }

    private bool CheckIfPlaceable(Evidence ev)
    {
        if (ev.AssociatedSuspects.Count == 0)
            return true;
        foreach(Suspect s in ev.AssociatedSuspects)
        {
            if (_suspectTotals[_allSuspects.IndexOf(s)] >= _otherSuspectMax)
                return false;
        }
        return true;
    }
}
