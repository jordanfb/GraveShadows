using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
    public RawImage image;
    public float startingAlpha = 1; // what alpha to start at
    public float speed = 1; // 1 second I guess

    private float alpha;

    Coroutine runningCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        alpha = startingAlpha;
        SetAlpha();
        FadeIn(); // if it's already transparent no problem it'll exit
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetAlpha()
    {
        Color c = image.color;
        c.a = Smootherstep(alpha);
        image.color = c;
    }

    public void FadeToScene(string sceneName)
    {
        FadeOut(() => { SceneManager.LoadScene(sceneName); });
    }

    [ContextMenu("Fade Out")]
    public void FadeOut(System.Action callback = null)
    {
        // fade to black
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(Fade(speed, callback));
    }

    public float Smootherstep(float x)
    {
        // adapted from wikipedia
        return x * x * x * (x * (x * 6 - 15) + 10);
    }

    public float GetAlpha()
    {
        return alpha;
    }

    [ContextMenu("Fade In")]
    public void FadeIn(System.Action callback = null)
    {
        // fade to transparent
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(Fade(-speed, callback));
    }

    private IEnumerator Fade(float speed, System.Action callback = null)
    {
        alpha += speed * Time.deltaTime;
        SetAlpha();
        while (alpha > 0 && alpha < 1)
        {
            yield return null;
            alpha += speed * Time.deltaTime;
            SetAlpha();
        }
        alpha = Mathf.Clamp01(alpha);
        SetAlpha();
        runningCoroutine = null;
        if (callback != null)
        {
            callback(); // run the callback!
        }
    }
}
