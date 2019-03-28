using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{

    public Vector3 sensorDirection;
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
            incomingLight = target.transform.position - transform.parent.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            sensorDirection = (transform.forward + transform.up).normalized;
            
            incomingLight = target.transform.position - transform.parent.position;

            angle = Vector3.Angle(incomingLight, sensorDirection);

            actualAngle = angle;
            forward = transform.forward;

            Debug.DrawRay(transform.position, sensorDirection, Color.red);

            if (angle > 90)
            {
                angle = 90;
            }

            value = Mathf.Cos(Mathf.Deg2Rad * angle);
            //value = 1 - angle / 90;
        }
    }
}
