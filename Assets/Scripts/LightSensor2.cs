using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor2 : MonoBehaviour
{
    public Vector3 sensorDirection;
    public float value;

    private Transform target;
    private Vector3 incomingLight;


    // Start is called before the first frame update
    void Start()
    {
        sensorDirection = transform.forward;

        target = GameObject.FindGameObjectWithTag("sun").transform;

        if (target != null)
        {
            incomingLight = target.transform.position - transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            sensorDirection = transform.forward;
            incomingLight = target.transform.position - transform.position;

            float angle = Vector3.Angle(incomingLight, sensorDirection);

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
