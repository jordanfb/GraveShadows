using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CulpritReplacementScript : MonoBehaviour
{
    public string toReplace = "CULPRIT";
    public Text textToReplaceIn;

    string culprit = "CULPRIT";
    // Start is called before the first frame update
    void Start()
    {
        culprit = EvidenceManager.instance.culprit.Name;
    }

    // Update is called once per frame
    void Update()
    {
        textToReplaceIn.text = textToReplaceIn.text.Replace(toReplace, culprit);
    }
}
