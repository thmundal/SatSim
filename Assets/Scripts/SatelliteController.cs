using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    SolarPanel[] solarPanels;

    // Start is called before the first frame update
    void Start()
    {
        solarPanels = GetComponentsInChildren<SolarPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform t = GetComponentInParent<Transform>();
        foreach(SolarPanel panel in solarPanels)
        {
            float adjustValue = panel.adjust_value;
            //Vector3 rotation = transform.right;
            //rotation.Scale(new Vector3(adjustValue * 90, 0, 0));

            int direction = (adjustValue > 0 ? 1 : -1);

            panel.transform.Rotate(Vector3.right * adjustValue * 1e5f);
        }
    }
}
