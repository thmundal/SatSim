using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteController2 : MonoBehaviour
{
    //public Transform sun;

    private LightSensor2[] lightSensors;
    private SolarPanel2[] solarPanels;

    private AsyncSocketClient socket;

    private Rigidbody rgbd;

    private float desired_angle;

    //public bool rotateClockwise = true;

    private float lastVel = 0;

    public bool disablePID = false;
    public bool disableRotate = false;
    public bool disableSolarPanel = false;

    //public float maxAngularSpeed = 5.0f;

    private PIDController pidRotate;

    //public bool showParticles = true;

    //public GameObject[] thrusterClockwise;
    //public GameObject[] thrusterCounterClockwise;

    // Start is called before the first frame update
    void Start()
    {
        desired_angle = 0;
        lightSensors = GetComponentsInChildren<LightSensor2>();
        solarPanels = GetComponentsInChildren<SolarPanel2>();

        rgbd = GetComponent<Rigidbody>();

        //pidRotate = new PIDController(0.000001f, 0.025f, 0.0000f);
        pidRotate = new PIDController(5f, 1f, 0.01f);
        //pidRotate = new PIDController(1f, 0f, 0f);
        //pidRotate = new PIDController(1f, 0.001f, 0.001f);
        //pidRotate = new PIDController(0.001f, 0f, 0f);

        //AnimateThrusters(thrusterClockwise, false);
        //AnimateThrusters(thrusterCounterClockwise, false);

        socket = FindObjectOfType<NetworkHandler>().client;

        socket.On("connect", () =>
        {
            //socket.FillSendBuffer<string>("u","\"float\"");
            socket.FillSendBuffer<string>("actual_angle","\"float\"");
            socket.FillSendBuffer<string>("desired_angle","\"float\"");
            socket.SendAvailableData();
        });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //desired_angle = 
        Vector3 vecToSun = CalculatedSolarDir();
        Vector3 desiredSolarPanelDir = Vector3.ProjectOnPlane(vecToSun, transform.right);
        Vector3 desiredRotateDir = Vector3.ProjectOnPlane(vecToSun, transform.up);
        Debug.DrawRay(transform.position, desiredSolarPanelDir * 3, Color.magenta);
        Debug.DrawRay(transform.position, desiredRotateDir * 3, Color.green);

        //if (showParticles)
        //{
        //    AnimateThrusters(thrusterClockwise, true);
        //    AnimateThrusters(thrusterCounterClockwise, true);
        //}
        //else
        //{
        //    AnimateThrusters(thrusterClockwise, false);
        //    AnimateThrusters(thrusterCounterClockwise, false);
        //}

        //Debug.DrawRay(transform.position, (sun.position - transform.position).normalized * 5f, Color.white);

        //if (rotateClockwise)
        //{
        //    rgbd.AddRelativeTorque(transform.up, ForceMode.Acceleration);
        //}
        //else
        //{
        //    rgbd.AddRelativeTorque(-transform.up, ForceMode.Acceleration);
        //}
        if (!disableRotate)
        {
            HandleRotate(desiredRotateDir);
        }
        if (!disableSolarPanel)
        {
            HandleSolarPanels(desiredSolarPanelDir);
        }
        socket.FillSendBuffer<float>("actual_angle", Vector3.SignedAngle(transform.forward, Vector3.right, Vector3.up));
        socket.FillSendBuffer<float>("desired_angle", Vector3.SignedAngle(desiredRotateDir, Vector3.right, Vector3.up));
        socket.SendBuffer();
    }

    Vector3 CalculatedSolarDir()
    {
        Vector3 vecToSun = Vector3.zero;
        foreach (LightSensor2 s in lightSensors)
        {
            vecToSun += s.sensorDirection * s.value;
        }
        //Vector3 vecToSun = Vector3.ProjectOnPlane(solarPanel.calculated_solar_dir, transform.up).normalized * solarPanel.calculated_solar_dir.magnitude;
        //vecToSun += (edgeSensor[0].value * edgeSensor[0].forward);
        //vecToSun += (edgeSensor[1].value * edgeSensor[1].forward);
        Debug.DrawRay(transform.position, vecToSun * 20f, Color.yellow);
        return vecToSun;
    }

    void HandleSolarPanels(Vector3 desiredSolarPanelDir)
    {
        foreach (SolarPanel2 panel in solarPanels)
        {
            float current_s = Vector3.SignedAngle(desiredSolarPanelDir, panel.transform.forward, -transform.right);
            panel.transform.Rotate(Vector3.right * current_s * 0.01f);
        }
    }

    void HandleRotate(Vector3 desiredRotateDir)
    {
        float e = Vector3.SignedAngle(transform.forward, desiredRotateDir, transform.up);

        if (!disablePID)
        {
            float u = pidRotate.CalculateFixed(e);
            Accelerate(u);
        }
        else
        {
            Accelerate(e);
        }


        //if (u > 100)
        //{
        //}

        //print($"e: {e}  u: {u}");

        //socket.FillSendBuffer<float>("u", u);
        //socket.FillSendBuffer<float>("desired_angle", Vector3.SignedAngle(desiredRotateDir, Vector3.forward, Vector3.up));
        //socket.FillSendBuffer<float>("actual_angle", Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up));
        //socket.SendBuffer();


        //print((rgbd.angularVelocity.magnitude - lastVel) / Time.deltaTime);
        //lastVel = rgbd.angularVelocity.magnitude;

        //float s = (lastVel * lastVel) / (2f * 1.2f);
        //float current_s = Vector3.SignedAngle(desiredRotateDir, transform.forward, -transform.up);

        //transform.Rotate(transform.up * current_s * 0.01f);

        //if (current_s > 5)
        //{
        //    if (rgbd.angularVelocity.magnitude == 0)
        //    {
        //        rgbd.AddRelativeTorque(transform.up * 0.1f, ForceMode.VelocityChange);
        //    }
        //}
        //else if (rgbd.angularVelocity.magnitude != 0)
        //{
        //    rgbd.AddRelativeTorque(-transform.up * 0.1f, ForceMode.VelocityChange);
        //}

        //if (s > current_s)
        //{
        //    rgbd.AddRelativeTorque(transform.up, ForceMode.Acceleration);
        //    print("Clockwise");
        //}
        //else
        //{
        //    if (lastVel < 1.2f * Time.deltaTime)
        //    {
        //        rgbd.angularVelocity = Vector3.zero;
        //        print("Do nothing  |  " + s + "  |  " + current_s);
        //    }
        //    else
        //    {
        //        rgbd.AddRelativeTorque(-transform.up, ForceMode.Acceleration);
        //        print("counter");
        //    }
        //}
    }

    void Accelerate(float angle)
    {
        if (angle < 0)
        {
            //if (rgbd.angularVelocity.magnitude < maxAngularSpeed)
            //{
                rgbd.AddTorque(transform.up * angle, ForceMode.Acceleration);
                //AnimateThrusters(thrusterClockwise, true);
                //AnimateThrusters(thrusterCounterClockwise, false);
            //}
            //else
            //{
                //AnimateThrusters(thrusterClockwise, false);
                //AnimateThrusters(thrusterCounterClockwise, false);
            //}
        }
        else
        {
            //if (rgbd.angularVelocity.magnitude > -maxAngularSpeed)
            //{
                rgbd.AddTorque(transform.up * angle, ForceMode.Acceleration);
                //AnimateThrusters(thrusterCounterClockwise, true);
                //AnimateThrusters(thrusterClockwise, false);
            //}
            //else
            //{
                //AnimateThrusters(thrusterClockwise, false);
                //AnimateThrusters(thrusterCounterClockwise, false);
            //}
        }
    }

    void AnimateThrusters(GameObject[] thrusters, bool state)
    {
        foreach (GameObject g in thrusters)
        {
            g.SetActive(state);
        }
    }
}
