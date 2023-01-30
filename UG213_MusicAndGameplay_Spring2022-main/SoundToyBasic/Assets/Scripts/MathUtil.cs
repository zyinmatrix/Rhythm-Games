using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil 
{
    public static float Map(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    public static float ScaleToAnimCurve(float value, float min, float max, AnimationCurve animCurve)
    {
        //clamp it so we don't go out of bounds
        float tempValue = Mathf.Clamp(value, min, max);
        //we need to find out our current output percentage - where we are between min and max
        float valuePercentage = Mathf.InverseLerp(min, max, tempValue);
        //then use that value to evaluate our curve, which could be any shape we want
        float mappedPercentage = animCurve.Evaluate(valuePercentage);
        //and finally re-map back to our scale. 
        float finalParamValue = Mathf.Lerp(min, max, mappedPercentage);

        return finalParamValue;
    }
}
