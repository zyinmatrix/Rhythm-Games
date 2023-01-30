using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add to an object with a 2D rigidbody and an AudioSource in order to translate it's speed into sound
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MotionToSound : MonoBehaviour
{
    //NOTE - QuantPitch, as in "Quantized Pitch" is scaling from middle C
    public enum AudioSourceMap { Volume, QuantPitch, ContPitch}

    //uses the unsigned speed (always positive)
    public AudioSourceMap objectSpeedMap;
    public float objectSpeedLow, objectSpeedHigh;

    [Header("Optional - use with Quantized Pitch")]
    public bool useScale = false;
    public MusicalScale musicalScale;

    [Header("mappingLow should be 0 if using Quantized Pitch")]
    [Space(10)]
    [Header("mapping should go from 0 to 1 if using Volume")]
    [Tooltip("if using QuantPitch, best to use a low C note and map from 0 to your highest pitch (in semitones)")]
    public float mappingLow, mappingHigh;

    public float smoothing = 0.1f;

    AudioSource aSource;
    Rigidbody2D rb;


    void Awake()
    {
        aSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        float clampedValue = Mathf.Clamp(MathUtil.Map(rb.velocity.magnitude, objectSpeedLow, objectSpeedHigh, mappingLow, mappingHigh), mappingLow, mappingHigh);

        switch (objectSpeedMap)
        {
            

            case AudioSourceMap.Volume:

                aSource.volume = Mathf.Lerp(aSource.volume, clampedValue, smoothing);

                break;
            case AudioSourceMap.QuantPitch:
                
                //first we want to clamp our semitone to an int
                int semiTone = Mathf.RoundToInt(clampedValue);

                //the formula from our starting note to the new note n is going 
                //to be 2^(n/12)
                //this will convert a 12 note-to-an-octave step to a relative pitch value
                //so, for instance, 12 semitones up (or one octave up) will convert to a pitch of 2.0f
                float chromScale = Mathf.Pow(2f, 1.0f / 12f);

                if (musicalScale != null && useScale)
                {
                    //find the octave
                    int octave = Mathf.FloorToInt(semiTone / 12f);
                    //find the percentage of the octave from the current semitone
                    float percentage = (semiTone % 12) / 12f;

                    //find the corresponding scale degree
                    int newScaleDegree = musicalScale.scaleNotes[
                        Mathf.FloorToInt(percentage * musicalScale.scaleNotes.Length)];
                    aSource.pitch = Mathf.Pow(chromScale, (octave * 12f) + newScaleDegree);

                }

                //otherwise just use chromatic scale
                else
                {
                    aSource.pitch = Mathf.Pow(chromScale, semiTone);
                }

                break;
            case AudioSourceMap.ContPitch:
                aSource.pitch = Mathf.Lerp(aSource.pitch, clampedValue, smoothing);
                break;
            default:
                break;
        }
    }
}
