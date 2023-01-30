using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMusicPlayer : MonoBehaviour
{
    public AK.Wwise.Event musicEvent;

    //switches for our different chords
    public AK.Wwise.Switch AMin, CMaj, DMin, Emin, FMaj, GMaj;

    //put these in same order, there should be 6
    public AK.Wwise.Switch[] chordSwitches;

    AK.Wwise.Switch currentChord;

    uint playingID;

    public void StartMusic()
    {
        CMaj.SetValue(gameObject);
        currentChord = CMaj;
        playingID = musicEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncUserCue, ChooseNextChord);

    }

    //here we're going to choose the next chord based on some probabilities. If you want to keep working in this way, it might be nice to find a CSV importer and do things in an excel sheet
    public void ChooseNextChord(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {

        Debug.Log(currentChord.Name);
        Debug.Log(GMaj.Name);
        //do our dice roll
        float randValue = Random.value;

        if(currentChord.Name == AMin.Name)
        {                           // Am   CMaj    Dm      Em      FM      GM 
            ChordPicker(new float[] { 0.1f, 0.4f,   0.2f,   0.1f,   0.1f,   0.1f });
            Debug.Log("A");
        }
        else if(currentChord.Name == CMaj.Name)
        {
            ChordPicker(new float[] { 0.2f, 0.2f,   0.2f,   0.1f,   0.2f,   0.1f });
            Debug.Log("C");
        }
        else if(currentChord.Name == DMin.Name)
        {
            ChordPicker(new float[] { 0.1f, 0.1f,    0.2f,  0.1f,   0.4f,   0.1f });
            Debug.Log("D");
        }
        else if (currentChord.Name == Emin.Name)
        {
            ChordPicker(new float[] { 0.4f, 0.1f,    0.2f,  0.05f,   0.05f,     0.2f });
            Debug.Log("E");
        }
        else if (currentChord.Name == FMaj.Name)
        {
            ChordPicker(new float[] { 0.1f, 0.3f,    0.2f,   0.1f,   0.1f,      0.2f });
            Debug.Log("f");
        }
        else if (currentChord.Name == GMaj.Name)
        {
            ChordPicker(new float[] { 0.3f, 0.3f,   0.05f,   0.05f,  0.2f,      0.1f });
            Debug.Log("g");
        }


    }

    void ChordPicker(float[] chordWeights)
    {

        Debug.Log("picking chord");
        if (chordWeights.Length != 6) Debug.LogWarning("need 6 entries for chord weights!");

        float testValue = 0;
        for(int i = 0; i < chordWeights.Length; i ++)
        {
            testValue += chordWeights[i];
        }
        if (testValue != 1.0f) Debug.LogWarning("weights should add up to 1!");

        float randValue = Random.value;

        //start at 0
        //add the weight from this entry
        //check to see if our rand value is below this weight

        //this method has it's flaws, but it's ok for our purposes.
        float progCounter = 0f;
        
        for (int i = 0; i < chordWeights.Length; i++)
        {
            progCounter += chordWeights[i];
            if (randValue < progCounter)
            {
                currentChord = chordSwitches[i];
                chordSwitches[i].SetValue(gameObject);
                Debug.Log("Setting value: " + chordSwitches[i].Name);
                return;
            }
        }

    }
}
