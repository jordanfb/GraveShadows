using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text playGameText;


    public void Update()
    {
        // this is just here because it loads slowly I guess... this works, but FIX
        if (Options.instance.demoMode)
        {
            playGameText.text = "Play Demo";
        }
    }

    public void Start()
    {
        // search for and destroy the custom level one stuff if they exist
        foreach (CustomLevel1Logic logic in GameObject.FindObjectsOfType<CustomLevel1Logic>())
        {
            Destroy(logic.gameObject);
        }
        foreach (LevelOneEvidenceManager evidenceManager in GameObject.FindObjectsOfType<LevelOneEvidenceManager>())
        {
            Destroy(evidenceManager.gameObject);
        }
    }


    public void NewGame()
    {
        if (Options.instance.demoMode)
        {
            GameplayManager.instance.NewDemoGame();
            if (Options.instance.demoModeEnableTutorial) {
                GameplayManager.instance.VisitCrimeScene();
            }
            else
            {
                GameplayManager.instance.VisitDemoOffice();
            }
        }
        else
        {
            // normal game
            GameplayManager.instance.NewGame();
            GameplayManager.instance.VisitCrimeScene();
        }
    }

    public void LoadGame()
    {
        GameplayManager.instance.LoadGameFromPlayerPrefs();
        Debug.LogWarning("This isn't implemented and may never be");
        GameplayManager.instance.VisitCrimeScene();
    }

    public void QuitGame()
    {
        //Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OptionsMenu()
    {
        GameplayManager.instance.VisitOptions();
    }

    public void CreditsMenu()
    {
        GameplayManager.instance.VisitCreditsScene();
    }

    public void ControlsMenu()
    {
        GameplayManager.instance.VisitControls();
    }
}
