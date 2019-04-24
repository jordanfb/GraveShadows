using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle playMusicToggle;
    public Toggle yInvertedToggle;
    public Toggle invertYarnboardPanning;
    public Text mouseSensitivity;

    private void Start()
    {
        playMusicToggle.isOn = Options.instance.playMusic;
        yInvertedToggle.isOn = Options.instance.InvertY;
        invertYarnboardPanning.isOn = Options.instance.invertYarnboardPanning;
    }

    public void PlayMusicTogglePressed(bool value)
    {
        Options.instance.playMusic = value;
        AudioSource[] music = FindObjectsOfType<AudioSource>();
        for (int i =0; i < music.Length; i++)
        {
            music[i].enabled = value; // for now we're just assuming that it's the music one welp.
        }
    }

    public void YInvertedTogglePressed(bool value)
    {
        Options.instance.InvertY = value;
    }

    public void InvertYarnboardTogglePressed(bool value)
    {
        Options.instance.invertYarnboardPanning = value;
    }

    public void ReturnToMainMenu()
    {
        GameplayManager.instance.VisitMainMenuScene();
    }
}
