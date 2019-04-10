using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLine
{
    public string formattedText;
    public string unformattedText;
    public float[] characterTimes;

    public string characterName;
    public bool usePostDelay = true; // start the delay after the end of the line before it
    public float delayTime;
    public ConversationMember actor;
    public float speed;

    public ScriptLine(string character, string line, float speedIn)
    {
        //inputText
        characterName = character;
        formattedText = line;
        speed = speedIn;
        ParseLine();
    }

    private void ParseLine()
    {
        // parse the line and figure out the speeds and the formatting
        // formatting: <b></b>, <i></i>, <size=50></size>, <color=00ff00ff></color>
        // my formatting: <speed=.01></speed>
        // I should probably parse chances based on the word size so smaller words are harder to hear?
        // that would make sense plot wise, for now let's not do that though.
        string text = formattedText;
        int i = 0;
        // find the unformatted text
        unformattedText = "";
        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                while (text[i] != '>')
                {
                    i++;
                }
                i++;
            } else
            {
                // add the text to unformatted text
                unformattedText += text[i];
                i++;
            }
        }
        // now find the speeds for the text
        characterTimes = new float[unformattedText.Length];
        Stack<float> speeds = new Stack<float>(); // this is to store the effect of the speed tags
        speeds.Push(speed);
        i = 0;
        int unformattedi = 0;
        while (i < text.Length)
        {
            // currently the writers aren't allowed to use < normally, but that's probably not a big deal?
            // go through it and parse out all the innards and calculate the speeds!
            if (text[i] == '<')
            {
                if (text.Length >= i+1 && text[i + 1] == '/')
                {
                    // then it's a closing tag, check if it's speed, otherwise we don't care...
                    if (text.IndexOf("</speed>", i) == i)
                    {
                        // then it's a closing tag here
                        speeds.Pop(); // pop off the top of the stack
                    }
                    // then skip to the end of the closing tag
                    i = text.IndexOf(">", i) + 1;
                } else
                {
                    // it's an opening tag, so figure out what it is. If it's speed then we should add it to the stack
                    if (text.IndexOf("<speed=", i) == i)
                    {
                        // then it's a speed tag! Add it to the stack!
                        int endOfNumber = text.IndexOf('>', i);
                        if (endOfNumber != -1)
                        {
                            // then we figure out the speed
                            float trySpeed = 0;
                            if (float.TryParse(text.Substring(i + 7, endOfNumber - (i + 7)), out trySpeed))
                            {
                                speeds.Push(trySpeed);
                            }
                            else
                            {
                                Debug.LogError("Error parsing speed in line: " + formattedText);
                            }
                        } else
                        {
                            // we're in trouble oh dear
                            Debug.LogError("Parsing error in line: " + formattedText);
                        }
                    }
                    // skip to the end of the tag
                    i = text.IndexOf(">", i) + 1;
                }
            } else
            {
                characterTimes[unformattedi] = speeds.Peek();
                unformattedi++;
                i++; // for now just do this
            }
        }
        // then go and remove all the speed tags from the formatted line because they don't render correctly
        i = 0;
        while (i < formattedText.Length && (i = formattedText.IndexOf("<speed=", i)) != -1)
        {
            // we have an occurance of the speed tag, so remove it!
            formattedText.Remove(i, 7); // remove the <speed= part
            while (i < formattedText.Length && formattedText[i] != '>')
            {
                formattedText = formattedText.Remove(i, 1);
            }
            if (i < formattedText.Length)
            {
                formattedText = formattedText.Remove(i, 1); // remove the ">"
            }
        }
        formattedText = formattedText.Replace("</speed>", ""); // remove all occurances of the speed closing tag
        //while (formattedText.IndexOf("</string>") != -1)
        //{
        //    Debug.LogError("Replaced text " + formattedText);
        //}
    }

    private string GetOpenTag(string line, int startingPoint)
    {
        for (int i = startingPoint; i < line.Length; i++)
        {
            if (line[i] == '>')
            {
                // then it's closed here!
                //Debug.Log(line.Substring(startingPoint, i - startingPoint + 1));
                return line.Substring(startingPoint, i - startingPoint + 1);
            }
        }
        return "";
    }

    private string RemoveOpenTag(string line, string opentag)
    {
        return "";
    }

    private string GetClosingTag(string openTag)
    {
        // this will be passed something like <color=blue> or <size or <i>
        // and it returns the correct closing tag
        string inside = "";
        bool opened = false;
        for (int i = 0; i < openTag.Length; i++)
        {
            if (!opened && openTag[i] == '<')
            {
                opened = true;
            } else
            {
                // we've been opened so check if we should close otherwise add it to the inside
                if (openTag[i] == '=' || openTag[i] == '>')
                {
                    break; // we made it to the end of the important inside stuff
                }
                inside += openTag[i];
            }
        }
        return "</" + inside + ">";
    }
}

