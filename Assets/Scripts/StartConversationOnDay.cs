using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConversationOnDay : MonoBehaviour
{
    // This is a monobehavior to give a public function that will start a conversation but only on a certain day (or excluding a certain day)
    // this is for creating "first day: I should try to find as much evidence as possible but avoid those guards!" and maybe a last day too

    public enum modeSpecificFunctionality
    {
        allModes, demoModeOnly, fullGameOnly
    }


    public int dayNum = 0;
    public string conversationName = "";
    public modeSpecificFunctionality validModes = modeSpecificFunctionality.allModes;

    private ConversationMetaManager manager;

    public void TryStartConversation()
    {
        if (GameplayManager.instance.dayNum == dayNum && (validModes == modeSpecificFunctionality.allModes || (Options.instance.demoMode == (validModes == modeSpecificFunctionality.demoModeOnly)) ) )
        {
            // only run on the specific day and if it's the correct mode
            if (manager != null && conversationName.Length > 0)
            {
                // it's possible to play the convo
                manager.PlayConversationOfName(conversationName);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<ConversationMetaManager>();
    }
}
