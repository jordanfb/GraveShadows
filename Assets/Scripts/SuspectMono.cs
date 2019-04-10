using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspectMono : MonoBehaviour
{
    [SerializeField]
    private Suspect _suspect;

    public Suspect SuspectInfo
    {
        get { return _suspect; }
        set { _suspect = value; }
    }
}
