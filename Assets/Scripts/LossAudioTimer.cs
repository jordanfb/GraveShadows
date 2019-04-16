using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossAudioTimer : MonoBehaviour
{
    public float delay;
    [HideInInspector]
    public AudioSource myAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(delay);
        myAudioSource.Play();
    }
}
