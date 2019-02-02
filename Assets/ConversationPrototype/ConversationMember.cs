using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConversationMember : MonoBehaviour
{
    // this handles conversations! You give one of these with references to each of the text boxes in question, and then
    // you choose which one is the maxium, 
    // this needs a custom editor

    public string characterName = "Unnamed";
    public ConversationMember master; // only for slaves

    public TextMeshProUGUI text; // we may want to make this the non-UGUI version but for now it works
    public bool alwaysFaceCamera = true;
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool ValidateAllNames(bool printNames = true)
    {
        // this ensures that no-one is named the same thing, and prints out all the names
        ConversationMember[] members = FindObjectsOfType<ConversationMember>();
        if (printNames)
        {
            string allNames = members.Length + " members\nAll names:";
            for (int i = 0; i < members.Length; i++)
            {
                allNames += "\n" + members[i].characterName;
            }
            Debug.Log(allNames);
        }
        bool valid = true;
        for (int i = 0; i < members.Length; i++)
        {
            int numNamed = 0;
            for (int j = i+1; j < members.Length; j++)
            {
                // check if they're the same name
                if (members[i].characterName == members[j].characterName)
                {
                    numNamed++;
                }
            }
            if (numNamed > 1)
            {
                valid = false;
                Debug.Log("ERROR: " + numNamed + " characters named " + members[i].characterName);
            }
        }
        return valid;
    }
}
