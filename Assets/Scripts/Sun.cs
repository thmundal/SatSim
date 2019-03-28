using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    GameObject satellite;
    Bounds satBounds;
    // Start is called before the first frame update
    void Start()
    {
        satellite = GameObject.FindGameObjectWithTag("sat");
        satBounds = satellite.GetComponent<MeshRenderer>().bounds;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float width = (satBounds.size.x + 8) * 4;
        float height = satBounds.size.y * 4;

        for(int x = 0; x<width; x++)
        {
            for(int y = 0; y<height; y++)
            {
                Vector3 offset = new Vector3(x - (width / 2), y - (height / 2), 0);

                Vector3 rayPos =  transform.position - offset;
                Vector3 targetPos = satellite.transform.position - offset;
                Vector3 rayDirection = targetPos - rayPos;

                //Debug.DrawRay(rayPos, targetPos - rayPos, Color.white);

                bool panel_hit = false;

                if (Physics.Raycast(rayPos, targetPos - rayPos, out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject hittedObject = hit.collider.transform.gameObject;
                    SolarPanel panel = hittedObject.GetComponent<SolarPanel>();

                    if (panel)
                    {
                        Debug.DrawRay(rayPos, targetPos - rayPos, Color.yellow);

                        Vector3 hitNormal = hit.normal;
                        if(hitNormal == hittedObject.transform.forward || hitNormal == -hittedObject.transform.forward)
                        {
                            if(hitNormal == -hittedObject.transform.forward)
                            {
                                hitNormal *= -1;
                            }
                            Vector3 calculated_solar_dir = Vector3.ProjectOnPlane(-rayDirection, hittedObject.transform.right);
                            float angleError = Vector3.SignedAngle(hitNormal, Vector3.ProjectOnPlane(-rayDirection, hittedObject.transform.right), hittedObject.transform.right);
                            float targetPosition = -Vector3.SignedAngle(panel.zero_position.forward, calculated_solar_dir, transform.right);
                           
                            panel.angleError = angleError;
                            panel.calculated_solar_dir = calculated_solar_dir;
                            panel.desired_position = targetPosition;

                            Debug.DrawRay(hit.point, hitNormal);
                            panel_hit = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
