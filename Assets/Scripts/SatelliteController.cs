using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    SolarPanel[] solarPanels;
    LogWriter[] logWriters;
    private float start_time;
    private AsyncSocketClient socket;

    private float elapsed_delta = 0;
    public bool disable = false;

    public bool disablePID = false;
    GameObject sun;

    // Start is called before the first frame update
    void Start()
    {
        solarPanels = GetComponentsInChildren<SolarPanel>();
        logWriters = new LogWriter[solarPanels.Length];

        for(int i=0; i<logWriters.Length; i++)
        {
            logWriters[i] = new LogWriter("panel_" + i + ".csv");

            if(logWriters[i].Length() == 0)
            {
                logWriters[i].WriteLine("time,value");
            }
        }

        socket = FindObjectOfType<NetworkHandler>().client;

        socket.On("connect", () =>
        {
            string[] available_data = new string[solarPanels.Length];
            for(int i=0; i<solarPanels.Length; i++)
            {
                available_data[i] = "\"adjust_value_"+i+"\":\"float\", \"actual_angle_"+i+"\":\"float\", \"desired_position_"+i+"\":\"float\", \"actual_solar_angle_"+i+"\":\"float\", \"AngleError_"+i+"\":\"float\"";
            }
            socket.Send("{\"available_data\": {"+ string.Join(",", available_data) +"}}");
        });

        socket.On("message", () =>
        {

        });
        start_time = Time.time;

        sun = GameObject.FindGameObjectWithTag("sun");
    }

    // Update is called once per frame
    void Update()
    {
        Transform t = GetComponentInParent<Transform>();

        for(int i=0; i<solarPanels.Length; i++)
        {
            SolarPanel panel = solarPanels[i];
            float adjustValue = panel.adjust_value;

            if(!disable)
            {
                panel.transform.Rotate(Vector3.right * adjustValue);
            }

            float elapsed_time = Time.time - start_time;
            elapsed_delta += Time.deltaTime;

            if(socket.connected && elapsed_delta > 0.05)
            {
                //float solar_angle     = Vector3.SignedAngle(panel.resting_pos.forward, panel.calculated_solar_dir,                        transform.right);
                float actualSolarAngle  = Vector3.SignedAngle(panel.zero_position.forward, sun.transform.position - panel.transform.position, transform.right);

                elapsed_delta = 0;
                //string msg = "{ \"adjust_value\": { \"time\":" + elapsed_time.ToString().Replace(",", ".") + ", \"value\": "+ adjustValue.ToString().Replace(",", ".")+"} }";
                string msg = "{ \"adjust_value_"+i+"\": "+ adjustValue.ToString().Replace(",", ".")+" , \"actual_angle_"+i+"\": "+ panel.current_offset_angle.ToString().Replace(",", ".") + ", \"desired_position_"+i+"\": "+panel.desired_position.ToString().Replace(",", ".")+", \"actual_solar_angle_"+i+"\": "+actualSolarAngle.ToString().Replace(",", ".")+", \"AngleError_"+i+"\":"+panel.angleError.ToString().Replace(",", ".")+"}";

                socket.Send(msg);
            }
            logWriters[i].WriteLine(elapsed_time.ToString().Replace(",", ".") + "," + adjustValue.ToString().Replace(",", "."));
        }
    }

    private void OnDestroy()
    {
        socket.Disconnect();
    }
}
