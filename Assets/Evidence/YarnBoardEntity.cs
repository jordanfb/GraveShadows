using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class YarnBoardEntity : ScriptableObject
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private Sprite _photo;

    public string Name
    {
        get { return _name; }
    }

    public Sprite Photo
    {
        get { return _photo; }
        set { _photo = value; }
    }
}
