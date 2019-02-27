using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationMetaManager : MonoBehaviour
{
    // this manages conversation managers. The fact that this is neccessary is probably a bad thing.
    // Likely the correct idea would be to merge all conversation managers into this.

    ConversationManager[] managers;

    private void Start()
    {
        managers = FindObjectsOfType<ConversationManager>();
    }

    public void PlayConversationOfName(string scriptName)
    {
        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i].scriptName == scriptName)
            {
                managers[i].StartRunningScript();
                break;
            }
        }
    }

    public void PlayConversation(TextAsset script)
    {
        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i].script == script)
            {
                managers[i].StartRunningScript();
                break;
            }
        }
    }
}
