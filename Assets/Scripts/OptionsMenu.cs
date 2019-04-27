using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle playMusicToggle;
    public Toggle yInvertedToggle;
    public Toggle invertYarnboardPanning;
    public Toggle demoMode;
    public Toggle demoModeTutorial;
    public Text mouseSensitivity;

    private void Start()
    {
        playMusicToggle.isOn = Options.instance.playMusic;
        yInvertedToggle.isOn = Options.instance.InvertY;
        invertYarnboardPanning.isOn = Options.instance.invertYarnboardPanning;
        demoMode.isOn = Options.instance.demoMode;
        demoModeTutorial.isOn = Options.instance.demoModeEnableTutorial;
    }

    public void PlayMusicTogglePressed(bool value)
    {
        Options.instance.playMusic = value;
        Options.instance.EnableOrDisableMusic();
        Options.instance.SaveOptionsToPlayerPrefs();
    }

    public void DemoModeTogglePressed(bool value)
    {
        Options.instance.demoMode = value;
        Options.instance.SaveOptionsToPlayerPrefs();
    }

    public void DemoModeTutorialTogglePressed(bool value)
    {
        Options.instance.demoModeEnableTutorial = value;
        Options.instance.SaveOptionsToPlayerPrefs();
    }

    public void YInvertedTogglePressed(bool value)
    {
        Options.instance.InvertY = value;
        Options.instance.SaveOptionsToPlayerPrefs();
    }

    public void InvertYarnboardTogglePressed(bool value)
    {
        Options.instance.invertYarnboardPanning = value;
        Options.instance.SaveOptionsToPlayerPrefs();
    }

    public void ReturnToMainMenu()
    {
        GameplayManager.instance.VisitMainMenuScene();
        Options.instance.SaveOptionsToPlayerPrefs();
    }
}
