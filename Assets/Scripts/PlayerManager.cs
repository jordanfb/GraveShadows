using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    private List<Evidence> _collectedEvidence;
    /* for use later probably
    [SerializeField]
    private Player _player;
    */

    void Awake()
    {
        //This ensures we don't have multiple instances of the Player Manager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public List<Evidence> CollectedEvidence
    {
        get { return _collectedEvidence; }
    }

    public void CollectEvidence(Evidence ev)
    {
        if (_collectedEvidence.Contains(ev))
            return;
        _collectedEvidence.Add(ev);


        // then collect it with other things as well!
        EvidenceManager.instance.FindEvidence(ev);
        GameLevelManager lm = FindObjectOfType<GameLevelManager>();
        if (lm)
        {
            lm.evidenceFoundThisDay.Add(ev);
        }
        else
        {
            Debug.LogError("Unable to log evidence unable to find gamelevelmanager");
        }
    }
}