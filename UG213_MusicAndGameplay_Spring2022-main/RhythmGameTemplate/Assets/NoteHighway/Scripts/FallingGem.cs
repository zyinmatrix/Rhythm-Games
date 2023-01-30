using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGem : MonoBehaviour
{

    [HideInInspector] public double OkWindowStart;
    [HideInInspector] public double GoodWindowStart, PerfectWindowStart, PerfectWindowEnd, GoodWindowEnd, OkWindowEnd;
    [HideInInspector] public float crossingTime;

    public NoteHighwayWwiseSync wwiseSync;


    public enum CueState { Early = 0, OK = 1, Good = 2, Perfect = 3, Late = 4, AlreadyScored = 5, Sustain = 6}
    public CueState gemCueState;

    public Vector3 destination;

    private Vector3 velocity;

    //may be needed to make the game feel more fair
    public float crossPositionOffset;

    //debugging crossing sync issues
    private bool _gemCrossed = false;

    public string playerInput;

    //new additions as of 7 March 2022
    public enum SustainType { none, start, end}
    [HideInInspector] public SustainType sustainType;
    [HideInInspector] public GameObject connectedNote;
    [HideInInspector] public LineRenderer lineRenderer;

    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        destination = GameObject.FindGameObjectWithTag("NowCrossing").transform.position;

        //we want to stay in the lane, so the destination will have the same x and y coordinates as the start.
        destination.x = transform.position.x;
        destination.y = transform.position.y;
        destination.z -= crossPositionOffset;

        //velocity = distance/time -- we want to make sure that the cue crosses our destination on beat
        velocity = (destination - transform.position) / (float)(0.001f*(crossingTime - wwiseSync.GetMusicTimeInMS()));

        gemCueState = CueState.Early;

        //we use the line renderer to connect sustained notes
        lineRenderer = GetComponent<LineRenderer>();
        
        //you can change this or set it manually if you wish
        lineRenderer.startColor = GetComponent<Renderer>().material.color;
        lineRenderer.endColor = GetComponent<Renderer>().material.color;

        if (sustainType == SustainType.start)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, startPosition);
        }
        else
        {
            lineRenderer.enabled = false;
        }

        
    }

   
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
        UpdateWindow();
        UpdateLineRenderer();
    }

    public void UpdateWindow()
    {
        //check our cue state against Wwise event

        //for this case (more-or-less typical japanese rhythm game style), our detection windows are 
        // early - ok - good - perfect - good - ok - late

        switch (gemCueState)
        {

            case CueState.Early:
                //check to see if we've gotten to "ok"
                if (wwiseSync.GetMusicTimeInMS() > OkWindowStart)
                {
                    gemCueState = CueState.OK;
                }
                break;
            case CueState.OK:
                //check to see if we've gotten to "good"...
                if (wwiseSync.GetMusicTimeInMS() > GoodWindowStart && wwiseSync.GetMusicTimeInMS() < PerfectWindowStart)
                {
                    gemCueState = CueState.Good;

                }
                //... or maybe we're at the end of the last "ok" window
                else if (wwiseSync.GetMusicTimeInMS() > OkWindowEnd)
                {
                    gemCueState = CueState.Late;

                    //right now, our scoring system is checking every gem that's still in existence, so it's a good idea to render this inactive once it's crossed the "late" threshold
                }
                break;
            case CueState.Good:
                //check to see if we've gotten to "perfect"
                if (wwiseSync.GetMusicTimeInMS() > PerfectWindowStart && wwiseSync.GetMusicTimeInMS() < PerfectWindowEnd)
                {
                    gemCueState = CueState.Perfect;
                }
                //
                else if (wwiseSync.GetMusicTimeInMS() > GoodWindowEnd)
                {
                    gemCueState = CueState.OK;
                }
                break;
            case CueState.Perfect:
                if (wwiseSync.GetMusicTimeInMS() > PerfectWindowEnd)
                {
                    gemCueState = CueState.Good;
                }
                break;
            default:
                //if we're "late" or "already scored" there are no more potential state changes
                break;


        }
    }

    public void UpdateLineRenderer()
    {
        if (sustainType != SustainType.start) return;

        lineRenderer.SetPosition(0, transform.position);

        if (connectedNote == null)
        {
            lineRenderer.SetPosition(1, startPosition);
        }
        else
        {
            lineRenderer.SetPosition(1, connectedNote.transform.position);
        }
    }

    /// <summary>
    /// called when the player successfully scores a gem
    /// </summary>
    public void GemScored()
    {
        //if we're not at the start of the sustain, the gem can be immediately destroyed
        if (sustainType != SustainType.start)
        {
            
            gemCueState = CueState.AlreadyScored;
            //instantiate particles

            //destroy immediately, and destroy any connected notes if they exist
            if (connectedNote != null)
            {
                Destroy(connectedNote.gameObject);
            }
            Destroy(gameObject);
        }
        //otherwise, we want to put the gem into a sustain state
        //right now this doesn't affect scoring - this is a potential space for improvement
        else
        {
            gemCueState = CueState.Sustain;
        }

    }

    /// <summary>
    /// called when the player misses a gem
    /// </summary>
    public void GemLate()
    {
        //AlreadyScored prevents the gem from being checked against future button presses
        gemCueState = CueState.AlreadyScored;
        
        //if it's a sustain note, we want to keep it around to see the tie note
        if(sustainType != SustainType.start)
        {
            Destroy(gameObject, 2f);

            //destroy connected note for released notes
            if(connectedNote != null)
            {
                Destroy(connectedNote.gameObject, 2f);
            }
        }
        else
        {
            //you can add some visual changes here if the player misses a sustain
            //maybe changing the color on the line renderer
        }
        
    }

    
}
