using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Evidence", menuName = "Evidence", order = 1)]
public class Evidence : ScriptableObject
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private Sprite _photo;
    [SerializeField]
    [TextArea]
    private string _flavorText;
    [SerializeField]
    private EvidenceType _evidenceType;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public Sprite Photo
    {
        get { return _photo; }
        set { _photo = value; }
    }

    public string FlavorText
    {
        get { return _flavorText; }
    }

    public EvidenceType GetEvidenceType
    {
        get { return _evidenceType; }
    }

    
}

public enum EvidenceType
{
    Object, Conversation, Document
}
