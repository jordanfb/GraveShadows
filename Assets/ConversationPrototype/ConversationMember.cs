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
    public List<ConversationManager> masters = new List<ConversationManager>(); // only for slaves

    public TextMeshProUGUI text; // we may want to make this the non-UGUI version but for now it works
    public bool alwaysFaceCamera = true;

    private float timer = 0;
    private int characterNumber = 0;
    private int unformattedCharacterNumber = 0;
    private ScriptLine line;

    public void InterruptConversation(string newLine)
    {

    }

    public void StopConversation()
    {

    }

    public void PlayConversation()
    {

    }

    public void PauseConversation()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFinished())
        {
            // then say stuff!
            timer += Time.deltaTime;
            while (unformattedCharacterNumber >= line.unformattedText.Length || timer >= line.characterTimes[unformattedCharacterNumber])
            {
                // this should be replaced with the actual character times once I get parsing working
                // then add it to the line!
                timer = 0;
                characterNumber++;
                if (characterNumber < line.formattedText.Length)
                {
                    while (line.formattedText[characterNumber] == '<')
                    {
                        // then add the entire tag to what we're displaying
                        while (line.formattedText[characterNumber] != '>')
                        {
                            characterNumber++;
                        }
                        characterNumber++; // to get exclude the '>'
                    }
                }
                unformattedCharacterNumber++;
                UpdateTextToDisplay();
                if (IsFinished())
                {
                    // this handles the fact that it's now a while loop to display speed = 0 things immediately not just every frame.
                    break;
                }
            }
        }
    }

    [ContextMenu("Clear Masters")]
    public void ClearMasters()
    {
        masters.Clear();
    }

    private void UpdateTextToDisplay()
    {
        // display characterNumber characters
        if (unformattedCharacterNumber < line.unformattedText.Length)
        {
            text.text = line.formattedText.Substring(0, characterNumber);// + line.unformattedText.Substring(unformattedCharacterNumber);
        } else
        {
            text.text = line.formattedText.Substring(0, characterNumber);
        }
        // this needs to be FIXed because it'll shimmy around when more text gets added. Gotta use the transparent color formatting
    }

    public void SayLine(ScriptLine line)
    {
        //Debug.Log("Being told to say:\n" + line.formattedText);
        timer = 0;
        characterNumber = 0;
        unformattedCharacterNumber = 0;
        this.line = line;
    }

    public bool IsFinished()
    {
        return line == null || characterNumber >= line.formattedText.Length;
    }

    public void AddMaster(ConversationManager m)
    {
        masters.Add(m);
    }

    public void RemoveMaster(ConversationManager m)
    {
        masters.Remove(m);
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
