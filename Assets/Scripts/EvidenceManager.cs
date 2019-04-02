using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager instance; // monobehavior

    public List<YarnBoardEntity> allEvidenceEntities;

    private List<SerializedEvidence> allSerializedEvidence = new List<SerializedEvidence>(); // this is private so that it doesn't get saved. It's initialized either from save data or by code
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

    public void RandomlySpawnEvidence()
    {
        // Eric put your code here! Feel free to do whatever including ignore this, this is just what I was thinking when 
        // I wrote this class.
        // If we include spawning evidence in random locations then we'd also probably need to store data that we don't have currently
        // so let me know if we need to add that.

        // TODO

        for (int i = 0; i < allSerializedEvidence.Count; i++)
        {
            // for now just randomize it entirely
            allSerializedEvidence[i].evidenceState = Random.Range(0, 1) < .5f ? SerializedEvidence.EvidenceState.NotInGame : SerializedEvidence.EvidenceState.OffYarnBoard;
        }
    }
}
