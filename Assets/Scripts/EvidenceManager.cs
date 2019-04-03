using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager instance; // monobehavior
    public int maxEvidence = 30;
    public int otherSuspectMax = 3;
    public List<YarnBoardEntity> allEvidenceEntities;
    public Suspect culprit;
    private List<SerializedEvidence> allSerializedEvidence = new List<SerializedEvidence>(); // this is private so that it doesn't get saved. It's initialized either from save data or by code
    private List<Suspect> suspects = new List<Suspect>();
    [HideInInspector]
    public List<SerializedEvidence> officeEv = new List<SerializedEvidence>();
    [HideInInspector]
    public List<SerializedEvidence> factoryEv = new List<SerializedEvidence>();
    private int[] suspectTotals = new int[5];
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
            InitializeSerializedEvidence();
            
        }
        // we can also just load a new game with pressing a button and calling NewSaveData()
    }

    public bool IsEvidenceInWarehouse(int i)
    {
        return false;
    }

    public void InitializeSerializedEvidence()
    {
        // then initialize all the evidence with the correct indices:
        for (int i = 0; i < allEvidenceEntities.Count; i++)
        {
            SerializedEvidence e = new SerializedEvidence(i);
            AllEvidence.Add(e);
        }
        // this is called and then later you'd initialize whether the evidence is in the game or not and store that in this info
        // you may also store spawn points or something but not for now TODO
        RandomlySpawnEvidence(); // this chooses what to place into the game world and disables everything else
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
        // then it also makes a new game
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

        System.Random rng = new System.Random();
        int j = rng.Next(0, 4);
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
        //These are the amount of evidence currently allocated to the levels
        int office, factory;
        office = factory = 0;

        // Add all the evidence that corresponds to the culprit.
        foreach (SerializedEvidence se in remainingEvidence)
        {
            //Lots of casting, might be able to condense
            Evidence ev = ReferencedEntity(se) as Evidence;

            //If we can cast the YBE to Evidence, then it's a piece of Evidence!
            if(ev != null)
            {
                //Find the culprit
                if(ev.AssociatedSuspects.Contains(culprit))
                {
                    se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                    if (ev.GetLevel == Level.Office)
                    {
                        office++;
                        officeEv.Add(se);
                    }
                    else
                    {
                        factory++;
                        factoryEv.Add(se);
                    }
                    suspectTotals[suspects.IndexOf(culprit)]++;
                }                       
                else if(ev.GetEvidenceType == EvidenceType.Conversation)
                {
                    se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                    if (ev.GetLevel == Level.Office)
                    {
                        office++;
                        officeEv.Add(se);
                    }
                    else
                    {
                        factory++;
                        factoryEv.Add(se);
                    }

                    foreach (Suspect s in ev.AssociatedSuspects)
                        suspectTotals[suspects.IndexOf(s)]++;
                }
                continue;
            }

            Suspect sus = ReferencedEntity(se) as Suspect;
            if(sus != null)
            {
                // Make sure evidence state is set to OffYarnBoard, since we always have the suspect profiles.
                // Remove suspects from the list. We don't want them fucking with the algorithm.
                se.evidenceState = SerializedEvidence.EvidenceState.OffYarnBoard;
            }
            
        }
                
        System.Random rng = new System.Random();
        while(office < 15 || factory < 15)
        {
            if (remainingEvidence.Count == 0)
                break;

            // Generate the next piece of evidence and check if it can be added.
            int next = rng.Next(0, remainingEvidence.Count - 1);
            SerializedEvidence se = remainingEvidence[next];
            Evidence ev = ReferencedEntity(se) as Evidence;
            if(ev == null)
            {
                remainingEvidence.Remove(se);
                continue;
            }

            if (!CheckIfPlaceable(ev))
                continue;

            //Check levels and add to corresponding level.
            switch(ev.GetLevel)
            {
                case Level.Office:
                    if(office < maxEvidence / 2)
                    {
                        // Add it to the game!
                        se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                        office++;
                        officeEv.Add(se);
                    }
                    break;
                case Level.Factory:
                    if(factory < maxEvidence / 2)
                    {
                        // Add it to the game!
                        se.evidenceState = SerializedEvidence.EvidenceState.NotFound;
                        factory++;
                        factoryEv.Add(se);
                    }
                    break;
            }

            // Update suspect totals
            foreach (Suspect s in ev.AssociatedSuspects)
                suspectTotals[suspects.IndexOf(s)]++;
            remainingEvidence.Remove(se);
        }

        int count = 0;
        foreach(SerializedEvidence se in allSerializedEvidence)
        {
            if (se.evidenceState == SerializedEvidence.EvidenceState.NotFound)
                count++;
        }
        Debug.Log(culprit.CodeName);
        Debug.Log(count);
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
}
