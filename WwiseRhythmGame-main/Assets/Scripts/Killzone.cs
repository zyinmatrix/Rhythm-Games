using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Killzone : MonoBehaviour
{

    public Text feedbackText;
    private void OnTriggerEnter(Collider other)
    {

        if(other.tag != "Spaceship")
        {
            // print test HIT By Asteroid!
            feedbackText.text = "HIT by\nan Asteroid!";
            feedbackText.color = Color.gray;
        }

        Destroy(other.gameObject);
    }
}
