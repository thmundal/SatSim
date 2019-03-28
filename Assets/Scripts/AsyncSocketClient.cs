using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example - discarded
// https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb

public class AsyncSocketClient
{
    private const int port = 11000;
    private const string host = "localhost";

    private ManualResetEvent connectDone    = new ManualResetEvent(false);
    private ManualResetEvent sendDone       = new ManualResetEvent(false);
    private ManualResetEvent receiveDone    = new ManualResetEvent(false);
    private string response = String.Empty;
    private Socket client;

    private NetworkHandler handler;

    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    public bool connected = false;

    private Dictionary<string, Action> event_listeners;
    private bool do_disconnect = false;
    private bool running = false;

    private List<string> buffer;

    public AsyncSocketClient()
    {
        event_listeners = new Dictionary<string, Action>();
        buffer = new List<string>();
    }

    public void Connect()
    {
        StartClient();
    }

    private void StartClient()
    {
        try
        {
            running = true;
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void TryConnect()
    {
        Debug.Log("Trying to connect");
        socketConnection = new TcpClient();

        while(!connected)
        {
            try {
                socketConnection.Connect(host, port);
                connected = true;
                Debug.Log("connected");
            }
            catch (SocketException)
            {
                Debug.Log("Server not responding...");
                Thread.Sleep(1000);
            }
        }
    }

    private void ListenForData()
    {
        try
        {

            Byte[] bytes = new byte[1024];

            while(running)
            {
                TryConnect();

                if(hasListener("connect"))
                {
                    getListener("connect").Invoke();
                }

                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    while((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        string serverMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log("Received from server: " + serverMessage);
                    }
                }

                if(do_disconnect)
                {
                    return;
                }
            }

        } catch(SocketException socketException)
        {
            connected = false;
            Debug.Log(socketException);
        } catch(InvalidOperationException ie)
        {
            connected = false;
            Debug.Log("Server disconnected");
        }
    }

    public void FillSendBuffer<T>(string key, T value)
    {
        buffer.Add("\"" + key + "\":" + value.ToString().Replace(",", "."));
    }

    public void SendAvailableData()
    {
        Send("{\"available_data\": {" + string.Join(",", buffer.ToArray()) + "}}");
        buffer.Clear();
    }

    public void SendBuffer()
    {
        Send("{" + string.Join(",", buffer.ToArray()) + "}");
        buffer.Clear();
    }

    public void Send(string data)
    {
        if(socketConnection == null || !connected)
        {
            Debug.Log("not connected");
            return;
        }

        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if(stream.CanWrite)
            {
                byte[] output = Encoding.ASCII.GetBytes(data + "\n");
                stream.Write(output, 0, output.Length);
                //Debug.Log("Sent to server: " + data);
            }
        } catch(SocketException e)
        {
            Debug.Log(e);
        }
    }

    public bool hasListener(string evt)
    {
        return event_listeners.ContainsKey(evt);
    }

    public Action getListener(string evt)
    {
        if(hasListener(evt))
        {
            return event_listeners[evt];
        }

        return null;
    }

    public void On(string evt, Action cb)
    {
        event_listeners.Add(evt, cb);
    }

    public void Disconnect()
    {
        if(socketConnection != null)
        {
            do_disconnect = true;
            socketConnection.Dispose();
            connected = false;
        }

        running = false;
    }
}
