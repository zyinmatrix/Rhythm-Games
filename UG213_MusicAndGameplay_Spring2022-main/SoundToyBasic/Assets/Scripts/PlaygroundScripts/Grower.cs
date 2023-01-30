using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE
/// THE SYNTHS HERE ARE BY ANDREW BENSON (PIXLPA)
/// DOCUMENTATION ON THESE IS AVAILABLE AT https://github.com/pixlpa/Unity-Synth-Experiments
/// </summary>

public class Grower : MonoBehaviour
{
    
    public enum GrowerType { sample, synth }
    public GrowerType growerType = GrowerType.sample;

    public AudioSource aSource;

    [Header("only required when using synth")]
    public pxStrax subtractiveSynth;
    public float minSynthParam = 100;
    public float maxSynthParam = 12000;
    public AnimationCurve mappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);


    float lifespan;

    bool isAlive;

    public bool isGrowing;

    [Header("assigned at runtime")]
    public PositionToPitch posToPitch;
    public Vector3 startScale;

    public float maxScale;
    public float maxTime;


    // Start is called before the first frame update
    void Start()
    {

        //just mapping pitch for now, will switch to synth mode
        if (growerType == GrowerType.sample)
        {
            aSource.pitch = posToPitch.PitchMapping(transform.position);
        }
        else
        {
            subtractiveSynth.sustain = true;
            posToPitch.sendMidi = true;
            subtractiveSynth.KeyOn(posToPitch.PitchMapping(transform.position));

            Debug.Log("synth pitch: " + posToPitch.PitchMapping(transform.position));
        }

    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        isAlive = false;
    }

    public void PopGrower()
    {
        StopAllCoroutines();
        isAlive = false;
        aSource.Stop();
        subtractiveSynth.KeyOff();
        Destroy(gameObject);
    }

    public void Grow()
    {
        if (lifespan < maxTime)
        {
            lifespan += Time.deltaTime;

            transform.localScale = startScale + (startScale * (lifespan / maxTime) * maxScale);

            //change this to be mapped to other parameters
            aSource.volume = MathUtil.Map(lifespan, 0f, maxTime, 0f, 1f);

            if (growerType == GrowerType.synth)
            {
                //we're mapping both the volume and our synth parameter (cutoff frequency by default)
                //to the lifetime
                MapSynthParameterToTime(lifespan);
                subtractiveSynth.volume = MathUtil.Map(lifespan, 0f, maxTime, 0f, 1f);
            }
        }
    }

    public void StopGrowing ()
    {
        isGrowing = false;
        isAlive = true;

        StartCoroutine(GrowShrink());
            
    }

    private void OnMouseDown()
    {
        
    }

    public IEnumerator GrowShrink()
    {
        while (isAlive)
        {
            if(isGrowing)
            {
                float growTime = 0;
                while (growTime < lifespan)
                {
                    growTime += Time.deltaTime;

                    transform.localScale = startScale + (startScale * (growTime / maxTime) * maxScale);

                    //change this to be mapped to other parameters
                    aSource.volume = MathUtil.Map(growTime, 0f, maxTime, 0f, 1f);

                    if (growerType == GrowerType.synth)
                    {
                        MapSynthParameterToTime(growTime);
                        subtractiveSynth.volume = MathUtil.Map(growTime, 0f, maxTime, 0f, 1f);
                    }

                    yield return null;
                }
                isGrowing = false;
            }

            else
            {

                //shrink twice as fast as growing, then wait for a bit and grow again
                float shrinkTime = lifespan;
                while (shrinkTime > 0f)
                {
                    shrinkTime -= Time.deltaTime * 2f;

                    transform.localScale = startScale * (shrinkTime / maxTime) * maxScale;

                    //will change this later so it can be mapped to other parameters
                    aSource.volume = MathUtil.Map(shrinkTime, 0f, maxTime, 0f, 1f);

                    if (growerType == GrowerType.synth)
                    {
                        MapSynthParameterToTime(shrinkTime);
                        subtractiveSynth.volume = MathUtil.Map(shrinkTime, 0f, maxTime, 0f, 1f);
                    }

                    yield return null;
                }
                yield return new WaitForSeconds(lifespan / 2f);
                isGrowing = true;
            }
        }
    }

    /* 
     * GOOD SPOT TO CHANGE THINGS! use different synth parameters! 
     * pxFemme and pxSnarple have different parameters to play with as well!
     * 
     */

    public void MapSynthParameterToTime(float input)
    {
        float tempValue = MathUtil.Map(input, 0f, maxTime, minSynthParam, maxSynthParam);

        //same procedure as in Position to pitch;

        float paramValue = MathUtil.ScaleToAnimCurve(tempValue, minSynthParam, maxSynthParam, mappingCurve);

        //could replace this with other synth parameters
        subtractiveSynth.osc2Mix = paramValue;
    }

}
