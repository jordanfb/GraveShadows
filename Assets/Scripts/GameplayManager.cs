using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{

    /*
     * This keeps track of the days passed, and what was found each day, and whatever more needs to be done to make
     * the game function.
     * 
     */
    
    EvidenceManager evidenceManager;
    
    public static GameplayManager instance;

    [Space]

    [Tooltip("Can you miss the last day if you get caught on the second to last?")]
    public bool allowSkippingLastDay = true;
    // gameplay data:
    public const int numExploringDays = 5; // the number of days to explore levels
    public int dayNum = 0; // 0-4 are days when you can do stuff
    // day 5 is when you have to choose the evidence

    public bool debugMode = false;

    [Tooltip("this gets set by the script don't edit it in the editor")]
    public List<DayData> dayData;

    private string debugModeString = "zasegj";
    private bool[] debugModeKeysPressed = new bool[6];

    private void Awake()
    {
#if UNITY_EDITOR
        // make it debug mode
        debugMode = true;
#endif
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame(); // for now just create a new game
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            debugModeKeysPressed[0] = true;
            CheckToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            debugModeKeysPressed[1] = true;
            CheckToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            debugModeKeysPressed[2] = true;
            CheckToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            debugModeKeysPressed[3] = true;
            CheckToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            debugModeKeysPressed[4] = true;
            CheckToggleDebug();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            debugModeKeysPressed[5] = true;
            CheckToggleDebug();
        }
    }

    private void CheckToggleDebug()
    {
        if (debugModeKeysPressed[0] && debugModeKeysPressed[1] && debugModeKeysPressed[2] && debugModeKeysPressed[3] && debugModeKeysPressed[4] && debugModeKeysPressed[5])
        {
            debugMode = !debugMode;
            debugModeKeysPressed = new bool[6];
        }
    }

    public void ExitBackToHubNextDay()
    {
        GameLevelManager gameLevel = FindObjectOfType<GameLevelManager>();
        Debug.Assert(gameLevel != null); // duh it can't be null we need it in all our levels
        GameplayManager.instance.NextDay(GameplayManager.instance.GenerateTodaysRecipt(gameLevel.level, gameLevel.evidenceFoundThisDay, false, gameLevel.HasFoundEverything()));
        VisitHubScene();
    }

    private void StartFactoryScene()
    {
        SceneManager.LoadScene("Level 3");
    }

    private void StartHubScene()
    {
        SceneManager.LoadScene("Level 0 HUB");
    }

    private void StartOptions()
    {
        SceneManager.LoadScene("Options");
    }

    private void StartCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    private void StartOfficeScene()
    {
        SceneManager.LoadScene("Level 2");
    }

    private void StartMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuNew");
    }

    private void StartCrimeScene()
    {
        SceneManager.LoadScene("Level 1");
    }

    public string GenerateTodaysRecipt(Level visitedLocation, List<Evidence> evidenceFound, bool wasSpotted, bool foundAll)
    {
        string visitedLocationString = "";
        switch (visitedLocation)
        {
            case Level.Apartment:
                visitedLocationString = "The Apartment";
                break;
            case Level.Factory:
                visitedLocationString = "The Factory";
                break;
            case Level.Hub:
                visitedLocationString = "My Office";
                break;
            case Level.Office:
                visitedLocationString = "The Office";
                break;
        }
        string s = "<u>Visited:</u>\n" + visitedLocationString + "\n<u>Found:</u>\n";
        if (evidenceFound.Count == 0)
        {
            s += "Nothing\n";
        } else
        {
            // loop through the evidence found and print their names
            for (int i = 0; i < evidenceFound.Count; i++)
            {
                s += evidenceFound[i].Name + "\n";
            }
        }

        if (wasSpotted)
        {
            s += "\nThey caught me. Damn.";
        }
        if (foundAll)
        {
            s += "\nI think I found everything.";
        }
        return s;
    }

    public void VisitFactory()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartFactoryScene();
        } else
        {
            f.FadeOut(StartFactoryScene);
        }
    }

    public void FadeOut(System.Action a)
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            a.Invoke();
        }
        else
        {
            f.FadeOut(a);
        }
    }

    public void VisitOffice()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartOfficeScene();
        }
        else
        {
            f.FadeOut(StartOfficeScene);
        }
    }

    public void VisitCrimeScene()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartCrimeScene();
        }
        else
        {
            f.FadeOut(StartCrimeScene);
        }
    }

    public void VisitScene(string sceneName)
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            f.FadeOut(() => { SceneManager.LoadScene(sceneName); });
        }
    }

    public void VisitHubScene()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartHubScene();
        }
        else
        {
            f.FadeOut(StartHubScene);
        }
    }

    public void VisitMainMenuScene()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartMainMenuScene();
        }
        else
        {
            f.FadeOut(StartMainMenuScene, false);
        }
    }

    public void VisitOptions()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartOptions();
        }
        else
        {
            f.FadeOut(StartOptions);
        }
    }
    public void VisitCreditsScene()
    {
        FadeToBlack f = GameObject.FindObjectOfType<FadeToBlack>();
        if (f == null)
        {
            Debug.LogError("NO FADE TO BLACK IN THIS SCENE I REALLY WANT ONE");
            StartCredits();
        }
        else
        {
            f.FadeOut(StartCredits);
        }
    }

    public void SkipDay(string caught_day_summary)
    {
        // pass in the summary of the day you were caught on
        // this fills in the summary for the day you lay low on
        // if you get caught skip a day on top of the regular next day
        if (IsChoosingDay())
        {
            return; // return early you can't go further than this
        }
        if (dayNum == numExploringDays - 1 && !allowSkippingLastDay)
        {
            // it doesn't skip because the boolean says not to skip
            return;
        }
        NextDay(caught_day_summary);
        if (!IsChoosingDay())
        {
            dayData[dayNum].dayContent = "<b>\nLast time I was caught, so this day I had to lay-low</b>";
            dayNum++; // increase an extra day
        }
    }

    public void NextDay(string summary)
    {
        if (IsChoosingDay())
        {
            return; // return early you can't go further than this
        }
        dayData[dayNum].dayContent = summary;
        dayNum++;
    }

    public bool IsChoosingDay()
    {
        return dayNum >= numExploringDays;
    }

    public void NewGame()
    {
        // starts a new game!
        // chooses the evidence and whatever
        EvidenceManager.NewSaveGame();
        UndoRedoStack.Reset(); // make sure to reset the undo redo stack otherwise weird bugs
        PlayerManager.instance.NewGame(); // reset the found evidence
        // then load the game data here:
        dayNum = 0; // first day
        dayData = new List<DayData>();
        for (int i = 0; i < numExploringDays; i++)
        {
            dayData.Add(new DayData());
        }
    }

    public void LoadGameFromPlayerPrefs()
    {
        EvidenceManager.LoadEvidenceFromPlayerPrefs();
        LoadGame();
    }

    public void LoadGame()
    {
        // it loads the evidence data then loads whatever else in this function
    }
}


[System.Serializable]
public class DayData
{
    public DayData()
    {
        dayContent = "";
        wasSkipped = false;
    }
    public DayData(string content)
    {
        dayContent = content;
        wasSkipped = false;
    }
    public DayData(string content, bool skipped)
    {
        dayContent = content;
        wasSkipped = skipped;
    }
    public string dayContent;
    public bool wasSkipped;
}
