using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CulpritNameOnWinScreen : MonoBehaviour
{
    public static Text displayText;
    public GameObject textBox;

    public string nameOfCulprit;

    // Start is called before the first frame update
    void Start()
    {
        //does what the unity UI tutorial said it would do, i guess
        displayText = textBox.GetComponent<Text>();

        //gets the full name of the culprit
        nameOfCulprit = EvidenceManager.instance.culprit.Name;
    }

    // Update is called once per frame
    void Update()
    {
        //Sets the text displaying in the textBox game object to the first third of the article and replaces it with the culprit's name
        displayText.text = "    The crime syndicate known as the Nuclear Family met its end Friday night. " + nameOfCulprit + ", now known as the head of the group, was found dead in their home. Federal agents stepped in to assist local police and quickly discovered Police Chief Harold Clap- ton had been receiving payments from " + nameOfCulprit + " to turn a blind eye to the Family’s crimes. \n    Federal agents believe they have arrested all remaining members of the Nuclear Family, who have all ag- reed in interrogation that the murder victim was their boss. The Family has been accused for the murder of Detec- tive Marvin Mitchell, whose death had ";
    }
}
