using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script
{
    private string script;


    public Script(string inputScript)
    {
        // then validate it and all that
        script = inputScript;
        // this script class stores all the members in the conversations, it stores where in
        // the conversation we are, it stores magic too?
        // do I need this here or should it all be stored in the conversation manager?
        // honestly it should just be in the conversation manager.
    }

    public bool ValidateScript()
    {
        // validates the script
        return false;
    }
}
