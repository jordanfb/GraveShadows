﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


[CustomEditor(typeof(ConversationMember))]
public class ConversationMemberEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        ConversationMember m = (ConversationMember)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Character Name:", GUILayout.ExpandWidth(false));
        EditorGUILayout.PrefixLabel("Character Name:");
        m.characterName = EditorGUILayout.DelayedTextField(m.characterName);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            // then validate the character names after you finish editing the name
            ConversationMember.ValidateAllNames();
        }

        // you're just a slave only show slave things
        if (m.master != null)
        {
            if (GUILayout.Button("Go to master"))
            {
                // then it focuses on the master object
                EditorGUIUtility.PingObject(m.master);
            }
        }
        else
        {
            // tell the user that there's no master
            EditorGUILayout.LabelField("This slave has no master");
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
