using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{

    Vector3 sensorDirection;
    Vector3 incomingLight;

    public Transform target;

    public float value = 0;
    public float angle = 0;

    public float actualAngle = 0;
    public Vector3 forward;

    // Start is called before the first frame update
    void Start()
    {
        sensorDirection = transform.forward;

        target = GameObject.FindGameObjectWithTag("sun").transform;

        if(target != null)
        {
            incomingLight = target.transform.position - transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            sensorDirection = transform.forward;
            incomingLight =  transform.position - target.transform.position;
            
            //Debug.DrawLine(target.transform.position, transform.position);
            Debug.DrawRay(transform.position, incomingLight);

            //incomingLight = Vector3.ProjectOnPlane(incomingLight, transform.right);

            angle = Vector3.Angle(incomingLight, sensorDirection);
            actualAngle = angle;
            forward = transform.forward;

            Debug.DrawRay(transform.position, sensorDirection, Color.red);


            if (angle > 90)
            {
                angle = 90;
            }

            value = 1 - angle / 90;
        }
    }
}
