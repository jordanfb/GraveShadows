using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public static Options instance;

    public bool demoModeByDefault = false;

    public bool InvertY = false;
    public bool playMusic = true;
    public bool invertYarnboardPanning = false;
    public bool demoMode = false; // only allow the demo mode if so
    public bool demoModeEnableTutorial = false;
    public bool useEvidenceFromAllLevels = true; // this is to try the new evidence approach which takes evidence from all levels.

    [Space]
    public AudioClip musicClipInBackgroundToPlayPause;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadPlayerPrefs();
    }

    public void SaveOptionsToPlayerPrefs()
    {
        PlayerPrefs.SetInt("OptionsInvertMouseY", InvertY ? 1 : 0);
        PlayerPrefs.SetInt("OptionsInvertYarnboardPanning", invertYarnboardPanning ? 1 : 0);
        PlayerPrefs.SetInt("OptionsPlayMusic", playMusic ? 1 : 0);
        PlayerPrefs.SetInt("OptionsDemoMode", demoMode ? 1 : 0);
        PlayerPrefs.SetInt("OptionsDemoModeTutorial", demoModeEnableTutorial ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void EnableOrDisableMusic()
    {
        // then set the music I guess
        AudioSource[] music = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < music.Length; i++)
        {
            if (music[i].clip == musicClipInBackgroundToPlayPause)
            {
                music[i].enabled = playMusic;
            }
        }
    }

    [ContextMenu("Clear Saved Settings")]
    public void ClearPlayerprefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void LoadPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("OptionsPlayMusic"))
        {
            InvertY = PlayerPrefs.GetInt("OptionsInvertMouseY") == 1;
        }
        else
        {
            InvertY = false;
        }
        if (PlayerPrefs.HasKey("OptionsPlayMusic"))
        {
            invertYarnboardPanning = PlayerPrefs.GetInt("OptionsInvertYarnboardPanning") == 1;
        }
        else
        {
            invertYarnboardPanning = false;
        }
        if (PlayerPrefs.HasKey("OptionsPlayMusic"))
        {
            playMusic = PlayerPrefs.GetInt("OptionsPlayMusic") == 1;
        }
        else
        {
            playMusic = true;
        }
        if (PlayerPrefs.HasKey("OptionsDemoMode"))
        {
            demoMode = PlayerPrefs.GetInt("OptionsDemoMode") == 1;
        }
        else
        {
            demoMode = demoModeByDefault; // default to whatever the setting is game
        }
        if (PlayerPrefs.HasKey("OptionsDemoModeTutorial"))
        {
            demoModeEnableTutorial = PlayerPrefs.GetInt("OptionsDemoModeTutorial") == 1;
        }
        else
        {
            demoModeEnableTutorial = true; // default to normal game
        }

        EnableOrDisableMusic();
    }
}
