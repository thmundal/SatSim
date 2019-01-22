using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBody : MonoBehaviour
{
    public float mass;
    public Vector3 velocity;
    public static float G = 6.673f * Mathf.Pow(10, -11);
    public float gravitationalPull = 0;

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

        foreach(PhysicalBody body in otherBodies) {
            if (body == this) continue;
            
            ApplyGravityWith(body);
            body.ApplyGravityWith(this);
            //Debug.DrawLine(transform.position, body.transform.position);
        }

        transform.position += velocity;
    }

    public float GravityBetween(PhysicalBody other)
    {
        float d_squared = Mathf.Pow(Vector3.Distance(transform.position, other.transform.position), 2);
        if(d_squared == 0)
        {
            d_squared = 1 / float.MaxValue;
        }
        return G * mass * other.mass / d_squared;
    }

    public void ApplyGravityWith(PhysicalBody other)
    {
        Vector3 direction = (other.transform.position - transform.position).normalized;
        gravitationalPull = GravityBetween(other);
        Debug.DrawLine(transform.position, (direction * gravitationalPull) / mass);
        velocity += (direction * gravitationalPull) / mass;
    }
}
