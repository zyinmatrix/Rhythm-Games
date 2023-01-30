using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmSyncingIdeas : MonoBehaviour
{

    //double timeOfNextNote;

    double timeToPlayParticles;

    public ParticleSystem particles;

    bool particlesToPlay = false;

    public AudioSource soundToPlay;

    
    //Synchronizing Animations to the beat
    private void Update()
    {
        //if (AudioSettings.dspTime >= timeOfNextNote)
        //{
            //trigger our animation
            //update timeOfNextNote
            //timeOfNextNote += Beat.Clock.Instance.BeatLengthD();

        //}

        if(AudioSettings.dspTime >= timeToPlayParticles && particlesToPlay)
        {
            particles.Play();
            particlesToPlay = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayParticlesOnNextEighth();
        }


    }

    public void PlayParticlesOnNextEighth()
    {
        timeToPlayParticles = Beat.Clock.Instance.AtNextQuarter();
        soundToPlay.PlayScheduled(timeToPlayParticles);
        particlesToPlay = true;
    }
}
