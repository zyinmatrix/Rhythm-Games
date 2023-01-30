using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerElevationToWwise : MonoBehaviour
{
    public AK.Wwise.RTPC playerElevationParam;

    public StarterAssets.FirstPersonController fpsController;

    public Gradient colorGradient;

    public float dayNightClock;
    public float timeScale = 0.1f;
    public float maxDayNightTime = 24;




    // Update is called once per frame
    void Update()
    {
        playerElevationParam.SetGlobalValue(transform.position.y);


        dayNightClock += Time.deltaTime / 0.1f; 
        if (dayNightClock > maxDayNightTime)
        {
            dayNightClock = 0f;
        }

        

    }
}
