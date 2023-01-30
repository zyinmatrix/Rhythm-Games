using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



[System.Serializable]
public class FallingGemInput
{
    [Tooltip("this must match the name in unity's input manager")]
    public string playerInput;
    public InputAction inputAction;

    public GameObject cueStartLocation;
    public GameObject cuePrefab;

}

//This script drives the level, in conjunction with NoteHighwayWwiseSync
public class GemGenerator : MonoBehaviour
{
    //the "cue" here can be a number of things
    //for now it's just the spawn time offset (in number of beats)
    //right now, this assumes that each beatEvent will have the same cue offset.
    //if you don't want this to be the case, have a seperate beatmap/input evaluator pair for these other events
    //(example - rhythm heaven has varying cue lengths)
    [Header("Cue Offset in Beats")]
    public int cueBeatOffset;

    //Make sure the OkWindow > GoodWindow > PerfectWindow!!!  Also make sure that you don't have successive notes at shorter timespans than your OkWindow
    [Header("Window Sizes in MS")]
    public int OkWindowMillis = 200;
    public int GoodWindowMillis = 100;
    public int PerfectWindowMillis = 50;

    //you can + should mix this up - I chose 3 inputs just to keep things simple
    //note that you should do a "find references" to find other areas of the code that you should change
    public FallingGemInput fallingGemR, fallingGemG, fallingGemB;


    [Header("Assign this - Gems won't work otherwise")]
    public NoteHighwayWwiseSync wwiseSync;

    //caching sustained notes upon creation]
    FallingGem RSustainStart = null;
    FallingGem GSustainStart = null;
    FallingGem BSustainStart = null;

    private void OnEnable()
    {
        fallingGemR.inputAction.Enable();
        fallingGemB.inputAction.Enable();
        fallingGemG.inputAction.Enable();
    }

    private void OnDisable()
    {
        fallingGemR.inputAction.Disable();
        fallingGemB.inputAction.Disable();
        fallingGemG.inputAction.Disable();
    }

    private void Start()
    {
        RSustainStart = null;
    }

    //In the example scene+song, there is a cue called "EndLevel" which happens at the end of the song
    public void EndLevel()
    {
        Debug.Log("Level Ended");
    }

    

    //We connect these next three methods to relevant events on our Note Highway Wwise Sync
    public void GenerateRCue()
    {
        //we need to instantiate the cue, set the desired player input accordingly, and then set the window timings
        GameObject newCue = Instantiate(fallingGemR.cuePrefab, fallingGemR.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemR.playerInput;

        SetGemTimings(fallingGem);
    }

    public void GenerateRSustainStart()
    {
        GameObject newCue = Instantiate(fallingGemR.cuePrefab, fallingGemR.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemR.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.start;

        RSustainStart = fallingGem;

        SetGemTimings(fallingGem);
    }

    public void GenerateRSustainEnd()
    {
        GameObject newCue = Instantiate(fallingGemR.cuePrefab, fallingGemR.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemR.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.end;
        if(RSustainStart == null)
        {
            Debug.LogWarning("we have an R sustain end without a start!");
        }

        else
        {
            fallingGem.connectedNote = RSustainStart.gameObject;
            RSustainStart.connectedNote = fallingGem.gameObject;
        }
        

        SetGemTimings(fallingGem);
    }

    public void GenerateGCue()
    {
        GameObject newCue = Instantiate(fallingGemG.cuePrefab, fallingGemG.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemG.playerInput;

        SetGemTimings(fallingGem);
    }

    public void GenerateGSustainStart()
    {
        GameObject newCue = Instantiate(fallingGemG.cuePrefab, fallingGemG.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemG.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.start;

        GSustainStart = fallingGem;

        SetGemTimings(fallingGem);
    }

    public void GenerateGSustainEnd()
    {
        GameObject newCue = Instantiate(fallingGemG.cuePrefab, fallingGemG.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemG.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.end;
        if (GSustainStart == null)
        {
            Debug.LogWarning("we have an R sustain end without a start!");
        }

        else
        {
            fallingGem.connectedNote = GSustainStart.gameObject;
            GSustainStart.connectedNote = fallingGem.gameObject;
        }


        SetGemTimings(fallingGem);
    }


    public void GenerateBCue()
    {
        GameObject newCue = Instantiate(fallingGemB.cuePrefab, fallingGemB.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemB.playerInput;

        SetGemTimings(fallingGem);
    }

    public void GenerateBSustainStart()
    {
        GameObject newCue = Instantiate(fallingGemB.cuePrefab, fallingGemB.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemB.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.start;

        BSustainStart = fallingGem;

        SetGemTimings(fallingGem);
    }

    public void GenerateBSustainEnd()
    {
        GameObject newCue = Instantiate(fallingGemB.cuePrefab, fallingGemB.cueStartLocation.transform.position, Quaternion.identity);

        FallingGem fallingGem = newCue.GetComponent<FallingGem>();

        fallingGem.playerInput = fallingGemB.playerInput;
        fallingGem.sustainType = FallingGem.SustainType.end;
        if (BSustainStart == null)
        {
            Debug.LogWarning("we have an R sustain end without a start!");
        }

        else
        {
            fallingGem.connectedNote = BSustainStart.gameObject;
            BSustainStart.connectedNote = fallingGem.gameObject;
        }


        SetGemTimings(fallingGem);
    }



    void SetGemTimings(FallingGem fallingGem)
    {

        fallingGem.wwiseSync = wwiseSync;

        fallingGem.crossingTime = (float)wwiseSync.SetCrossingTimeInMS(cueBeatOffset);

        //Set Window Timings - we're going to use wwise for this
        fallingGem.OkWindowStart = fallingGem.crossingTime - (0.5f * OkWindowMillis);
        fallingGem.OkWindowEnd = fallingGem.crossingTime + (0.5f * OkWindowMillis);
        fallingGem.GoodWindowStart = fallingGem.crossingTime - (0.5f * GoodWindowMillis);
        fallingGem.GoodWindowEnd = fallingGem.crossingTime + (0.5f * GoodWindowMillis);
        fallingGem.PerfectWindowStart = fallingGem.crossingTime - (0.5f * PerfectWindowMillis);
        fallingGem.PerfectWindowEnd = fallingGem.crossingTime + (0.5f * PerfectWindowMillis);
    }

}
