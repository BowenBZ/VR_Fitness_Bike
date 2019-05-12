// Writen by Boris Chuprin smokerr@mail.ru
// Modified by Bowen Zhang
using UnityEngine;
using System.Collections;

//this is script contains all variables translated to bike's script.
//so, it's just a holder for all control variables
//mobile/keyboard scripts sends nums(float, int, bools) to this one

public class controlHub : MonoBehaviour
{
    public float Vertical { get; set; }//variable translated to bike script for bike accelerate/stop and 
    public float Horizontal { get; set; }//variable translated to bike script for pilot's mass 
    public float VerticalMassShift { get; set; }//variable for pilot's mass translate along     
    public float HorizontalMassShift { get; set; }//variable for pilot's mass translate across bike

    public bool rearBrakeOn { get; set; }//this variable says to bike's script to use rear 
    public bool restartBike { get; set; }//this variable says to bike's script 
    public bool fullRestartBike { get; set; } //this variable says to bike's script to full 
    public bool reverse { get; set; }//for reverse speed

    public float startSpeedKPH;
    public bool MoveByUdp { get; set; } // to detect wether the bike is controled by out input
    public bool TurnByUdp { get; set; } // to detect wether the bike is controled by out input
    public float bikeSpeedKPH { get; set; } // Variable to control the speed of the bike
    public float wheelAngle { get; set; }// The fixed angle turns
    UdpControl udpControl;

    public string CurrentBlock { get; set; } // The name of current name road
    public int currentSignUser { get; set; } // Current triggerd sign that user passed by
    public int currentSignAI { get; set; } // Current triggered sign that AI passed by


    void Start()
    {
        MoveByUdp = false;
        TurnByUdp = false;
        bikeSpeedKPH = startSpeedKPH;
        udpControl = transform.GetComponent<UdpControl>();
        currentSignUser = 0;
        CurrentBlock = "Block_0";
        currentSignUser = 0;
        currentSignAI = 0;
    }

    void Update()
    {
        if (MoveByUdp)
        {
            bikeSpeedKPH = udpControl.LatestSpeed;
            Vertical = (bikeSpeedKPH > 0) ? 0.9f : 0;
        }
        if (TurnByUdp)
        {
            wheelAngle = udpControl.LatestAngle;
            Horizontal = (wheelAngle == 0) ? 0 : wheelAngle / Mathf.Abs(wheelAngle);
        }
    }
}