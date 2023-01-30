using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class RhythmInput
{
    public InputAction inputAction;
    public enum InputState { down, held, up }
    public InputState inputState;

    //corresponds to Input System button
    public string inputString;
}

/// <summary>
/// The purpose of this class is twofold:
/// - Get the Clock-synchronized timing of the user's input
/// - Check that against the windows of currently existing obstacles
/// </summary>
public class InputEvaluator : MonoBehaviour
{
    
    public List<FallingGem> activeGems;
    public List<FallingGem> activeSustains;
    public List<RhythmInput> cachedInputs = new List<RhythmInput>();

    public GemGenerator gemGenerator;

    //ideally we'd manage score on a seperate script
    public int gameScore;

    public NoteHighwayWwiseSync wwiseSync;

    bool gemRHeld, gemGHeld, gemBHeld;

    private void Awake()
    {
        gemGenerator.fallingGemR.inputAction.started += ctx => OnRPerformed();
        gemGenerator.fallingGemR.inputAction.canceled += ctx => OnRReleased();

        gemGenerator.fallingGemG.inputAction.started += ctx => OnGPressed();
        gemGenerator.fallingGemG.inputAction.canceled += ctx => OnGReleased();

        gemGenerator.fallingGemB.inputAction.started += ctx => OnBPressed();
        gemGenerator.fallingGemB.inputAction.canceled += ctx => OnBReleased();


    }

    void LateUpdate()
    {
        //only used for debugging right now
        float wwiseTime = wwiseSync.GetMusicTimeInMS();

        //every frame, we do two things:
        //1: cache all of our inputs, so we know what the player pressed
        //2: evaluate every gem that's in play

        //now that we've cached any inputs, compare inputs to current beatMap windows

        //first find any non-destroyed cues

        FallingGem[] allGems = FindObjectsOfType<FallingGem>();

        activeGems.AddRange(allGems);
        for (int i = 0; i < activeGems.Count; i ++)
        {
            //we're not going to do anything with early inputs or has already been scored
            if (activeGems[i].gemCueState != FallingGem.CueState.Early 
                || activeGems[i].gemCueState != FallingGem.CueState.AlreadyScored)
            {
                
                //if player hasn't input anything, don't do anything
                if (cachedInputs.Count == 0)
                    break;
                //go through each of our inputs from this frame, and check them against this gem
                for (int j = 0; j < cachedInputs.Count; j++)
                {
                    if (CheckInputTypeMatch(activeGems[i], cachedInputs[j]))
                    {
                        ScoreGem(activeGems[i], cachedInputs[j]);
                    }
                }
            }
        }

        //clear Lists (note that this doesn't destroy anything, just clears the cache's to prepare for the next frame)

        activeGems.Clear();
        cachedInputs.Clear();
    }

    //using new Unity Input System
    void OnRPerformed()
    {
        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemR.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.down;
        _rhythmInput.inputAction = gemGenerator.fallingGemR.inputAction;

        cachedInputs.Add(_rhythmInput);
    }

    // -- MIGRATING TO NEW INPUT SYSTEM -- //
    // this could be streamlined + modularized to account for additional gems
    void OnRReleased()
    {

        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemR.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.up;

        cachedInputs.Add(_rhythmInput);

    }

    void OnGPressed()
    {
        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemG.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.down;
        cachedInputs.Add(_rhythmInput);
    }
    
    void OnGReleased()
    {
        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemG.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.up;
        cachedInputs.Add(_rhythmInput);
    }

    void OnBPressed()
    {
        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemB.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.down;
        cachedInputs.Add(_rhythmInput);
    }

    void OnBReleased()
    {
        RhythmInput _rhythmInput = new RhythmInput();
        _rhythmInput.inputString = gemGenerator.fallingGemB.playerInput;
        _rhythmInput.inputState = RhythmInput.InputState.up;
        cachedInputs.Add(_rhythmInput);
    }

    void ScoreGem(FallingGem gem, RhythmInput input)
    {

        switch (gem.gemCueState)
        {
            case FallingGem.CueState.AlreadyScored:
                break;
            case FallingGem.CueState.Sustain:
                break;
            case FallingGem.CueState.OK:
                
                gameScore += 1;
                Debug.Log("OK!");
                gem.GemScored();
                if(gem.sustainType == FallingGem.SustainType.start)
                {
                    activeSustains.Add(gem);
                }
                break;
            case FallingGem.CueState.Good:
                gameScore += 2;
                Debug.Log("Good!");
                gem.GemScored();
                if (gem.sustainType == FallingGem.SustainType.start)
                {
                    activeSustains.Add(gem);
                }
                break;
            case FallingGem.CueState.Perfect:
                gameScore += 3;
                Debug.Log("Perfect!");
                gem.GemScored();
                if (gem.sustainType == FallingGem.SustainType.start)
                {
                    activeSustains.Add(gem);
                }
                break;
            case FallingGem.CueState.Late:
                Debug.Log("Missed!");

                gem.GemLate();
                break;
            
        }


    }

    //expanded to account for sustain notes
    bool CheckInputTypeMatch(FallingGem gem, RhythmInput input)
    {
        //first check that the input string (corresponding to the input manager) is the same as the one on the gem
        if(input.inputString != gem.playerInput)
        {
            return false;
        }

        
        //then check the button position against the sustain
        switch(gem.sustainType)
        {
            case FallingGem.SustainType.start:
                return (input.inputState == RhythmInput.InputState.down);
            case FallingGem.SustainType.none:
                return (input.inputState == RhythmInput.InputState.down);
            case FallingGem.SustainType.end:
                return (input.inputState == RhythmInput.InputState.up);
        }

        //return false if we are still mismatched
        return false;
    }


    /* ROOM FOR EXPANSION - better checking of held notes
     * - check to see if a sustain down has been activated
     * - add Input.GetButton() conditions in our update function - this checks if the button is held
     * - continuously add points if there's a corresponding gem in a "Sustain" state
     * - if player releases too soon, then end the sustain state and score the release as "missed" 
    */
    void SuccessfulHold ()
    { }

}
