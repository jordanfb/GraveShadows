using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{

    public TextAsset script;
    public List<ConversationMember> slaves;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ParseScript()
    {
        // parse the script to find the people, as well as the lines in the script, and timings, etc.
    }

    public bool ValidateScript()
    {

        if (!ParseScript() || !FindSlaves())
        {
            return false;
        }
        // we know we're set finding the slaves,
        // so now validate the script itself for other magical reasons somehow?
        return true;
    }

    public bool FindSlaves()
    {
        // find all the conversation members in the scene and then check for names
        if (!ConversationMember.ValidateAllNames(false))
        {
            return false; // because the names were false. It will display the errors itself
        }
        // then find all the slaves in the script!

        // then find the slaves we need!

        return false; // didn't find all the slaves needed
    }
}
