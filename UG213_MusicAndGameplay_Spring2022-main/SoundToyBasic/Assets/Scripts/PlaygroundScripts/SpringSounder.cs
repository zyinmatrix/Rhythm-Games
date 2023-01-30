using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent For a Ball (or other object with a rigidbody and a MotionToSound) 
/// attached to a Point via a spring or hinge
/// 
/// Draws a line between this and the ball
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class SpringSounder : MonoBehaviour
{
    [Header("Spring-based motion sound - mouse drag on the fixed end to move it around")]
    [Space(10)]

    [Header("connect to bouncing ball or whatever is connected to the spring")]
    //we may not do anything with this
    public MotionToSound motionSound;
    public LineRenderer lineRenderer;

    [Header("optional - origin point of spring")]
    public GameObject springOrigin;

    float initialZPos;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (springOrigin == null) springOrigin = gameObject;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, motionSound.transform.position);

        initialZPos = transform.position.z;


    }

    private void Update()
    {
        //update line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, motionSound.transform.position);
    }

    private void OnMouseDrag()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = initialZPos;
        transform.position = newPos;
    }



}
