// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {

	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script
    bicycle_code bike;
    UdpControl udpControl;
    camSwitcher cameraSwitch;
    public bool headSteer = true;
    GameObject FirstView;
    float rotation;

    // Use this for initialization
    void Start () {
		ctrlHub = gameObject;//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
        bike = GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>();
        udpControl = GetComponent<UdpControl>();
        cameraSwitch = GetComponent<camSwitcher>();
        FirstView = GameObject.Find("FirstView"); 
    }
	
	// Update is called once per frame
	void Update () {

        // If haven't been controlled by udp
        if (!outsideControls.MoveByUdp)
        {
            // Accerelate
            outsideControls.Vertical = Input.GetAxis("Vertical");

            // Increase max speed
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button1))
                outsideControls.bikeSpeedKPH += 0.1f;

            // Decrease max speed
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Joystick1Button0))
                outsideControls.bikeSpeedKPH -= 0.1f;
        }


        if (!outsideControls.MoveByUdp && !headSteer)
        {
            // Turn with keyboard control
            outsideControls.Horizontal = Input.GetAxis("Horizontal");
            Debug.Log("Horizontal Axis is: " + outsideControls.Horizontal);
            Debug.Log("Input.GetAxis(Horizontal) is: " + Input.GetAxis("Horizontal"));
        }

        // steering with UDP
        if (!headSteer)
        {
            outsideControls.TurnByUdp = true;
        }
        
        // steering with only headset
        if (headSteer)
        {
            outsideControls.TurnByUdp = false;
            rotation = FirstView.GetComponent<Transform>().rotation.eulerAngles.z;

            if (rotation > 0 && rotation < 180)
            {
                rotation = rotation / 30.0f;
            }
            else if (rotation > 180 && rotation < 360)
            {
                rotation = (rotation - 360.0f) / 30.0f;
            }
            rotation = Mathf.Clamp(rotation, -1.0f, 1.0f);
            outsideControls.Horizontal = rotation * -1.0f;

        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Current is controlled by keyboard, then change to udp
            if (!outsideControls.MoveByUdp)
            {
                udpControl.LatestSpeed = 0;
                udpControl.LatestAngle = 0;
            }
            else // Current is controled by udp, then change to keyboard
            {
                udpControl.LatestSpeed = outsideControls.startSpeedKPH;
                outsideControls.bikeSpeedKPH = outsideControls.startSpeedKPH;
            }

            outsideControls.MoveByUdp = !outsideControls.MoveByUdp;
            outsideControls.TurnByUdp = !outsideControls.TurnByUdp;
        }

        // Test udp send
        if (Input.GetKeyDown(KeyCode.P))
            gameObject.GetComponent<UdpControl>().SendData(5);

        // Restart bike
        if (Input.GetKeyDown(KeyCode.R))
        {
            outsideControls.bikeSpeedKPH = 25;
            outsideControls.restartBike = true;
        }
        else
        {
            outsideControls.restartBike = false;
        }

        // RightShift for full restart
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            outsideControls.bikeSpeedKPH = outsideControls.startSpeedKPH;
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
            cameraSwitch.firstView = !cameraSwitch.firstView;

        // Exit the game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
