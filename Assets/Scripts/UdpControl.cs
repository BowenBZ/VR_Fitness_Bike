using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UdpControl : MonoBehaviour
{
    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    bicycle_code bike;

    // public
    // public string IP = "127.0.0.1"; default local
    public int vrPort; // define > init

    class PiData
    {
        public float speed;
        public float angle;
    }


    // start from unity3d
    public void Start()
    {
        init();
        bike = GameObject.FindWithTag("bike").GetComponent<bicycle_code>();
    }

    // init
    private void init()
    { 
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    void ReceiveData()
    {
        client = new UdpClient(vrPort);
        while (true)
        {
            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                PiData piData = new PiData();
                piData = JsonUtility.FromJson<PiData>(text);

                // Show the result
                //Debug.Log("speed: " + piData.speed);
                //Debug.Log("angle: " + piData.angle);

                bike.Ride(piData.speed);
                bike.Turn(piData.angle);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}