using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    public AsyncSocketClient client = new AsyncSocketClient();
    // Start is called before the first frame update
    void Start()
    {
        client.Connect();
    }

    public void ReceiveData(string data)
    {
        Debug.Log("GameObject received " + data);
    }

    public void Send(string data)
    {
        client.Send(data + "\n");
    }

    public bool Connected()
    {
        return client.connected;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Application.isPlaying)
        {
            client.Disconnect();
        }
    }
}
