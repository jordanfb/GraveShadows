using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenRedux : MonoBehaviour
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
        Head2.text = "FBI Raid Crime Syndicate";
        Subhead2.text = "FBI Cracks Down on Traitorous Weapons Dealers";
        string bBN = EvidenceManager.instance.culprit.Name;
        if (bBN == "Robert Brackenridge")
        {
            Head1.text = "Late Mogul Had Crime Connections";
            Subhead1.text = "Brackenridge Found to be Leader of Crime Syndicate";
            bigBad.sprite = mogul;
        }
        else if (bBN == "Patricia Engle")
        {
            Head1.text = "Crime Leader Killed, Identity Exposed";
            Subhead1.text = "Unsuspecting Engle Gunned Down in Connection with the Family";
            bigBad.sprite = vault;
        }
        else if (bBN == "Tulley Kee")
        {
            Head1.text = "Prominent Navajo Head Found Dead";
            Subhead1.text = "Deep Connections to Underworld Discovered";
            bigBad.sprite = foreman;
        }
        else if (bBN == "Thomas Carter")
        {
            Head1.text = "Inspector Shot Dead, Crime Syndicate Falls";
            Subhead1.text = "Evidence Arises Implicating Thomas Carter";
            bigBad.sprite = inspector;
        }
        else if (bBN == "Meredith Earls")
        {
            Head1.text = "Earls' Ranch Seized";
            Subhead1.text = "Merdith Earls Revealed as Family Head, Assets Seized";
            bigBad.sprite = mustang;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
