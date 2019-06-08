using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip stepSoundEffect;
    public AudioClip warpSoundEffect;
    public AudioClip copMurmerSoundEffect;
    public GameObject copContainer;
    public GameObject player;
    public float minCopConvoDistance = 0;
    public float maxCopConvoDistance = 4f;

    private AudioSource stepSource;
    private AudioSource warpSource;

    void Start()
    {
        setUpCopSoundEffects();
        setupFootstepSounds();
        setupWarpSounds();
    }

    // Update is called once per frame
    void Update()
    {
        updateFootstepSounds();
        updateWarpSound();
    }
    void setUpAudioSourceFor3D(AudioSource s, float minDistance, float maxDistance) {
        s.spatialBlend = 1f;
        s.rolloffMode = AudioRolloffMode.Linear;
        s.minDistance = minDistance;
        s.maxDistance = maxDistance;

    }

    //All cops are bastards

    void setUpCopSoundEffects() { 
        for(int i =0; i< copContainer.transform.childCount; i++) {
            AudioSource newCopAS = copContainer.transform.GetChild(i).gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            newCopAS.clip = copMurmerSoundEffect;
            setUpAudioSourceFor3D(newCopAS, minCopConvoDistance, maxCopConvoDistance);
            newCopAS.loop = true;
            newCopAS.Play();

        }
    }

    void setupFootstepSounds() {
        stepSource = player.AddComponent(typeof(AudioSource)) as AudioSource;
        stepSource.clip = stepSoundEffect;
        stepSource.loop = true;

    }


    void setupWarpSounds()
    {
        warpSource = player.AddComponent(typeof(AudioSource)) as AudioSource;
        warpSource.clip = warpSoundEffect;
        warpSource.loop = false;

    }

    void updateFootstepSounds()
    {
        if (player.GetComponent<Rigidbody>().velocity.magnitude > 0.1) {
            if (!stepSource.isPlaying) {
                stepSource.Play();

            }
            if (player.GetComponent<simplePlayerMovement>().isSprinting)
            {
                stepSource.pitch = 2f;
            }
            else
            {
                stepSource.pitch = 1f;
            }

        }
        else {
            stepSource.Stop();
        }
    }
    bool lastFrameIsInshadowRealm = false;
    void updateWarpSound() {

        if(lastFrameIsInshadowRealm != player.GetComponent<ShadowRealmManager>().isInShadowRealm) {
            warpSource.Play();
        }
        lastFrameIsInshadowRealm = player.GetComponent<ShadowRealmManager>().isInShadowRealm;
    
    }
}
