using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        GameplayManager.instance.NewGame();
        GameplayManager.instance.VisitCrimeScene();
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

    public void Options()
    {
        GameplayManager.instance.VisitOptions();
    }

    public void Credits()
    {
        GameplayManager.instance.VisitCreditsScene();
    }
}
