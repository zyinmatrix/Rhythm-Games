using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Music", fileName = "MusicalScale")]
public class MusicalScale : ScriptableObject
{
    [Header("Be sure the first note is 0")]
    public int[] scaleNotes;
}
