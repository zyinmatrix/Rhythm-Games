using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//makes particles where bounces happen
public class PlatformParticles : MonoBehaviour
{
    public GameObject particlePrefab;
    public float particleLifetime = 5f;

    //connect this to the "bounceEvent" on Sample Bounce
    public void PlayParticles(Vector3 bouncePos)
    {
        GameObject newParticles = Instantiate(particlePrefab, bouncePos, Quaternion.identity);
        ParticleSystem ps = newParticles.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(newParticles, particleLifetime);
    }
}
