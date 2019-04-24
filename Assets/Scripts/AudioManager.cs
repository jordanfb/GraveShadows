using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private simplePlayerMovement _player;
    [SerializeField]
    private ShadowRealmManager _srManager;
    [SerializeField]
    private AudioClip[] _clips;
    private AudioSource _audioSource;

    private bool previouslyInShadowRealm = false;
    private bool currentlyInShadowRealm = false;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        currentlyInShadowRealm = _srManager.isInShadowRealm;
        if((Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f) &&
            !_audioSource.isPlaying && !_srManager.isInShadowRealm)
        {
            _audioSource.clip = _clips[0];
            _audioSource.Play();
        }

        if(currentlyInShadowRealm != previouslyInShadowRealm)
        {

            _audioSource.clip = _clips[1];
            _audioSource.volume = 0.1f;
            _audioSource.Play();
        }

        previouslyInShadowRealm = _srManager.isInShadowRealm;
    }
}
