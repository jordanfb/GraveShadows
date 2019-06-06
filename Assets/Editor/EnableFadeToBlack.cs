using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EnableFadeToBlack : EditorWindow
{
    //UnityEngine.SceneManagement.Scene[] scenes = new UnityEngine.SceneManagement.Scene[UnityEngine.SceneManagement.c;
    //public static void EnableFadeToBlackObjects()
    //{
    //    for scene in UnityEditor.SceneManagement.
    //}
    //UnityEditor.EditorBuildSettings.
    //foreach (UnityEditor.EditorBuildSettingsScene S in scenes EditorBuildSettings.scenes) {
    //    }

    [MenuItem("Utilites/Enable Fade To Black")]
    public static void EnableFadeToBlackObjects()
    {
        EnableOrDisableFadeToBlackObjects(true);
    }

    [MenuItem("Utilites/Disable Fade To Black")]
    public static void DisableFadeToBlackObjects()
    {
        EnableOrDisableFadeToBlackObjects(false, false);
    }

    public static bool RecursiveObjectSearch<T>(GameObject go, out T t)
    {
        t = go.GetComponent<T>();
        if (t != null)
        {
            return true;
        }
        for (int i = 0; i < go.transform.childCount; i++)
        {
            bool found = RecursiveObjectSearch<T>(go.transform.GetChild(i).gameObject, out t);
            if (found)
                return true;
        }
        return false;
    }

    public static void EnableOrDisableFadeToBlackObjects(bool enable = true, bool recursively = true)
    {
        int num_enabled = 0;
        int num_total = 0;
        int num_without = 0;
        foreach (UnityEditor.EditorBuildSettingsScene editorBuildScene in EditorBuildSettings.scenes)
        {
            if (editorBuildScene.enabled)
            {
                UnityEngine.SceneManagement.Scene s = EditorSceneManager.OpenScene(editorBuildScene.path);
                FadeToBlack fade = GameObject.FindObjectOfType<FadeToBlack>();
                if (fade == null)
                {
                    // search for disabled game objects
                    List<GameObject> rootObjects = new List<GameObject>();
                    s.GetRootGameObjects(rootObjects);
                    foreach (GameObject go in rootObjects)
                    {
                        // search recursively for fade to black!
                        FadeToBlack f = go.GetComponentInChildren<FadeToBlack>();
                        if (f != null)
                        {
                            fade = f;
                            break;
                        }
                        if (f == null)
                        {
                            bool found = RecursiveObjectSearch<FadeToBlack>(go, out f);
                            if (found)
                            {
                                Debug.Log("found it using recursive search");
                                fade = f;
                                break;
                            }
                        }
                    }
                }
                if (fade != null)
                {
                    if ((fade.gameObject.activeInHierarchy == enable) && (fade.enabled == enable))
                    {
                        num_enabled++;
                    }
                    else
                    {
                        // need to recursively activate it I guess?
                        fade.enabled = true; // always have the fade to black object be enabled, just disable its game object
                        //fade.gameObject.SetActive(true);
                        Transform go = fade.transform;
                        do
                        {
                            go.gameObject.SetActive(enable);
                            go = go.parent;
                        }
                        while (go != null && recursively) ;
                    }
                    EditorSceneManager.SaveScene(s, editorBuildScene.path);
                }
                else
                {
                    num_without++;
                }
                num_total++;
            }
        }
        //EditorSceneManager.SaveScenes();

        string enable_text = "Iterated over " + num_total + " scenes. " + (enable ? "enabled " : "disabled ") + (num_total - num_without - num_enabled);
        if (num_total - num_without - num_enabled == 0 && enable)
        {
            enable_text += " Good Job!";
        }
        if (num_without > 0)
        {
            enable_text += "\nMissing fade to black objects in " + num_without + " scenes";
        }
        Debug.Log(enable_text);
    }
}
