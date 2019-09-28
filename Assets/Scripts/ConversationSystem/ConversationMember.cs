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

    public GameObject enableDisableUponSpeaking;

    public bool skipOnClick = false; // true for monologues

    public TextMeshProUGUI text; // we may want to make this the non-UGUI version but for now it works
    public bool alwaysFaceCamera = true;

    private float timer = 0;
    private int characterNumber = 0;
    private int unformattedCharacterNumber = 0;
    private ScriptLine line;

    private float doneTalkingTimer = 0; // if this is 0 then it disables everything
    private bool overrideFinishedLine = false;

    private ScriptLine interuptedLine;
    private List<ConversationManager> runningManagersWhenInterupted = new List<ConversationManager>();
    bool resumeAfterFinished = false;

    public bool InterruptConversation(string newLine, float speed = .05f, bool keepTrackOfInteruppted = true, bool resumeAfterFinished = false)
    {
        this.resumeAfterFinished = resumeAfterFinished;
        if (keepTrackOfInteruppted)
        {
            runningManagersWhenInterupted.Clear();
            interuptedLine = line;
            foreach (ConversationManager m in masters)
            {
                if (m != null)
                {
                    //Debug.Log("Have a conversation manager");
                    //Debug.Log("Manager script " + m.scriptName);
                    if (m.Running)
                    {
                        //Debug.Log("It's running, so add it to the list");
                        runningManagersWhenInterupted.Add(m);
                    }
                    m.StopRunningScript(); // FIX so it pauses the script instead
                }
            }
            if (runningManagersWhenInterupted.Count > 1)
            {
                //Debug.LogError("ERROR: Multiple conversations running somehow");
            }
        }
        SayLine(new ScriptLine(characterName, newLine, speed));
        return runningManagersWhenInterupted.Count > 0; // at least one was running
    }

    public void ResumeConversation()
    {
        // resume the conversation then
        // now that we've finished our line to say to merge back into the conversation again, we resume
        foreach (ConversationManager m in runningManagersWhenInterupted)
        {
            //m.// resume the script I guess
            m.ContinueScript();
        }
        //line = interuptedLine;
        interuptedLine = null;
    }

    //public void StopConversation()
    //{

    //}

    //public void PlayConversation()
    //{

    //}

    //public void PauseConversation()
    //{

    //}

    private void FaceCamera()
    {
        transform.LookAt(Camera.main.transform);
        // for some reason they need to be flipped after they look at the camera?
        transform.Rotate(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (alwaysFaceCamera)
        {
            FaceCamera();
        }
        if (IsFinished() && doneTalkingTimer > 0)
        {
            doneTalkingTimer -= Time.deltaTime;
            if (doneTalkingTimer <= 0)
            {
                FinishedTalkingHandling();
            }

            if (skipOnClick && Input.GetMouseButtonDown(0))
            {
                // left click skips it if the bool is true
                overrideFinishedLine = true;
                characterNumber = line.formattedText.Length;
                FinishedTalkingHandling();
            }
        }
        if (!IsFinished())
        {
            if (skipOnClick && Input.GetMouseButtonDown(0))
            {
                // left click skips it if the bool is true
                if (unformattedCharacterNumber >= line.unformattedText.Length)
                {
                    // then skip to the next dialog, make it finished
                    // this will likely never happen (instead it gets used later down in this function)
                    overrideFinishedLine = true;
                    characterNumber = line.formattedText.Length;
                    FinishedTalkingHandling();
                    return;
                }
                else
                {
                    // otherwise display all the text
                    unformattedCharacterNumber = line.unformattedText.Length;
                }
            }
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
                    if (enableDisableUponSpeaking != null)
                    {
                        enableDisableUponSpeaking.SetActive(false); // disable it
                    }
                    break;
                }
            }
            if (enableDisableUponSpeaking != null)
            {
                enableDisableUponSpeaking.SetActive(true);
            }
            doneTalkingTimer = 3; // so that the ui stays around for an extra second
        }
    }

    private void FinishedTalkingHandling()
    {
        doneTalkingTimer = 0;
        if (enableDisableUponSpeaking != null && enableDisableUponSpeaking.activeSelf)
        {
            // empty the text and set it not active
            text.text = "";
            enableDisableUponSpeaking.SetActive(false); // disable it
        }
        if (resumeAfterFinished)
        {
            // then resume!
            ResumeConversation();
            resumeAfterFinished = false;
        }
    }

    public bool IsStillTalking()
    {
        return doneTalkingTimer > 0;
    }

    [ContextMenu("Clear Masters")]
    public void ClearMasters()
    {
        masters.Clear();
    }

    private void UpdateTextToDisplay()
    {
        // display characterNumber characters
        text.text = line.formattedText;
        text.maxVisibleCharacters = unformattedCharacterNumber;
        //if (unformattedCharacterNumber < line.unformattedText.Length)
        //{
        //    text.text = line.formattedText.Substring(0, characterNumber);// + line.unformattedText.Substring(unformattedCharacterNumber);
        //} else
        //{
        //    text.text = line.formattedText.Substring(0, characterNumber);
        //}
        // this needs to be FIXed because it'll shimmy around when more text gets added. Gotta use the transparent color formatting
    }

    public void SayLine(ScriptLine line)
    {
        //Debug.Log("Being told to say:\n" + line.formattedText);
        text.text = ""; // clear the text!
        timer = 0;
        characterNumber = 0;
        unformattedCharacterNumber = 0;
        overrideFinishedLine = false;
        this.line = line;
    }

    public bool IsFinished()
    {
        return line == null || characterNumber >= line.formattedText.Length || overrideFinishedLine;
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
                Debug.LogError("ERROR: " + numNamed + " characters named " + members[i].characterName);
            }
        }
        return valid;
    }
}
