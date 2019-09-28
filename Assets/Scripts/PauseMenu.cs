using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public OptionsMenu optionsMenu;
    public GameObject[] optionsMenuUI;

    private float timescale = 0;
    private float dft = 0;
    private CursorLockMode lockMode;
    private bool visible;

    private void OnEnable()
    {
        timescale = Time.timeScale;
        dft = Time.fixedDeltaTime;
        lockMode = Cursor.lockState;
        visible = Cursor.visible;
        // freeze time
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;
        Cursor.lockState = CursorLockMode.None; // free the mouse to press the buttons!
        Cursor.visible = true;
        optionsMenu.gameObject.SetActive(false); // hide the options menu
    }

    private void OnDisable()
    {
        Time.fixedDeltaTime = dft;
        Time.timeScale = timescale;
        Cursor.lockState = lockMode;
        Cursor.visible = visible;
        optionsMenu.gameObject.SetActive(false); // hide the options menu
    }

    public void LoadOptionsMenu()
    {
        // enable the options menu and hide the pause menu items
        optionsMenu.gameObject.SetActive(true);
        foreach(GameObject g in optionsMenuUI)
        {
            g.SetActive(false);
        }
    }

    public void DisableOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(false);
        foreach (GameObject g in optionsMenuUI)
        {
            g.SetActive(true);
        }
    }

    public void ReturnToGame()
    {
        // unpause time and disable ourselves!
        gameObject.SetActive(false); // which then calls on disable, which then unpauses time!
    }

    public void ReturnToMenu()
    {
        GameplayManager.instance.VisitMainMenuScene();
    }
}
