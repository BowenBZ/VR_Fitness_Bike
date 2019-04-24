// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {

	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script
    bicycle_code bike; 
    
	// Use this for initialization
	void Start () {
		ctrlHub = GameObject.FindGameObjectWithTag("manager");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
        bike = GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>();
    }
	
	// Update is called once per frame
	void Update () {
        // Accerelate
        if (!bike.rideByOutInput)
            outsideControls.Vertical = Input.GetAxis("Vertical");

        // Turn
        if (!bike.turnByOutInput)
            outsideControls.Horizontal = Input.GetAxis("Horizontal");

        // Test udp send
        if (Input.GetKeyDown(KeyCode.P))
            gameObject.GetComponent<UdpControl>().SendData(5);

        // Increase max speed
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button1))
            bike.velocityKMSet += 0.1f;

        // Decrease max speed
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Joystick1Button0))
            bike.velocityKMSet -= 0.1f;

        // Restart bike
        if (Input.GetKeyDown(KeyCode.R))
        {
            bike.velocityKMSet = 25;
            outsideControls.restartBike = true;
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            outsideControls.restartBike = false;
        }

        // RightShift for full restart
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            bike.velocityKMSet = 25;
            outsideControls.restartBike = true;
            outsideControls.fullRestartBike = true;
        }

        if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.Joystick1Button2))
        {
            outsideControls.restartBike = false;
            outsideControls.fullRestartBike = false;
        }

        // Switch view
        if (Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            GameObject.Find("CamSwitch").GetComponent<camSwitcher>().firstView =
                !GameObject.Find("CamSwitch").GetComponent<camSwitcher>().firstView;

        //////////////////////////////////// Rider's mass translate ////////////////////////////////////////////////////////////
        //this strings controls pilot's mass shift along bike(vertical)
        //      if (Input.GetKey (KeyCode.F)) {
        //	outsideControls.VerticalMassShift = outsideControls.VerticalMassShift += 0.1f;
        //	if (outsideControls.VerticalMassShift > 1.0f) outsideControls.VerticalMassShift = 1.0f;
        //}

        //if (Input.GetKey(KeyCode.V)){
        //	outsideControls.VerticalMassShift = outsideControls.VerticalMassShift -= 0.1f;
        //	if (outsideControls.VerticalMassShift < -1.0f) outsideControls.VerticalMassShift = -1.0f;
        //}
        //if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.V)) outsideControls.VerticalMassShift = 0;

        ////this strings controls pilot's mass shift across bike(horizontal)
        //if (Input.GetKey(KeyCode.E)){
        //	outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift += 0.1f;
        //	if (outsideControls.HorizontalMassShift >1.0f) outsideControls.HorizontalMassShift = 1.0f;
        //}

        //if (Input.GetKey(KeyCode.Q)){
        //	outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift -= 0.1f;
        //	if (outsideControls.HorizontalMassShift < -1.0f) outsideControls.HorizontalMassShift = -1.0f;
        //}
        //if(!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q)) outsideControls.HorizontalMassShift = 0;


        ////////////////////////////////////// Rear Brake ////////////////////////////////////////////////////////////////
        //// Rear Brake
        //if (Input.GetKey (KeyCode.X)) {
        //	outsideControls.rearBrakeOn = true;
        //}
        //      else
        //      {
        //          outsideControls.rearBrakeOn = false;
        //      }

        //// Restart & full restart
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    outsideControls.reverse = true;
        //}
        //else outsideControls.reverse = false;
        /////

    }
}
