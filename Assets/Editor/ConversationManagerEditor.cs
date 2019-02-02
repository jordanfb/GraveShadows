using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[CustomEditor(typeof(ConversationManager))]
public class ConversationManagerEditor : Editor
{
    //private void DeleteSlave(object slaveIndex)
    //{
    //    ConversationMember m = (ConversationMember)target;
    //    m.slaves.RemoveAt((int)slaveIndex);
    //}

    private bool displaySlaves = false;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        ConversationManager m = (ConversationManager)target;

        // first select the script
        m.script = (TextAsset)EditorGUILayout.ObjectField("Script:", m.script, typeof(TextAsset), true);
        // then have a button to find all the named slaves (and error if it can't find the slaves)
        if (m.script != null && GUILayout.Button("Validate Script and Find Slaves"))
        {
            // then validate the script and make it all work!
            m.ValidateScript();
        }

        // then add a list of all of the members of the conversation
        displaySlaves = EditorGUILayout.Foldout(displaySlaves, "Slaves");
        if (displaySlaves)
        {
            for (int i = 0; i < m.slaves.Count; i++)
            {
                m.slaves[i] = (ConversationMember)EditorGUILayout.ObjectField("Slave " + i, m.slaves[i], typeof(ConversationMember), true);
            }
        }
    }
}
