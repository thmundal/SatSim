using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMover : MonoBehaviour
{
    public float radius = 100f;
    public float deltaTheta = 1f;

    private float theta = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        theta += deltaTheta * Time.deltaTime;
        //transform.position = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
        //transform.position = new Vector3(0f, radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
        transform.position = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
    }
}
