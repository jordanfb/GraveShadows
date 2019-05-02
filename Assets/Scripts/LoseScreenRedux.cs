using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreenRedux : MonoBehaviour
{
    public Text Head1;
    public Text Subhead1;
    public Text Head2;
    public Text Subhead2;
    public Image bigBad;
    public Sprite mogul;
    public Sprite vault;
    public Sprite mustang;
    public Sprite inspector;
    public Sprite foreman;

    // Start is called before the first frame update
    void Start()
    {
        Head2.text = "Detective Found Dead";
        Subhead2.text = "Colton Graves, Thought Dead After Accident, Found Riddled with Bullets";
        string bBN = EvidenceManager.instance.culprit.Name;
        if (bBN == "Robert Brackenridge")
        {
            Head1.text = "Mogul Invests in Uranium Mine";
            Subhead1.text = "Robert Brackenridge Expands Interests into Mining, Reason Unknown";
            bigBad.sprite = mogul;
        }
        else if (bBN == "Patricia Engle")
        {
            Head1.text = "New CEO Funds Weapons Research";
            Subhead1.text = "Patricia Engle Pushes Grant Enterprises into Weapons Research";
            bigBad.sprite = vault;
        }
        else if (bBN == "Tulley Kee")
        {
            Head1.text = "Kee Shells Out, Navajo Nation Grows";
            Subhead1.text = "Prominent Foreman Funds Navajo Rejuvenation Projects";
            bigBad.sprite = foreman;
        }
        else if (bBN == "Thomas Carter")
        {
            Head1.text = "New Inspector Appointed Head of BIR";
            Subhead1.text = "Regulations Change under new Head, Thomas Carter";
            bigBad.sprite = inspector;
        }
        else if (bBN == "Meredith Earls")
        {
            Head1.text = "New Annual Car Show at Earls' Ranch";
            Subhead1.text = "Car Show hosted by Meredith Earls, Come on Down";
            bigBad.sprite = mustang;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
