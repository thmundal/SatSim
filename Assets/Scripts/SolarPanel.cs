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
    Vector3 calculated_solar_dir;

    public float adjust_value = 0;

    public Transform resting_pos;
    public float current_offset_angle = 0f;
    public float desired_position = 0;

    [Range(0, 0.1f)]
    public float rotationSpeed = 0.02f;

    private PIDController pid;
    private SatelliteController satController;

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
        pid = new PIDController(0.1f, 0.015f, 0.001f);
        //pid = new PIDController(1f, 0, 0);
        satController = FindObjectOfType<SatelliteController>();
    }

    // Update is called once per frame
    void Update()
    {
        calculated_solar_dir = Vector3.zero;
        foreach (LightSensor sensor in light_sensors)
        {
            calculated_solar_dir += sensor.value * sensor.sensorDirection;
        }
        // If no sensors detect any light, make no adjustments.
        if (calculated_solar_dir == Vector3.zero)
        {
            adjust_value = 0;
        }
        else
        {
            Debug.DrawRay(transform.position, calculated_solar_dir, Color.magenta);
            Debug.DrawRay(transform.position, transform.forward, Color.green);
            DetermineAdjustValue();
        }
    }

    private void DetermineAdjustValue()
    {
        float solar_angle = Vector3.SignedAngle(resting_pos.forward, calculated_solar_dir, transform.right);
        current_offset_angle = Vector3.SignedAngle(resting_pos.forward, transform.forward, transform.right);

        desired_position = solar_angle;

        if (!satController.disablePID && !satController.disable)
        {
            float e = solar_angle - current_offset_angle;
            var u = pid.Calculate(e);
            adjust_value = u;
        } else
        {
            adjust_value = (solar_angle - current_offset_angle) * rotationSpeed;
        }
    }
}
