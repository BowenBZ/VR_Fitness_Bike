// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {



	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script
	


	// Use this for initialization
	void Start () {
		ctrlHub = GameObject.FindGameObjectWithTag("manager");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
	}
	
	// Update is called once per frame
	void Update () {
        //////////////////////////////////// ACCELERATE, braking & 'full throttle - manual trick' //////////////////////////////////////////////
        // Alpha2 is key "2".Used to make manual.Also, it can be achived by 100 % "throtle on mobile joystick"

        if (!Input.GetKey(KeyCode.Alpha2))
        {
            outsideControls.Vertical = Input.GetAxis("Vertical") / 1.112f;//to get less than 0.9 as acceleration to prevent wheelie(wheelie begins at >0.9
            if (Input.GetAxis("Vertical") < 0) outsideControls.Vertical = outsideControls.Vertical * 1.112f;//need to get 1(full power) for front brake
        }

        //////////////////////////////////// STEERING /////////////////////////////////////////////////////////////////////////
        outsideControls.Horizontal = Input.GetAxis("Horizontal");

		if (Input.GetKey (KeyCode.Alpha2)) outsideControls.Vertical = 1;

        //if (Input.GetKey(KeyCode.W))
        //{
        //    GameObject.FindWithTag("bike").GetComponent<bicycle_code>().moving = true;
        //}

        //if (Input.GetKeyUp(KeyCode.W))
        //{
        //    GameObject.FindWithTag("bike").GetComponent<bicycle_code>().moving = false;
        //}

        Debug.Log(Input.GetKeyDown(KeyCode.Joystick1Button1));

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button1))
            if (GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet > 50)
                GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet = 50;
            else
                GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet += 0.1f;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.Joystick1Button0))
            if (GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet <= 5)
                GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet = 5;
            else
                GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet -= 0.1f;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().wheelAngle -= 5;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().wheelAngle += 5;

        //////////////////////////////////// Rider's mass translate ////////////////////////////////////////////////////////////
        //this strings controls pilot's mass shift along bike(vertical)
        if (Input.GetKey (KeyCode.F)) {
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift += 0.1f;
			if (outsideControls.VerticalMassShift > 1.0f) outsideControls.VerticalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.V)){
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift -= 0.1f;
			if (outsideControls.VerticalMassShift < -1.0f) outsideControls.VerticalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.V)) outsideControls.VerticalMassShift = 0;

		//this strings controls pilot's mass shift across bike(horizontal)
		if (Input.GetKey(KeyCode.E)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift += 0.1f;
			if (outsideControls.HorizontalMassShift >1.0f) outsideControls.HorizontalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.Q)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift -= 0.1f;
			if (outsideControls.HorizontalMassShift < -1.0f) outsideControls.HorizontalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q)) outsideControls.HorizontalMassShift = 0;


		//////////////////////////////////// Rear Brake ////////////////////////////////////////////////////////////////
		// Rear Brake
		if (Input.GetKey (KeyCode.X)) {
			outsideControls.rearBrakeOn = true;
		} else
			outsideControls.rearBrakeOn = false;

		//////////////////////////////////// Restart ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if (Input.GetKey (KeyCode.R)) {
			outsideControls.restartBike = true;
		} else
			outsideControls.restartBike = false;

		// RightShift for full restart
		if (Input.GetKey (KeyCode.RightShift)) {
			outsideControls.fullRestartBike = true;
		} else
			outsideControls.fullRestartBike = false;

        // RightShift for full restart
        if (Input.GetKey(KeyCode.Joystick1Button2))
        {
            GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>().velocityKMSet = 25;
            outsideControls.restartBike = true;
            outsideControls.fullRestartBike = true;
        }
        else
        {
            outsideControls.restartBike = false;
            outsideControls.fullRestartBike = false;
        }
            

        //////////////////////////////////// Reverse ////////////////////////////////////////////////////////////////
        // Restart & full restart
        if (Input.GetKeyDown(KeyCode.C)){
				outsideControls.reverse = true;
		} else outsideControls.reverse = false;
		///
	}
}
