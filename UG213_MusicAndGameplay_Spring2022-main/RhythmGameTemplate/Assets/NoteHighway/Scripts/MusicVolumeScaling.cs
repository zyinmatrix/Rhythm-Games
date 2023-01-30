using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeScaling : MonoBehaviour
{

    public AK.Wwise.RTPC musicVolumeRTPC;

    public float scaleFactor = 3f;

    [Range(0f, 1f)] public float smoothingFactor = 0.5f;

    private Vector3 startScale;
    private float previousScaledValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float musicVolumeValue = musicVolumeRTPC.GetGlobalValue();

        //go from 0.0 - 1.0
        float normalizedValue = Mathf.InverseLerp(
            -12f, 
            0f, 
            Mathf.Clamp(musicVolumeValue, -12f, 0f));

        float scaledValue = Mathf.Lerp(1.0f, scaleFactor, normalizedValue);

        float finalValue = Mathf.Lerp(previousScaledValue, scaledValue, smoothingFactor);

        transform.localScale = new Vector3(finalValue, finalValue, finalValue);

        previousScaledValue = finalValue;

        //ways to improve:
        //smoothing (lerp, 
        //bigger peaks and valleys -- reduce the range on the incoming value

    }
}
