using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;
using DG.Tweening;

//Similar behavior to the luminaria in electroplankton
public class HopperMove : MonoBehaviour
{
    

    //we're going to spawn audio prefabs instead of just playing the audio source
    //this is so we get the decay on our notes, and we don't change the pitch of a note while moving

    public GameObject audioPrefab;

    //our base interval (quarter note, eigth note, etc)
    public TickValue intervalBase;

    //hoppers pulse when they play a note - this controls how big they get
    public float pulseScale = 1.0f;

    //intervalCount * intervalBase;
    double intervalValue;

    double timeOfNextNote;

    float speed; //set after each hop;

    Vector3 moveDirection;

    int intervalCounter;

    public int silentInterval;


    //assigned at runtime from grid script

    [HideInInspector] public double intervalCount = 1d;

    [HideInInspector] public HopperDirector nextDirector;
    [HideInInspector] public HopperDirector prevDirector;
    [HideInInspector] public bool hasStarted = false;

    [HideInInspector] public float xBoundLow, xBoundHigh;
    [HideInInspector] public float yBoundLow, yBoundHigh;
    [HideInInspector] public PositionToPitch posToPitch;


    public void InitHopperMove()
    {
        //how long it's going to take to move
        //the time between notes
        intervalValue = intervalCount * Clock.Instance.LengthOfD(intervalBase);

        //play at next beat (or eigth, or whatever we've set intervalBase to)
        timeOfNextNote = Clock.Instance.AtNext(intervalBase);

        //used for movement
        nextDirector = prevDirector.targetObject.GetComponent<HopperDirector>();

        moveDirection = Vector3.Normalize(nextDirector.targetPosition - transform.position);

        speed = Vector3.Distance(nextDirector.targetPosition, transform.position) / Clock.Instance.LengthOf(intervalBase);
    }

    private void Start()
    {
        //when dealing with rhythmic timing, it's easy to run into "race conditions" - trying to access values that haven't yet been set.
        //so I've implemented this version to have a built-in 0.5 second start delay. This is a "magic number" variable which is bad coding practice; 
        //ideally we would know how much we need to wait from the clock script, or we would wait for the user to press a button
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
         
        if (!hasStarted) return;

        transform.position += moveDirection * speed * Time.deltaTime;
        transform.position = CheckBoundary();

        //update our interval value in case it was changed manually
        intervalValue = Clock.Instance.LengthOfD(intervalBase) * intervalCount;


        if (AudioSettings.dspTime >= timeOfNextNote)
        {
            transform.position = nextDirector.transform.position;

            transform.DOPunchScale(new Vector3(pulseScale, pulseScale, pulseScale), 0.2f);

            SetNewTarget();
            //this is a bit weird. once we hit a node, we're immediately going to spawn an audio prefab at our new target, and set it to play at the next interval.



            GameObject soundObject = Instantiate(audioPrefab, nextDirector.transform.position, Quaternion.identity);

            AudioSource soundSource = soundObject.GetComponent<AudioSource>();
            //this is because "PlayScheduled" is meant to be done ahead of time; we don't necessarily want to do it right as we arrive, which is what would happen if we connected it to our movement code.

            float nextPitch = posToPitch.PitchMapping(soundObject.transform.position);

            soundSource.pitch = nextPitch;

            timeOfNextNote += intervalValue;

            //play
            soundSource.PlayScheduled(timeOfNextNote);

            //contingency for pitch value of 0
            if (soundSource.pitch == 0f) soundSource.pitch = 1f;

            Destroy(soundObject, (float)intervalValue + (soundSource.clip.length / Mathf.Abs(soundSource.pitch)) + 0.2f);

            //intervalCounter++;
            //if i wanted to skip every silentInterval
            //if(intervalCounter % silentInterval == 0)
            //{

            //}


        }
    }

    /// <summary>
    /// called immediately after note plays -- finding our new target based on the direction the last HopperDirector was pointing
    /// </summary>
    void SetNewTarget()
    {
        prevDirector = nextDirector;
        
        moveDirection = Vector3.Normalize(prevDirector.targetPosition - transform.position);

        //speed is distance/time
        //we get the distance from our current position to the target, and we get our time from the clock script
        speed = Vector3.Distance(
            prevDirector.targetPosition, transform.position) 
            / 
            (float)intervalValue;

        nextDirector = prevDirector.targetObject.GetComponent<HopperDirector>();

        

    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.5f);
        {
            InitHopperMove();
        }
        hasStarted = true;
    }

    /// <summary>
    /// Use this for screen wrapping, put it just after the position update
    /// </summary>
    /// <returns></returns>
    Vector3 CheckBoundary()
    {
        Vector3 newPosition = transform.position;
        if (transform.position.x > xBoundHigh)
        {
            newPosition.x = xBoundLow;
        }
        if (transform.position.x < xBoundLow)
        {
            newPosition.x = xBoundHigh;
        }
        if (transform.position.y > yBoundHigh)
        {
            newPosition.y = yBoundLow;
        }
        if(transform.position.y < yBoundLow)
        {
            newPosition.y = yBoundHigh;
        }
        return newPosition;
    }
}
