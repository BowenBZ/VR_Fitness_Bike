// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

//this is script contains all variables translated to bike's script.
//so, it's just a holder for all control variables
//mobile/keyboard scripts sends nums(float, int, bools) to this one

public class controlHub : MonoBehaviour  {//need that for mobile controls

    public float Vertical { get; set; }//variable translated to bike script for bike accelerate/stop and 
    public float Horizontal { get; set; }//variable translated to bike script for pilot's mass 
    public float VerticalMassShift { get; set; }//variable for pilot's mass translate along     
    public float HorizontalMassShift { get; set; }//variable for pilot's mass translate across bike

    public bool rearBrakeOn { get; set; }//this variable says to bike's script to use rear 
    public bool restartBike { get; set; }//this variable says to bike's script 
    public bool fullRestartBike { get; set; } //this variable says to bike's script to full 
    public bool reverse { get; set; }//for reverse speed

    public float initialSpeedKM;
    public bool MoveByUdp { get; set; } // to detect wether the bike is controled by out input
    public bool TurnByUdp { get; set; } // to detect wether the bike is controled by out input
    public float VelocityKMSet { get; set; } // Variable to control the speed of the bike
    public float WheelAngle { get; set; }// The fixed angle turns

    public string CurrentRoad { get; set; } // The name of current name road

    UdpControl udpControl;

    void Start()
    {
        MoveByUdp = false;
        TurnByUdp = false;
        VelocityKMSet = initialSpeedKM;
        udpControl = GameObject.FindGameObjectWithTag("manager").GetComponent<UdpControl>();
    }

    void Update()
    {
        if (MoveByUdp)
        {
            VelocityKMSet = udpControl.LatestSpeed;
            Vertical = (VelocityKMSet > 0) ? 0.9f : 0;
        }   
        if (TurnByUdp)
        {
            WheelAngle = udpControl.LatestAngle;
            Horizontal = (WheelAngle == 0) ? 0 : WheelAngle / Mathf.Abs(WheelAngle);
        }
    }
}