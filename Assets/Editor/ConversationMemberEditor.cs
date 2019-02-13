using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[CustomEditor(typeof(ConversationMember))]
public class ConversationMemberEditor : Editor
{

    private bool displayMasters = false;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        ConversationMember m = (ConversationMember)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Character Name:", GUILayout.ExpandWidth(false));
        EditorGUILayout.PrefixLabel("Character Name:");
        string newName = EditorGUILayout.DelayedTextField(m.characterName);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            // then validate the character names after you finish editing the name
            Undo.RecordObject(m, "Set name of ConversationMember");
            Debug.Log("Set character name");
            m.characterName = newName;
            ConversationMember.ValidateAllNames();
        }
        Undo.RecordObject(m, "Modified ConversationMember");
        // you're just a slave only show slave things
        if (m.masters.Count > 0)
        {
            displayMasters = EditorGUILayout.Foldout(displayMasters, "Masters");
            if (displayMasters) {
                for (int i = 0; i < m.masters.Count; i++)
                {
                    m.masters[i] = (ConversationManager)EditorGUILayout.ObjectField("Master " + i, m.masters[i], typeof(ConversationManager), true);
                    //if (GUILayout.Button("Ping Master"))
                    //{
                    //    // then it focuses on the master object
                    //    EditorGUIUtility.PingObject(m.masters[i]);
                    //}
                }
            }
        }
        else
        {
            // tell the user that there's no master
            EditorGUILayout.LabelField("This slave has no masters");
        }

        // here have things that should appear for slaves and masters like should face camera and whatever.
        // this should also have a link to the master so that life is easy.
        // things like "should face camera constantly" and perhaps more
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Text Box Settings:");
        if (m.text == null && GUILayout.Button("Find Text Asset"))
        {
            // then check for a text item in itself and then its children
            TextMeshProUGUI t = m.GetComponent<TextMeshProUGUI>();
            if (t != null)
            {
                m.text = t;
            } else
            {
                // check in children
                t = m.GetComponentInChildren<TextMeshProUGUI>();
                if (t != null)
                {
                    m.text = t;
                }
            }
        }

        // display the text component
        m.text = (TextMeshProUGUI)EditorGUILayout.ObjectField(m.text, typeof(TextMeshProUGUI), true);

        // now we set other things like booleans to always face the camera
        m.alwaysFaceCamera = EditorGUILayout.Toggle("Always Face Camera", m.alwaysFaceCamera);
    }
}
