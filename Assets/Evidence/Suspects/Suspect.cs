using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Suspect", menuName = "Suspect", order = 1)]
public class Suspect : ScriptableObject
{
    [SerializeField]
    private string _name;
    [SerializeField]
    [TextArea]
    private string _bio;

    public string Name
    {
        get { return name; }
    }
}
