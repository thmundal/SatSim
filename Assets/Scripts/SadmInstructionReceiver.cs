using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SadmInstructionReceiver : MonoBehaviour
{
    [Range(-180, 180)]
    public float rotationAngle;

    public Transform sadmShaft;

    private float rotateIterator =  0;
    private float rotationSpeed = 5.0f;

    [Range(-180, 180)]
    public float targetAngle;

    private AsyncSocketClient socket;

    private bool instruction_running = false;
    private bool should_notify_complete = false;

    private struct Instruction
    {
        public string request;
        public float degrees;
    }

    // Start is called before the first frame update
    void Start()
    {
        socket = FindObjectOfType<NetworkHandler>().client;

        socket.OnMessageReceived((msg) =>
        {
            Instruction instruction = JsonConvert.DeserializeObject<Instruction>(msg);

            Debug.Log(msg);
            if(instruction.request == "rotate")
            {
                Debug.Log(instruction.degrees);
                //targetAngle = instruction.degrees;
                RotateTo(instruction.degrees);
            }
        });

        socket.On("connect", () =>
        {
            socket.FillSendBuffer<string>("rotate", "{\"degrees\":\"float\"}");
            socket.SendAvailableInstructions();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(rotateIterator != 0)
        {
            rotateIterator += -1 * Mathf.Sign(targetAngle);

            rotationAngle += rotateIterator * Time.deltaTime * rotationSpeed;

            if(rotateIterator == 0)
            {
                should_notify_complete = true;
            }
        }

        if (instruction_running && should_notify_complete)
        {
            socket.FillSendBuffer<string>("instruction_complete", "\"rotate\"");
            socket.SendBuffer();
            instruction_running = false;
            should_notify_complete = false;
        }

        //Vector3 e = sadmShaft.rotation.eulerAngles;
        //Vector3 targetRot = new Vector3(targetAngle, 0, 0);
        //float diff = Vector3.Distance(e, targetRot);

        //if(diff <= rotationSpeed)
        //{
        //    Debug.Log("Spot on");
        //}

        //sadmShaft.rotation = Quaternion.Lerp(sadmShaft.rotation, Quaternion.Euler(targetAngle, 0, 0),  Time.deltaTime);
        sadmShaft.rotation = Quaternion.Euler(rotationAngle, 0, 0);
    }

    void RotateTo(float target)
    {
        targetAngle = target;
        rotateIterator = targetAngle;
        instruction_running = true;
    }
}