public class ConversationManager : MonoBehaviour
{

    public TextAsset script;
    public List<ConversationMember> slaves = new List<ConversationMember>();
    public List<ScriptLine> scriptLines = new List<ScriptLine>();
    public Evidence associatedEvidence;

    public string scriptName = "";
    public string conversationDescription = "";
    public string scriptHeader = "";
    public string scriptContent = "";
    public List<string> characterNames = new List<string>();
    public float defaultSpeed = 1; // this is way too slow but for now it works
    public bool defaultUsePostDelay = true;
    public float defaultDelay = 0;

    private int conversationLineNumber = 0;
    private float conversationTimer = 0;
    private bool running = false;

    public bool Running
    {
        get { return running; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (script != null)
        {
            ValidateScript();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            // update the timer using predelay or post delay (when the line before is done) or if it's the first element of the script
            if (conversationLineNumber < scriptLines.Count)
            {
                if (conversationLineNumber == 0 || !scriptLines[conversationLineNumber].usePostDelay ||
                    (scriptLines[conversationLineNumber].usePostDelay && scriptLines[conversationLineNumber - 1].actor.IsFinished()))
                {
                    conversationTimer += Time.deltaTime;
                }
                if (conversationTimer > scriptLines[conversationLineNumber].delayTime)
                {
                    conversationTimer = 0;
                    // start running the next line
                    scriptLines[conversationLineNumber].actor.SayLine(scriptLines[conversationLineNumber]);
                    conversationLineNumber++;
                    // if we make it to the end then it'll wait for the final actor to be finished and then it'll stop running
                }
            } else if (scriptLines[conversationLineNumber - 1].actor.IsFinished())
            {
                // when the final actor is finished it'll stop running the script
                StopRunningScript();
            }
        }
    }

    [ContextMenu("Run Script")]
    public void StartRunningScript()
    {
        // we give the evidence to the player since it started playing the conversation:
        // we also check to see if we've already collected it in which case we don't play the conversation
        if (associatedEvidence)
        {
            // then collect it!
            if (PlayerManager.instance == null)
            {
                Debug.LogError("NO PLAYER MANAGER WHY");
            }
            else if (PlayerManager.instance.HasCollected(associatedEvidence))
            {
                return; // don't play the evidence we've already collected it!
            }
            else
            {
                PlayerManager.instance.CollectEvidence(associatedEvidence);
            }
        }
        ResetScript();
        if (scriptLines.Count == 0)
        {
            return; // if there are no lines then you can't start
        }
        running = true;
    }

    [ContextMenu("Reset Script")]
    public void ResetScript()
    {
        conversationLineNumber = 0;
        conversationTimer = 0;
    }

    [ContextMenu("Stop Running")]
    public void StopRunningScript()
    {
        running = false;
    }

    private bool ParseScript()
    {
        scriptLines = new List<ScriptLine>();
        characterNames = new List<string>();
        defaultUsePostDelay = true;
        defaultDelay = 0;
        defaultSpeed = 1;
        // parse the script to find the people, as well as the lines in the script, and timings, etc.
        int headerEnd = script.text.ToLower().IndexOf("#endheader");
        if (headerEnd != -1)
        {
            scriptHeader = script.text.Substring(0, headerEnd).Replace("#header", "");
            scriptContent = script.text.Substring(headerEnd + 10);
            string[] headerLines = scriptHeader.Split('\n');
            // validate the header
            for (int i = 0; i < headerLines.Length; i++)
            {
                string line = headerLines[i];
                if (line.Length <= 1 || (line.Length >= 2 && line[0] == '/' && line[1] == '/'))
                {
                    continue; // we don't care about lines with length 1 (i.e. newlines) or comments
                }
                //Debug.Log(line);
                if (line.ToLower().Contains("#character"))
                {
                    // then it's a character definition
                    characterNames.Add(line.Substring(11).Trim());
                }
                else if (line.ToLower().Contains("#speed"))
                {
                    // handling default speed
                    string[] splitLine = line.Split(' ');
                    float trySpeed = 0;
                    if (splitLine.Length == 2 && float.TryParse(splitLine[1], out trySpeed))
                    {
                        defaultSpeed = trySpeed;
                    }
                    else
                    {
                        Debug.LogError("Badly formatted line: " + line);
                    }
                }
                else if (line.ToLower().Contains("#predelay"))
                {
                    // handling default predelay
                    defaultUsePostDelay = false;
                    string[] splitLine = line.Split(' ');
                    float tryDelay = 0;
                    if (splitLine.Length == 2 && float.TryParse(splitLine[1], out tryDelay))
                    {
                        defaultDelay = tryDelay;
                    }
                    else
                    {
                        Debug.LogError("Badly formatted line: " + line);
                    }
                }
                else if (line.ToLower().Contains("#postdelay") || line.ToLower().Contains("#delay"))
                {
                    // handling default postdelay
                    defaultUsePostDelay = true;
                    string[] splitLine = line.Split(' ');
                    float tryDelay = 0;
                    if (splitLine.Length == 2 && float.TryParse(splitLine[1], out tryDelay))
                    {
                        defaultDelay = tryDelay;
                    }
                    else
                    {
                        Debug.LogError("Badly formatted line: " + line);
                    }
                }
                else if (line.ToLower().Contains("#description"))
                {
                    conversationDescription = line.Substring("#description".Length + 1);
                }
                else if (line.ToLower().Contains("#scriptname"))
                {
                    scriptName = line.Substring("#scriptname".Length + 1);
                }
            }
            // now validate the rest of the script, we want lines starting with #CharacterName, i.e. #Jordan
            string[] splitScriptLines = scriptContent.Split('\n');
            bool hasCharacterLine = false;
            bool postDelay = defaultUsePostDelay;
            float delay = defaultDelay;
            float speed = defaultSpeed;
            string currentCharacter = "";
            string currentLine = ""; // the line the character says
            for (int i = 0; i < splitScriptLines.Length; i++)
            {
                string line = splitScriptLines[i];
                // for now we have #Jordan and #endline
                if (!hasCharacterLine)
                {
                    if (line.Length > 0 && line[0] == '#')
                    {
                        // then it could be a character definition
                        for (int j = 0; j < characterNames.Count; j++)
                        {
                            // see if the line is a character definition
                            if (line.Contains("#"+characterNames[j]))
                            {
                                // then set the character
                                currentCharacter = characterNames[j];
                                hasCharacterLine = true;
                                currentLine = "";

                                // reset these to the default each new character line in the script
                                postDelay = defaultUsePostDelay;
                                delay = defaultDelay;
                                speed = defaultSpeed;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // add the line to the speech for that
                    if (line.Length > 0 && line[0] == '#' && line.Contains("#predelay"))
                    {
                        postDelay = false;
                        string[] splitLine = line.Split(' ');
                        float tryDelay = 0;
                        if (splitLine.Length == 2 && float.TryParse(splitLine[1], out tryDelay))
                        {
                            // then we have a delay!
                            delay = tryDelay;
                            // we may need to do something else too idk.
                        } else
                        {
                            Debug.LogError("Badly formatted line: " + line);
                        }
                    }
                    else if (line.Length > 0 && line[0] == '#' && (line.Contains("#postdelay") || line.Contains("#delay")))
                    {
                        postDelay = true;
                        string[] splitLine = line.Split(' ');
                        float tryDelay = 0;
                        if (splitLine.Length == 2 && float.TryParse(splitLine[1], out tryDelay))
                        {
                            // then we have a delay!
                            delay = tryDelay;
                            // we may need to do something else too idk.
                        }
                        else
                        {
                            Debug.LogError("Badly formatted line: " + line);
                        }
                    }
                    else if ((line.Length > 0 && line[0] == '#' && line.Contains("#endline")) || line == "#\r" || line == "#" || line == "#\n")
                    {
                        // it's the end of the character's line, so create the line for the character
                        hasCharacterLine = false;
                        //Debug.Log("LINE for " + currentCharacter + ": " + currentLine);
                        ScriptLine currentScriptLine = new ScriptLine(currentCharacter, currentLine, speed);
                        currentScriptLine.delayTime = delay;
                        currentScriptLine.usePostDelay = postDelay;
                        scriptLines.Add(currentScriptLine);
                    }
                    else
                    {
                        //Debug.Log(line + ":" + line.Length);
                        // add it to the character's line
                        if (currentLine.Length > 0)
                        {
                            // add a new line before it
                            currentLine += "\n";
                        }
                        currentLine += line;
                    }
                }
            }
            // now that we've reached the end just make sure that we haven't errored at all
            if (hasCharacterLine)
            {
                return false; // it should have finished off all of the lines
            } else
            {
                return true; // we finished everything nicely enough so we're good!
            }
        }
        return false;
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

    public void OnDestroy()
    {
        foreach (ConversationMember m in slaves)
        {
            m.RemoveMaster(this);
        }
        slaves.Clear();
    }

    public bool FindSlaves()
    {
        // find all the conversation members in the scene and then check for names
        if (!ConversationMember.ValidateAllNames(false))
        {
            return false; // because the names were false. It will display the errors itself
        }
        // clear your current slaves
        foreach (ConversationMember m in slaves)
        {
            if (m == null)
                continue;
            m.RemoveMaster(this);
        }
        slaves.Clear();
        // then find all the slaves in the script!
        // we have a list of character names from parsing the script
        // then find the slaves we need!
        bool foundAllCharacters = true;
        ConversationMember[] members = FindObjectsOfType<ConversationMember>();
        for (int i = 0; i < characterNames.Count; i++)
        {
            // find the character with that name
            bool foundCharacter = false;
            for (int j = 0; j < members.Length; j++)
            {
                if (members[j].characterName.ToLower().Equals(characterNames[i].ToLower()))
                {
                    // then we found the character
                    foundCharacter = true;
                    // add the character to the list of slaves
                    slaves.Add(members[j]);
                    members[j].AddMaster(this);
                    // we should also link it to all of its lines in the conversation
                    LinkCharacterToLines(members[j]);
                    break;
                }
            }
            if (!foundCharacter)
            {
                Debug.LogError("Error: Unable to find character named \"" + characterNames[i] + "\"");
                foundAllCharacters = false;
            }
        }
        return foundAllCharacters; // didn't find all the slaves needed
    }

    private void LinkCharacterToLines(ConversationMember member)
    {
        for (int i = 0; i < scriptLines.Count; i++)
        {
            if (scriptLines[i].characterName.ToLower() == member.characterName.ToLower())
            {
                scriptLines[i].actor = member;
            }
        }
    }
}
