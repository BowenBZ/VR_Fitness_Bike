using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UdpControl : MonoBehaviour
{
    // Receive 
    Thread receiveThread; // receiving 
    UdpClient receiveClient;  // udpclient object
    // public string IP = "127.0.0.1"; default local
    int vrPort = 8888; // receive port

    // Send
    public string piIP;  // define in init
    int piPort = 8888;  // define in init
    IPEndPoint remoteEndPoint;
    UdpClient sendClient;

    // Bike
    bicycle_code bike;

    // Last receive speed
    public float LatestSpeed { get; set; }
    // Last receive angle
    public float LatestAngle { get; set; }

    class PiData
    {
        public float speed;
        public float angle;
    }

    class VrData
    {
        public int resistanceLevel;
    }


    void Start()
    {
        LatestSpeed = 0;
        LatestAngle = 0;
        bike = GameObject.FindWithTag("bike").GetComponent<bicycle_code>();
        init();
    }

    // init
    private void init()
    {
        // Receive
        receiveClient = new UdpClient(vrPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Send
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(piIP), piPort);
        sendClient = new UdpClient();
    }

    // receive thread
    void ReceiveData()
    {
        //receiveClient = new UdpClient(vrPort);
        while (true)
        {
            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = receiveClient.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                PiData piData = new PiData();
                piData = JsonUtility.FromJson<PiData>(text);

                LatestSpeed = piData.speed;
                LatestAngle = -1 * piData.angle;

                //Show the result
                //Debug.Log("speed: " + piData.speed);
                //Debug.Log("angle: " + piData.angle);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // send data
    public void SendData(int level)
    {
        try
        {
            VrData vrData = new VrData();
            vrData.resistanceLevel = level;
            string text = JsonUtility.ToJson(vrData);
            byte[] data = Encoding.UTF8.GetBytes(text);
               
            // Send it
            sendClient.Send(data, data.Length, remoteEndPoint);

            Debug.Log("send");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}