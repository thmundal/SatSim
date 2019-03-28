using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public LightSensor sensor_front_top;
    public LightSensor sensor_front_bot;
    public LightSensor sensor_back_top;
    public LightSensor sensor_back_bot;

    List<LightSensor> light_sensors;
    public Vector3 calculated_solar_dir = Vector3.zero;

    public float adjust_value = 0;

    public Transform zero_position;
    public float current_offset_angle = 0f;
    public float desired_position = 0;

    [Range(0, 0.1f)]
    public float rotationSpeed = 0.02f;

    private PIDController pid;
    private SatelliteController satController;

    private Transform sun;

    public float angleError = 0.0f;
    public Vector3 solarDir;

    // Start is called before the first frame update
    void Start()
    {
        light_sensors = new List<LightSensor>(4)
        {
            sensor_front_top,
            sensor_front_bot,
            sensor_back_top,
            sensor_back_bot
        };
        calculated_solar_dir = new Vector3();

        //pid = new PIDController(0.1f, 0.0001f, 0.01f);
        //pid = new PIDController(0.009f, 0.00011f, 0.0000f);
        pid = new PIDController(0.1f, 0, 0);
        satController = FindObjectOfType<SatelliteController>();

        sun = GameObject.FindGameObjectWithTag("sun").GetComponent<Transform>();
        zero_position = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        calculated_solar_dir = Vector3.zero;
        float strength = 0;
        foreach (LightSensor sensor in light_sensors)
        {
            calculated_solar_dir += sensor.value * sensor.sensorDirection;
            strength += sensor.value;
        }

        // If no sensors detect any light, make no adjustments.
        if (calculated_solar_dir == Vector3.zero)
        {
            adjust_value = 0;
        }
        else
        {
            //Debug.DrawRay(transform.position, calculated_solar_dir, Color.magenta);
            //Debug.DrawRay(transform.position, transform.forward, Color.green);
            DetermineAdjustValue();
        }
    }

    private void DetermineAdjustValue()
    {
        float solar_angle = Vector3.SignedAngle(zero_position.forward, calculated_solar_dir, transform.right);
        //float solar_angle = desired_position;
        current_offset_angle = Vector3.SignedAngle(zero_position.forward, transform.forward, transform.right);

        //desired_position = Vector3.SignedAngle(resting_pos.forward, sun.position - resting_pos.position, transform.right);
        //desired_position = Vector3.SignedAngle(resting_pos.forward, calculated_solar_dir, transform.right);
        //desired_position = solar_angle;
        //solar_angle - current_offset_angle

        //float angleAdjust = angleError;

        //Debug.Log("angleAdjust:" + angleAdjust);
        //Debug.Log("current_offset_angle" + current_offset_angle);
        
        Debug.DrawRay(transform.position, calculated_solar_dir, Color.magenta);

        //angleAdjust += current_offset_angle;
        //if (angleAdjust > 180f)
        //{
        //    angleAdjust -= 360f;
        //}
        //else if (angleAdjust < -180f)
        //{
        //    angleAdjust += 360f;
        //}

        //angleAdjust = ((angleAdjust + 180) % 360) - 180;

        if (!satController.disablePID && !satController.disable)
        {
            float e = solar_angle - current_offset_angle;
            //float e = angleAdjust;
            var u = pid.Calculate(e);
            adjust_value = u;
        } else
        {
            adjust_value = (solar_angle - current_offset_angle) * rotationSpeed;
            //adjust_value = (angleAdjust) * rotationSpeed;

        }
    }
}
 