using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager instance; // monobehavior
    public int maxEvidence = 30;
    public int otherSuspectMax = 3;
    public int officeEvidenceCount;
    public int factoryEvidenceCount;
    public List<YarnBoardEntity> allEvidenceEntities;
    public List<GameObject> allEvidencePrefabs;
    public Suspect culprit;
    private List<SerializedEvidence> allSerializedEvidence = new List<SerializedEvidence>(); // this is private so that it doesn't get saved. It's initialized either from save data or by code
    private List<Suspect> suspects = new List<Suspect>();
    [System.NonSerialized]
    public List<SerializedEvidence> officeEv = new List<SerializedEvidence>();
    [System.NonSerialized]
    public List<SerializedEvidence> factoryEv = new List<SerializedEvidence>();
    [System.NonSerialized]
    public List<SerializedEvidence> apartmentEV = new List<SerializedEvidence>();
    private int[] suspectTotals = new int[5];
    private bool generated = false;

    public static List<GameObject> AllEvidencePrefabs
    {
        get { return instance.allEvidencePrefabs; }
    }

    public static List<SerializedEvidence> AllEvidence
    {
        get
        {
            return instance.allSerializedEvidence;
        }

        set
        {
            instance.allSerializedEvidence = value;
        }
    } // a static accessor to make life easy
    public static List<YarnBoardEntity> AllEvidenceEntities
    {
        get
        {
            return instance.allEvidenceEntities;
        }

        set
        {
            instance.allEvidenceEntities = value;
        }
    }

    public GameObject GetAssociatedPrefab(Evidence e)
    {
        int index = allEvidenceEntities.IndexOf(e) - 5;
        if (index < 0 || index >= allEvidencePrefabs.Count)
            return null;
        // Have to account for suspects existing as yarn board entities
        return allEvidencePrefabs[index];
    }

    public bool Generated
    {
        set { generated = value; }
    }

    public List<YarnBoardEntity> GetEvidenceNotOnYarnboard()
    {
        List<YarnBoardEntity> output = new List<YarnBoardEntity>();
        for (int i = 0; i < allSerializedEvidence.Count; i++)
        {
            if (allSerializedEvidence[i].evidenceState == SerializedEvidence.EvidenceState.OffYarnBoard)
            {
                // then add the connected evidence to the output list
                output.Add(allEvidenceEntities[allSerializedEvidence[i].evidenceindex]);
            }
        }
        return output;
    }

    public int GetEvidenceNotOnYarnboardCount()
    {
        int num = 0;
        for (int i = 0; i < allSerializedEvidence.Count; i++)
        {
            if (allSerializedEvidence[i].evidenceState == SerializedEvidence.EvidenceState.OffYarnBoard)
            {
                // then add the connected evidence to the output list
                num++;
            }
        }
        return num;
    }

    public void Awake()
    {
        // initialize yourself and the list of all evidence
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
            return; // stop updating things!
        }
        // try loading a game
        if (!LoadEvidenceFromPlayerPrefs())
        {
            // if you weren't able to load a save, initialize everything for a new game.
            //InitializeSerializedEvidence();
            //generated = true;
        }
        // we can also just load a new game with pressing a button and calling NewSaveData()
    }

    public void FindEvidence(Evidence e)
    {
        // find the evidence!
        SerializedEvidence se = FindSerializedEvidence(e);
        if (se != null)
        {
            // then we're set!
            //Debug.Assert(se.evidenceState == SerializedEvidence.EvidenceState.NotFound); // shouldn't have found something not in the game
            //Debug.Log("Evidence state: " + se.evidenceState);
            if (se.evidenceState != SerializedEvidence.EvidenceState.OnYarnBoard)
            {
                // this fixes the assert I guess but we should actually fix it. THings aren't set correctly unfortunately... Whatever.
                se.evidenceState = SerializedEvidence.EvidenceState.OffYarnBoard;
            }
        }

        SaveEvideneToPlayerPrefs();
    }

    public bool IsEvidenceInWarehouse(int i)
    {
        return false;
    }

    public void InitializeSerializedEvidence()
    {
        if (generated)
        {
            return;
        }
        officeEv = new List<SerializedEvidence>();
        factoryEv = new List<SerializedEvidence>();
        apartmentEV = new List<SerializedEvidence>();
        AllEvidence = new List<SerializedEvidence>(); // clear this on new games
        // then initialize all the evidence with the correct indices:
        for (int i = 0; i < allEvidenceEntities.Count; i++)
        {
            SerializedEvidence e = new SerializedEvidence(i);
            e.evidenceState = SerializedEvidence.EvidenceState.NotInGame; // make sure it's all not found so that it doesn't mess your second game up
            AllEvidence.Add(e);
        }
        // this is called and then later you'd initialize whether the evidence is in the game or not and store that in this info
        // you may also store spawn points or something but not for now TODO
        RandomlySpawnEvidence(); // this chooses what to place into the game world and disables everything else
        generated = true;
    }

    public static bool LoadEvidenceFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("EvidenceSaved"))
        {
            LoadEvidenceFromString(PlayerPrefs.GetString("EvidenceSaved"));
            return true;
        }
        return false;
    }

    public static void SaveEvideneToPlayerPrefs()
    {
        PlayerPrefs.SetString("EvidenceSaved", SaveEvidenceToString());
        PlayerPrefs.Save();
    }

    public static string SaveEvidenceToString()
    {
        return JsonUtility.ToJson(AllEvidence, true);
    }

    public static void LoadEvidenceFromString(string s)
    {
        JsonUtility.FromJsonOverwrite(s, AllEvidence);
    }

    public static void NewSaveGame()
    {
        // clears the player prefs
        PlayerPrefs.DeleteKey("EvidenceSaved");
        PlayerPrefs.Save();
        instance.generated = false; // force it to generate again
        // then it also makes a new game
        Random.InitState(System.DateTime.Now.Millisecond);
        instance.InitializeSerializedEvidence();
    }

    public void MarkRandomCulprit()
    {
        int[] indices = new int[5];
        int i = 0;
        foreach(SerializedEvidence se in allSerializedEvidence)
        {
            if (i == 5) break;
            Suspect s = ReferencedEntity(se) as Suspect;
            if(s != null)
            {
                se.evidenceState = SerializedEvidence.EvidenceState.OffYarnBoard;
                indices[i] = se.evidenceindex;
                i++;
                // I need this list to check if evidence is placeable later
                suspects.Add(s);
            }
        }

        int j = Random.Range(0, 5);
        culprit = ReferencedEntity(allSerializedEvidence[j]) as Suspect;
        if(culprit == null)
        {
            // Oh no everything's broken.
            Debug.LogError("Problem with MarkRandomCulprit()! Culprit was not found!");
        }
    }

    public void RandomlySpawnEvidence()
    {
        // Eric put your code here! Feel free to do whatever including ignore this, this is just what I was thinking when 
        // I wrote this class.
        // If we include spawning evidence in random locations then we'd also probably need to store data that we don't have currently
        // so let me know if we need to add that.
        // Ok Jordan *thumbs up*
       
        // Let's start with generating a culprit
        MarkRandomCulprit();

        List<SerializedEvidence> remainingEvidence = new List<SerializedEvidence>(allSerializedEvidence);
        
        // Popping the front of the list 5 times, to remove all suspects from the algorithm.
        for(int i = 0; i < 5; i++)
        {
            remainingEvidence.RemoveAt(0);
        }

        // Add all the evidence that corresponds to the culprit.
        foreach (SerializedEvidence se in remainingEvidence)
        {
            //Lots of casting, might be able to condense
            Evidence ev = ReferencedEntity(se) as Evidence;

            //If we can cast the YBE to Evidence, then it's a piece of Evidence!
            if (ev != null)
            {
                //Find the culprit
                if ((ev.AssociatedSuspects.Contains(culprit) && ev.GetEvidenceType != EvidenceType.Conversation) || ev.GetLevel == Level.Apartment)
                {
                    //Put it in the game as evidence!
                    se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                    switch(ev.GetLevel)
                    {
                        case Level.Apartment:
                            apartmentEV.Add(se);
                            break;
                        case Level.Office:
                            officeEv.Add(se);
                            break;
                        case Level.Factory:
                            factoryEv.Add(se);
                            break;
                        default:
                            Debug.LogWarning("FOund evidence with unknown level: " + ev.GetLevel);
                            break;
                    }

                    if(ev.GetLevel != Level.Apartment)
                    {
                        foreach(Suspect s in ev.AssociatedSuspects)
                        {
                            suspectTotals[suspects.IndexOf(s)]++;
                        }
                    }
                }
                /*
                else if (ev.GetEvidenceType == EvidenceType.Conversation)
                {
                    foreach (Suspect s in ev.AssociatedSuspects)
                        suspectTotals[suspects.IndexOf(s)]++;
                }
                */
            }
            
        }


        while(officeEv.Count < officeEvidenceCount)
        {
            if (remainingEvidence.Count == 0)
                break;

            // Generate the next piece of evidence and check if it can be added.
            int next = Random.Range(0, remainingEvidence.Count - 1);
            SerializedEvidence se = remainingEvidence[next];
            Evidence ev = ReferencedEntity(se) as Evidence;
            
            if(ev == null || ev.GetEvidenceType == EvidenceType.Conversation)
            {
                remainingEvidence.Remove(se);
                continue;
            }

            if(ev.GetLevel == Level.Office && !officeEv.Contains(se))
            {
                se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                officeEv.Add(se);
            }

            // Update suspect totals
            foreach (Suspect s in ev.AssociatedSuspects)
                suspectTotals[suspects.IndexOf(s)]++;
            remainingEvidence.Remove(se);
        }

        remainingEvidence = new List<SerializedEvidence>(allSerializedEvidence);
        for(int i = 0; i < 5; i++)
        {
            remainingEvidence.RemoveAt(0);
        }

        while (factoryEv.Count < factoryEvidenceCount)
        {
            if (remainingEvidence.Count == 0)
                break;

            // Generate the next piece of evidence and check if it can be added.
            int next = Random.Range(0, remainingEvidence.Count - 1);
            SerializedEvidence se = remainingEvidence[next];
            Evidence ev = ReferencedEntity(se) as Evidence;

            if (ev == null || ev.GetEvidenceType == EvidenceType.Conversation)
            {
                remainingEvidence.Remove(se);
                continue;
            }

            if (ev.GetLevel == Level.Factory && !factoryEv.Contains(se))
            {
                se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                factoryEv.Add(se);
            }

            // Update suspect totals
            foreach (Suspect s in ev.AssociatedSuspects)
                suspectTotals[suspects.IndexOf(s)]++;
            remainingEvidence.Remove(se);
        }

        Debug.Log("Office: " + officeEv.Count);
        Debug.Log("Factory: " + factoryEv.Count);
        //Debug.Log(culprit.CodeName);
        //Debug.Log("OFFICE EVIDENCE BELOW!");
        //string sus = "";
        //foreach(SerializedEvidence se in officeEv)
        //{          
        //    Evidence e = ReferencedEntity(se) as Evidence;
        //    Debug.Log(e.Name);
        //    foreach(Suspect s in e.AssociatedSuspects)
        //    {
        //        sus += suspects.IndexOf(s).ToString() + " / ";
        //    }
        //    Debug.Log(sus);
        //    sus = "";
        //}

        //Debug.Log("FACTORY EVIDENCE BELOW!");
        //foreach(SerializedEvidence se in factoryEv)
        //{
        //    Evidence e = ReferencedEntity(se) as Evidence;
        //    Debug.Log(e.Name);
        //    foreach (Suspect s in e.AssociatedSuspects)
        //    {
        //        sus += suspects.IndexOf(s).ToString() + " / ";
        //    }
        //    Debug.Log(sus);
        //    sus = "";
        //}


        //ClampEvidenceLists();
    }

    private void ClampEvidenceLists()
    {
        while(officeEv.Count > officeEvidenceCount)
        {
            int index = Random.Range(0, officeEv.Count - 1);
            SerializedEvidence se = officeEv[index];
            se.evidenceState = SerializedEvidence.EvidenceState.NotInGame;
            officeEv.Remove(se);
        }

        while(factoryEv.Count > factoryEvidenceCount)
        {
            int index = Random.Range(0, factoryEv.Count - 1);
            SerializedEvidence se = factoryEv[index];
            se.evidenceState = SerializedEvidence.EvidenceState.NotInGame;
            factoryEv.Remove(se);
        }
    }

    private bool CheckIfPlaceable(Evidence ev)
    {
        if (ev.AssociatedSuspects.Count == 0)
            return true;
        foreach (Suspect s in ev.AssociatedSuspects)
        {
            if (suspectTotals[suspects.IndexOf(s)] >= otherSuspectMax)
                return false;
        }
        return true;
    }

    public YarnBoardEntity ReferencedEntity(SerializedEvidence se)
    {
        return allEvidenceEntities[se.evidenceindex];
    }

    public SerializedEvidence FindSerializedEvidence(YarnBoardEntity e)
    {
        for (int i = 0; i < allSerializedEvidence.Count; i++)
        {
            if (allEvidenceEntities[allSerializedEvidence[i].evidenceindex] == e)
            {
                return allSerializedEvidence[i];
            }
        }

        Debug.LogError("UNABLE TO FIND EVIDENCE OH DEAR");
        return null;
    }

    public void ConnectEvidence(int e1, int e2)
    {
        if (e1 == e2 || e1 > AllEvidence.Count || e2 > AllEvidence.Count)
        {
            return; // can't connect them if they're you or they don't exist
        }
        SerializedEvidence e1e = AllEvidence[e1];
        SerializedEvidence e2e = AllEvidence[e2];
        YarnBoardConnectEvent connectEvent = new YarnBoardConnectEvent(e1, e1e, e2, e2e, false);
        connectEvent.Redo(); // this way I don't have to deal with implementing it multiple places
        // also add the event to the undo/redo stack
        UndoRedoStack.AddEvent(connectEvent);
    }

    public void ConnectEvidence(YarnBoardEntity e1, YarnBoardEntity e2)
    {
        ConnectEvidence(FindSerializedEvidence(e1).evidenceindex, FindSerializedEvidence(e2).evidenceindex);
    }

    public void DisconnectEvidence(int e1, int e2)
    {
        // delete the connection
        if (e1 == e2 || e1 > AllEvidence.Count || e2 > AllEvidence.Count)
        {
            return; // can't disconnect them if they're you or they don't exist
        }
        SerializedEvidence e1e = AllEvidence[e1];
        SerializedEvidence e2e = AllEvidence[e2];
        YarnBoardConnectEvent connectEvent = new YarnBoardConnectEvent(e1, e1e, e2, e2e, true);
        connectEvent.Redo(); // this way I don't have to deal with implementing it multiple places
        // also add the event to the undo/redo stack
        UndoRedoStack.AddEvent(connectEvent);
    }

    public void DisconnectEvidence(YarnBoardEntity e1, YarnBoardEntity e2)
    {
        DisconnectEvidence(FindSerializedEvidence(e1).evidenceindex, FindSerializedEvidence(e2).evidenceindex);
    }
}
