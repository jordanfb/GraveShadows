using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIObjectVisibility : MonoBehaviour
{
    [System.Serializable]
    public struct PartVisibility
    {

        public Transform part;
        [Tooltip("What percent visibilty does seeing this part impart?")]
        public float visibilityPercent; // if you see this part what should be added to the person visiblity?
    }
    // basically just a list of places where the ai should be able to see and a percent of visiblity 
    // seeing that implies
    public PartVisibility[] parts;
}
