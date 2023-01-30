using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [Header("Switch if you're using tied notes!")]
    public bool usingTies = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!usingTies)
        {
            Destroy(other.gameObject);
        }
    }
}
