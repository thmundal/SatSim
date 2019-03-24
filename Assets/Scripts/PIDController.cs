using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController
{
    private float ePrev;
    private float eInt;
    private float Kp;
    private float Kd;
    private float Ki;

    public PIDController(float Kp, float Kd, float Ki)
    {
        ePrev = 0;
        eInt = 0;
        this.Kp = Kp;
        this.Kd = Kd;
        this.Ki = Ki;
    }

    public float Calculate(float e)
    {
        if (Time.deltaTime == 0f)
        {
            return 0f;
        }
        float eDet = (e - ePrev) / Time.deltaTime;
        eInt += e * Time.deltaTime;
        float u = (Kp * e) + (Kd * eDet) + (Ki * eInt);
        ePrev = e;
        return u;
    }

    public float CalculateFixed(float e)
    {
        if (Time.fixedDeltaTime == 0f)
        {
            return 0f;
        }
        float eDet = (e - ePrev) / Time.fixedDeltaTime;
        eInt += e * Time.fixedDeltaTime;
        float u = (Kp * e) + (Kd * eDet) + (Ki * eInt);
        ePrev = e;
        return u;
    }
}
