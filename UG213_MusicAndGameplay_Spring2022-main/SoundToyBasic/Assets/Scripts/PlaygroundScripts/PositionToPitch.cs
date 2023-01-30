using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maps an XY screen position to a pitch
//optional overloads for scale and root note
public class PositionToPitch : MonoBehaviour
{
    public enum spaceMappingMode { radial, diagonal, yOnly, xOnly}
    public spaceMappingMode defaultMapping = spaceMappingMode.radial;

    //our minimum and maximum screen bounds
    public float minX, minY, maxX, maxY;

    //calculated for radial mode
    Vector2 xyCenter;

    //either -3f to +3f for using audio source pitch, or 0 to 127 for MIDI notes
    [Header("pitch in semitones")]
    [Space(10)]
    [Header("relative to root note")]
    [Range(0f, 128f)] public float minPitch;
    [Range(0f, 128f)] public float maxPitch;

    public bool useScale = false;

    [Header("optional additions")]
    public MusicalScale scale;

    public bool sendMidi = false;

    //optional pitch map curve - doesn't change the output range, just scales it.
    public AnimationCurve pitchMapCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private void Start()
    {
        //find center point for radial mode
        float xCenter = minX + (maxX - minX) / 2f;
        float yCenter = minY + (maxY - minY) / 2f;

        Vector2 xyCenter = new Vector2(xCenter, yCenter);
    }

    public float PitchMapping(Vector2 inputPos)
    {
        float output = 1f;

        float yOutput = 0f;
        float xOutput = 0f;

        switch(defaultMapping)
        {
            case spaceMappingMode.diagonal:
                //take the higher number of the x or y mapping
                yOutput = MathUtil.Map(inputPos.y, minY, maxY, minPitch, maxPitch);
                xOutput = MathUtil.Map(inputPos.x, minX, maxX, minPitch, maxPitch);
                output = Mathf.Min(xOutput, yOutput);
                break;
            case spaceMappingMode.radial:
                //need to find the distance between our inputPos and the center
                //then compare it to the max distance. 

                //Note that for simplicity, we're not treating x and y distances separately.
                //so, if you have less y screen space than x, you won't get the full pitch range in the y direction.

                float distance = Vector2.Distance(inputPos, xyCenter);
                float maxDistance = Mathf.Max(Mathf.Abs(maxY - xyCenter.y), Mathf.Abs(maxX - xyCenter.x));
                output = MathUtil.Map(distance, 0, maxDistance, minPitch, maxPitch);
                
                break;
            case spaceMappingMode.xOnly:
                output = MathUtil.Map(inputPos.x, minX, maxX, minPitch, maxPitch);
                break;
            case spaceMappingMode.yOnly:
                output = MathUtil.Map(inputPos.y, minY, maxY, minPitch, maxPitch);
                break;

            default:
                break;

        }

        Debug.Log("output pitch: " + output);
        Debug.Log("min Y" + minY);

        output = MathUtil.ScaleToAnimCurve(output, minPitch, maxPitch, pitchMapCurve);


        //if we're using midi and/or scales
        if (useScale)
        {
            output = ScaledPitch(output, sendMidi);
        }

        return output;
    }

    public float ScaledPitch(float inputPitch, bool sendMidi = false)
    {

        //first we want to clamp our semitone to an int
        int semiTone = Mathf.RoundToInt(inputPitch);

        //the formula from our starting note to the new note n is going 
        //to be 2^(n/12)
        //this will convert a 12 note-to-an-octave step to a relative pitch value
        //so, for instance, 12 semitones up (or one octave up) will convert to a pitch of 2.0f
        float chromScale = Mathf.Pow(2f, 1.0f / 12f);

        //find the octave
        int octave = Mathf.FloorToInt(semiTone / 12f);
        //find the percentage of the octave from the current semitone
        float percentage = (semiTone % 12) / 12f;

        //find the corresponding scale degree
        int newScaleDegree = scale.scaleNotes[
            Mathf.FloorToInt(percentage * scale.scaleNotes.Length)];

        //add the octave back
        newScaleDegree += Mathf.RoundToInt(octave * 12f);

        //if we're sending midi, we're done. 
        //otherwise we need to convert this to the audio source pitch value
        if (sendMidi)
        {
            return (float) newScaleDegree;
        }
        
        else
        {
            return Mathf.Pow(chromScale, newScaleDegree);
        }
    }


}
