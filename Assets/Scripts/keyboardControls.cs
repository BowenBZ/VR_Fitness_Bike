// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class keyboardControls : MonoBehaviour {

	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script
    bicycle_code bike;
    UdpControl udpControl;
    camSwitcher cameraSwitch;

    // Use this for initialization
    void Start () {
		ctrlHub = gameObject;//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
        bike = GameObject.FindGameObjectWithTag("bike").GetComponent<bicycle_code>();
        udpControl = GetComponent<UdpControl>();
        cameraSwitch = GetComponent<camSwitcher>();
    }
	
	// Update is called once per frame
	void Update () {

        // If haven't been controled by udp
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

        if (!outsideControls.TurnByUdp)
        {
            // Turn
            outsideControls.Horizontal = Input.GetAxis("Horizontal");
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
