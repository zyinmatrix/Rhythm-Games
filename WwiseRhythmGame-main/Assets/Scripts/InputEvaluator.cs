﻿  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmInput
{
    public KeyCode inputKey;
    public float inputTime;  //in MS

    //only relevant for updatedInputSystem
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
    public List<RhythmInput> CachedInputs = new List<RhythmInput>();

    public GemGenerator gemGenerator;

    //ideally we'd manage score on a seperate script
    public int gameScore;

    public NoteHighwayWwiseSync wwiseSync;

    //visual feedback in the form of particles
    public GameObject perfectParticles, goodParticles, okParticles;
    public GameObject badParticles;

    //different colors for particles and feedback text;
    // public Color perfectColor, goodColor, okColor, missedColor;
    public Color bombColor;

    public Text feedbackText;
    public Text scoreText;
    public Text warningText;

    // public UnityEvent ScaleWarning;
    private bool showingWarning = false;
    private bool isLargeWarning = false;
    private float warningTimer = 0.0f;
    private float scaleTimer = 0.36f;

    void Update()
    {

        float wwiseTime = wwiseSync.GetMusicTimeInMS();

        //every frame, we do two things:
        //1: cache all of our inputs, so we know what the player pressed
        //2: evaluate every gem that's in play


        if (Input.GetButtonDown(gemGenerator.fallingGemQ.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemQ.playerInput;

            //might not be necessary?
            _rhythmInput.inputTime = wwiseTime;

            CachedInputs.Add(_rhythmInput);
            Debug.Log("Cached Input: " + _rhythmInput.inputString);
        }

        if(Input.GetButtonDown(gemGenerator.fallingGemW.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemW.playerInput;
            _rhythmInput.inputTime = wwiseTime;
            CachedInputs.Add(_rhythmInput);
        }

        if(Input.GetButtonDown(gemGenerator.fallingGemO.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemO.playerInput;
            _rhythmInput.inputTime = wwiseTime;
            CachedInputs.Add(_rhythmInput);
        }
        if (Input.GetButtonDown(gemGenerator.fallingGemP.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemP.playerInput;
            _rhythmInput.inputTime = wwiseTime;
            CachedInputs.Add(_rhythmInput);
        }

        //compare inputs to current beatMap windows

        //first find any non-destroyed cues

        FallingGem[] allGems = FindObjectsOfType<FallingGem>();

        activeGems.AddRange(allGems);
        for (int i = 0; i < activeGems.Count; i ++)
        {
            //we're not going to do anything with early inputs
            if (activeGems[i].gemCueState != FallingGem.CueState.Early)
            {
                //if player hasn't input anything, don't do anything
                if (CachedInputs.Count == 0)
                    break;
                //go through each of our inputs from this frame, and check them against this gem
                for (int j = 0; j < CachedInputs.Count; j++)
                {
                    if (CachedInputs[j].inputString == activeGems[i].playerInput)
                    {
                        ScoreGem(activeGems[i]);
                        scoreText.text = "Score:\n" + gameScore; //update score
                    }
                }
            }
        }
        

        //clear Lists
        activeGems.Clear();
        CachedInputs.Clear();
    }

    void FixedUpdate()
    {
         //check warning
        if(showingWarning)
        {
            showingWarning = false;
            warningTimer = 2.1f;
        }

        if(warningTimer > 0)
        {
            warningText.text = "YOU HIT A SPACESHIP!";
            warningText.color = bombColor;
            warningTimer -= Time.deltaTime;
        }
        else
        {
            warningText.text = "";
        }

        scaleTimer -= Time.deltaTime;
        if(scaleTimer <= 0)
        {
            isLargeWarning = !isLargeWarning;
            scaleTimer = 0.36f;
            if(isLargeWarning) warningText.fontSize = 40;
            else warningText.fontSize = 30;
        }
        
    }

    void ScoreGem(FallingGem gem)
    {
        GameObject newParticles;
        if(gem.isBomb){
            if(gem.gemCueState != FallingGem.CueState.Late){
                gameScore -= 6;
                Debug.Log("Bomb!");
                // feedbackText.text = "YOU HIT\na SPACESHIP!";
                // feedbackText.color = bombColor;
                showingWarning = true;
                Destroy(gem.gameObject);

                //deploy particles
                newParticles = Instantiate(badParticles, gem.transform.position, Quaternion.identity);
                var main = newParticles.GetComponent<ParticleSystem>().main;
                main.startColor = bombColor;
                Destroy(newParticles, 2f); 
            }
            return;
        }

        switch (gem.gemCueState)
        {
            case FallingGem.CueState.OK:
                gameScore += 1;
                Debug.Log("OK!");
                feedbackText.text = "Ok!";
                // feedbackText.color = okColor;
                feedbackText.color = gem.gemColor;
                Destroy(gem.gameObject);

                //deploy particles
                newParticles = Instantiate(okParticles, gem.transform.position, Quaternion.identity);
                var main = newParticles.GetComponent<ParticleSystem>().main;
                // main.startColor = okColor;
                main.startColor = gem.gemColor;
                Destroy(newParticles, 2f); 

                break;
            case FallingGem.CueState.Good:
                gameScore += 3;
                Debug.Log("Good!");
                feedbackText.text = "Good!";
                // feedbackText.color = goodColor;
                feedbackText.color = gem.gemColor;
                Destroy(gem.gameObject);

                //deploy particles
                newParticles = Instantiate(goodParticles, gem.transform.position, Quaternion.identity);
                main = newParticles.GetComponent<ParticleSystem>().main;
                // main.startColor = goodColor;
                main.startColor = gem.gemColor;
                Destroy(newParticles, 2f);

                break;
            case FallingGem.CueState.Perfect:
                gameScore += 5;
                Debug.Log("Perfect!");
                feedbackText.text = "Perfect!";
                // feedbackText.color = perfectColor;
                feedbackText.color = gem.gemColor;
                Destroy(gem.gameObject);

                newParticles = Instantiate(perfectParticles, gem.transform.position, Quaternion.identity);
                main = newParticles.GetComponent<ParticleSystem>().main;
                // main.startColor = perfectColor;
                main.startColor = gem.gemColor;
                Destroy(newParticles, 2f);

                break;
            case FallingGem.CueState.Late:
                // feedbackText.text = "Missed!";
                // Debug.Log(gem.playerInput);
                // feedbackText.color = perfectColor;
                Debug.Log("Missed!");

                newParticles = Instantiate(badParticles, gem.transform.position, Quaternion.identity);
                Destroy(newParticles, 2f);

                break;
        }


    }

}
