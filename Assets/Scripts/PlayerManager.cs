using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    private List<GameObject> _collectedEvidence;
    /* for use later probably
    [SerializeField]
    private Player _player;
    */

    void Awake()
    {
        //This ensures we don't have multiple instances of the Player Manager
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public List<GameObject> CollectedEvidence
    {
        get { return _collectedEvidence; }
    }

    public void CollectEvidence(GameObject go)
    {
        _collectedEvidence.Add(go);
    }
}
