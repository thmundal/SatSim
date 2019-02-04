﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBody : MonoBehaviour
{
    public float mass;
    public Vector3 velocity;
    public static float G = 6.673f * Mathf.Pow(10, -11);
    public float gravitationalPull = 0;
    public Vector3 gravityVector;

    public PhysicalBody[] otherBodies;

    // Start is called before the first frame update
    void Start()
    {
        otherBodies = FindObjectsOfType<PhysicalBody>();
    }

    // Update is called once per frame
    void Update()
    {
        // F = ma
        // a = m/F

        gravitationalPull = 0;
        gravityVector = Vector3.zero;

        foreach(PhysicalBody body in otherBodies) {
            if (body == this) continue;

            // Reset grav pull. Should reorganize this parameter to something that makes more sense

            gravitationalPull += GravityBetween(body);
            gravityVector += GravityVectorTowards(body);

            //ApplyGravityWith(body);
            //Debug.DrawLine(transform.position, body.transform.position);
        }

        ApplyGravity(gravityVector, gravitationalPull);
        Debug.DrawLine(transform.position, transform.position + (velocity * 5), Color.red);

        transform.position += velocity * Time.deltaTime;
    }

    public Vector3 GravityVectorTowards(PhysicalBody other)
    {
        return (other.transform.position - transform.position).normalized;
    }

    public float GravityBetween(PhysicalBody other)
    {
        float d_squared = Mathf.Pow(Vector3.Distance(transform.position, other.transform.position), 2);
        if(d_squared == 0)
        {
            d_squared = 1 / float.MaxValue;
        }
        float pull = G * mass * other.mass / d_squared;
        return pull;
    }

    public void ApplyGravityWith(PhysicalBody other)
    {
        Vector3 direction = (other.transform.position - transform.position).normalized;
        Debug.DrawLine(transform.position, (direction * gravitationalPull) / mass);
        velocity += (direction * gravitationalPull) / mass;
    }

    public void ApplyGravity(Vector3 direction, float force)
    {
        velocity += (direction * force) / mass;
    }
}
