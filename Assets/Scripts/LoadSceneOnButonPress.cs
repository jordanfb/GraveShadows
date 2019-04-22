using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnButonPress : MonoBehaviour
{
    public KeyCode key;
    public string scene;

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.debugMode && Input.GetKeyDown(key))
        {
            SceneManager.LoadScene(scene);
        }
    }

    public void LoadSceneAfterTime(float t)
    {
        StartCoroutine(LoadSceneCoroutine(t));
    }

    private IEnumerator LoadSceneCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(scene);
    }
}
