/// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using UnityEngine.UI;//need that for mobile controls
using UnityEngine.EventSystems;//need that for mobile controls
using System.Collections;


public class mobileControls : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {//need that for mobile controls


	public Image bgImgJoyRight; //UI object for right Joystick
	public Image bgImgJoyLeft; //UI object for left Joystick
	private Image joystickImg; //"picture" of stick itself
	private Image joystickImgLeft;//"picture" of stick itself for joyLeft
	private Vector3 inputVector; //this will be translated to bike script for bike accelerate/stop and leaning
	private Vector3 inputVectorLeft;//this will be translated to bike script for pilot's mass shift
	
	
	public bool rearBrakeOn;//this variable says to bike's script to use rear brake
	public bool restartBike;//this variable says to bike's script restart
	//public bool ESPMode;//this variable switch ESP on and off



	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script



	private void Start(){

		ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script

		joystickImg = bgImgJoyRight.transform.GetChild(0).GetComponent<Image>();//find stick
		joystickImgLeft = bgImgJoyLeft.transform.GetChild(0).GetComponent<Image>();//find stick
	}

	//Uncomment it if you want to lean by device rotation :)
	//from here---------------------------------------------------
	//private void Update(){
	//	outsideControls.Horizontal = Input.acceleration.x*1.25f;
	//}
	//to here---------------------------------------------------
	//don't forget to comment standard leaning style few strings below

	///////////////////////////////////////// Joystick Section /////////////////////////////////////////////////////////////////////////////////////
	public virtual void OnDrag(PointerEventData ped){
		Vector2 pos;//position for joystick Right
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImgJoyRight.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
			if (ped.position.x > Screen.width/2) {//use buttons should't take effect to joystics
				pos.x = (pos.x / bgImgJoyRight.rectTransform.sizeDelta.x);
				pos.y = (pos.y / bgImgJoyRight.rectTransform.sizeDelta.y);

				inputVector = new Vector3 (pos.x * 2 + 1, 0, pos.y * 2 - 1);
				inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

				//move stick
				joystickImg.rectTransform.anchoredPosition =
					new Vector3 (inputVector.x * (bgImgJoyRight.rectTransform.sizeDelta.x / 2), inputVector.z * (bgImgJoyRight.rectTransform.sizeDelta.y / 2));
			
				outsideControls.Vertical = inputVector.z;
				//Comment it if you want to lean by device rotation :)
				//from here---------------------------------------------------
				outsideControls.Horizontal = inputVector.x;
				//to here---------------------------------------------------
				//don't forget to uncomment rotate leaning style few strings above
			}
		}

		Vector2 pos_j2;//position for joystick Left
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImgJoyLeft.rectTransform, ped.position, ped.pressEventCamera, out pos_j2)){
			if (ped.position.x < Screen.width/2 && ped.position.y < 170){//use buttons should't take effect to joystics
				pos_j2.x = (pos_j2.x / bgImgJoyLeft.rectTransform.sizeDelta.x);
				pos_j2.y = (pos_j2.y / bgImgJoyLeft.rectTransform.sizeDelta.y);
			
				inputVectorLeft = new Vector3(pos_j2.x*2 + 1,0,pos_j2.y*2 - 1);
				inputVectorLeft = (inputVectorLeft.magnitude > 1.0f)?inputVectorLeft.normalized:inputVectorLeft;
			
				//move stick
				joystickImgLeft.rectTransform.anchoredPosition =
					new Vector3(inputVectorLeft.x * (bgImgJoyLeft.rectTransform.sizeDelta.x/2) ,inputVectorLeft.z * (bgImgJoyLeft.rectTransform.sizeDelta.y/2));

				outsideControls.VerticalMassShift = inputVectorLeft.z;
				outsideControls.HorizontalMassShift = inputVectorLeft.x;
			}
		}
	}

/////////////////////////////////////////// pointer function ////////////////////////////////////////////////////////////////////
	public virtual void OnPointerDown(PointerEventData ped){

		if (ped.pointerEnter.name == "Stick") OnDrag(ped);
		if (ped.pointerEnter.name == "Button_X") outsideControls.rearBrakeOn = true;
		if (ped.pointerEnter.name == "Button_R") outsideControls.restartBike = true;
		if (ped.pointerEnter.name == "Button_Rf") outsideControls.fullRestartBike = true;
		//if (ped.pointerEnter.name == "Button_E") {//we need to switch ESP on and off after each press on the ESP key
		//	if(!outsideControls.ESPMode){
		//		outsideControls.ESPMode = true;
		//	} else {
		//		outsideControls.ESPMode = false;
		//	}
		//}
		if (ped.pointerEnter.name == "Button_rev") outsideControls.reverse = true;

	}


	public virtual void OnPointerUp(PointerEventData ped){


		inputVector = Vector3.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
		inputVectorLeft = Vector3.zero;
		joystickImgLeft.rectTransform.anchoredPosition = Vector3.zero;
		outsideControls.rearBrakeOn = false;
		outsideControls.restartBike = false;
		outsideControls.fullRestartBike = false;
		outsideControls.reverse = false;

		outsideControls.Vertical = 0;
		outsideControls.Horizontal = 0;

		outsideControls.VerticalMassShift = 0;
		outsideControls.HorizontalMassShift = 0;
	}
}

