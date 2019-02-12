using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public LightSensor sensor_front_top;
    public LightSensor sensor_front_bot;
    public LightSensor sensor_back_top;
    public LightSensor sensor_back_bot;

    public float adjust_value = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(sensor_front_bot.value == 0 && sensor_front_top.value == 0)
        {
            adjust_value = sensor_back_bot.value - sensor_back_top.value;
        }
        else
        {
            adjust_value = sensor_front_top.value - sensor_front_bot.value;
        }
    }
}
