using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Suspect", menuName = "Suspect", order = 1)]
public class Suspect : YarnBoardEntity
{
    [SerializeField]
    private string _codeName;
    [SerializeField]
    [TextArea]
    private string _bio;

    public string CodeName
    {
        get { return _codeName; }
    }
}
