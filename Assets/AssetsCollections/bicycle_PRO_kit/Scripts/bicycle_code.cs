/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System.Collections;

public class bicycle_code : MonoBehaviour
{
    ///////////////////////////////////////////////////////// wheels ///////////////////////////////////////////////////////////
    // defeine wheel colliders
    public WheelCollider coll_frontWheel;
    public WheelCollider coll_rearWheel;
    // visual for wheels
    public GameObject meshFrontWheel;
    public GameObject meshRearWheel;
    // check isn't front wheel in air for front braking possibility
    bool isFrontWheelInAir = true;

    //////////////////////////////////////// Stifness, CoM(center of mass), crahsed /////////////////////////////////////////////////////////////
    //for stiffness counting when rear brake is on. Need that to lose real wheel's stiffness during time
    float stiffPowerGain = 0.0f;
    //for CoM moving along and across bike. Pilot's CoM.
    float tmpMassShift = 0.0f;

    // define CoM of bike
    public Transform CoM; //CoM object
    float normalCoM = -0.84f; //normalCoM is for situation when script need to return CoM in starting position										
    float CoMWhenCrahsed = -0.2f; //we beed lift CoM for funny bike turning around when crahsed													


    //////////////////// "beauties" of visuals - some meshes for display visual parts of bike ////////////////////////////////////////////
    public Transform rearPendulumn; //rear pendulumn
    public Transform steeringWheel; //wheel bar
    public Transform suspensionFront_down; //lower part of front forge
    private int normalFrontSuspSpring; // we need to declare it to know what is normal front spring state is
    private int normalRearSuspSpring; // we need to declare it to know what is normal rear spring state is
    private bool forgeBlocked = true; // variable to lock front forge for front braking
                                      //why we need forgeBlocked ?
                                      //There is little bug in PhysX 3.3f wheelCollider - it works well only with car mass of 1600kg and 4 wheels.
                                      //If your vehicle is not 4 wheels or mass is not 1600 but 400 - you are in troube.
                                      //Problem is absolutely epic force created by suspension spring when it's full compressed, stretched or wheel comes underground between frames(most catastrophic situation)
                                      //In all 3 cases your spring will generate crazy force and push rigidbody to the sky.
                                      //so, my solution is find this moment and make spring weaker for a while then return to it's normal condition.

    private float baseDistance; // need to know distance between wheels - base. It's for wheelie compensate(dont want wheelie for long bikes)

    // we need to clamp wheelbar angle according the speed. it means - the faster bike rides the less angle you can rotate wheel bar
    public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f), new Keyframe(100f, 1f));//first number in Keyframe is speed, second is max wheelbar degree

    // temporary variable to restrict wheel angle according speed
    private float tempMaxwheelAngle;

    //for wheels vusials match up the wheelColliders
    private Vector3 wheelCCenter;
    private RaycastHit hit;

    /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
    [HideInInspector]
    public float frontBrakePower = 25; //brake power absract - 100 is good brakes																		
    float airRes; //Air resistant

    /////////////////////////////////////////////////// BICYCLE CODE ///////////////////////////////////////////////////////
    [HideInInspector]
    public float frontWheelAPD;// usualy 0.05f

    private GameObject pedals;
    private pedalControls linkToStunt;
    private bool rearPend;



    [HideInInspector]
    public bool isReverseOn = false; //to turn On and Off reverse speed

    // Key control
    GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's script
    public float initialForce; // The initial torque added to the wheel when start
    [HideInInspector]
    public float bikeSpeed; // speed in km/h, which will be removed in the future

    public float bikeSpeedKPH { get; set; } // bike speed in km/h
    public float bikeSpeedMPS { get; set; } // bike speed in m/s
    public float bikeSpeedCPS { get; set; } // bike speed in cycle/s
    public float bikeSpeedCPM { get; set; } // bike speed in cycle/min
    float wheelRadius;     // The radius of wheel

    // ON SCREEN INFO 
    void OnGUI()
    {
        GUIStyle biggerText = new GUIStyle("label");
        biggerText.fontSize = 40;
        GUIStyle middleText = new GUIStyle("label");
        middleText.fontSize = 22;
        GUIStyle smallerText = new GUIStyle("label");
        smallerText.fontSize = 14;

        //to show speed on display interface
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width * 0.875f, Screen.height * 0.9f, 120, 80), string.Format("" + "{0:0.}", bikeSpeedKPH), biggerText);

        if (!isReverseOn)
        {
            GUI.color = Color.grey;
            GUI.Label(new Rect(Screen.width * 0.885f, Screen.height * 0.96f, 60, 40), "KM/H", smallerText);
        }
        else
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width * 0.885f, Screen.height * 0.96f, 60, 40), "KM/H", smallerText);
        }

        // user info help lines
        GUI.color = Color.white;
        string mode = (outsideControls.MoveByUdp) ? "UDP" : "Keyboard";
        GUI.Box(new Rect(Screen.width * 0.885f, 10, 180, 40), mode, middleText);

        //GUI.Box(new Rect(10, 40, 120, 20), "X - rear brake", smallerText);
        //GUI.Box(new Rect(10, 55, 320, 20), "Q,E,F,V - shift center of mass of biker", smallerText);
        //GUI.Box(new Rect(10, 70, 320, 20), "R - restart / RightShift+R - full restart", smallerText);
        //GUI.Box(new Rect(10, 85, 180, 20), "RMB - rotate camera around", smallerText);
        //GUI.Box(new Rect(10, 115, 320, 20), "C - toggle reverse", smallerText);

        //GUI.Box(new Rect(10, 130, 320, 20), "Space - bunnyhop", smallerText);
        //GUI.Box(new Rect(10, 145, 320, 20), "M - turn left 180", smallerText);
        //GUI.Box(new Rect(10, 160, 320, 20), "N - backflip 360", smallerText);
        //GUI.Box(new Rect(10, 175, 220, 20), "2 - manual", smallerText);
        //GUI.Box(new Rect(10, 190, 220, 20), "B - bunny jump right", smallerText);
        //GUI.Box(new Rect(10, 205, 220, 20), "/ - 1 hard clutch for half second", smallerText);


        //GUI.Box(new Rect(10, 220, 320, 20), "Esc - return to main menu", smallerText);
        //GUI.color = Color.black;
    }
    void Start()
    {

        //if there is no pendulum linked to script in Editor, it means MTB have no rear suspension, so no movement of rear wheel(pendulum)
        if (rearPendulumn)
        {
            rearPend = true;
        }
        else rearPend = false;

        //bicycle code
        frontWheelAPD = coll_frontWheel.forceAppPointDistance;

        ctrlHub = GameObject.FindGameObjectWithTag("manager");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        linkToStunt = GetComponentInChildren<pedalControls>();
        pedals = linkToStunt.gameObject;

        Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass
        GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
        GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass

        // wheel colors for understanding of accelerate, idle, brake(white is idle status)
        meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
        meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

        //for better physics of fast moving bodies
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

        // too keep LegsPower variable like "real" horse powers
        //LegsPower = 20;
        //LegsPower = LegsPower * 20;

        //*30 is for good braking to keep frontBrakePower = 100 for good brakes. So, 100 is like sportsbike's Brembo
        frontBrakePower = 25;
        frontBrakePower = frontBrakePower * 30;//30 is abstract but necessary for Unity5

        //tehcnical variables
        normalRearSuspSpring = (int)coll_rearWheel.suspensionSpring.spring;
        normalFrontSuspSpring = (int)coll_frontWheel.suspensionSpring.spring;

        baseDistance = coll_frontWheel.transform.localPosition.z - coll_rearWheel.transform.localPosition.z;// now we know distance between two wheels

        var tmpMeshRWh01 = meshRearWheel.transform.localPosition;
        tmpMeshRWh01.y = meshRearWheel.transform.localPosition.y - coll_rearWheel.suspensionDistance / 4;
        meshRearWheel.transform.localPosition = tmpMeshRWh01;


        //and bike's frame direction
        var tmpCollRW01 = coll_rearWheel.transform.localPosition;
        tmpCollRW01.y = coll_rearWheel.transform.localPosition.y - coll_rearWheel.transform.localPosition.y / 20;
        coll_rearWheel.transform.localPosition = tmpCollRW01;

        // Get the radius of wheel
        wheelRadius = transform.root.GetComponentInChildren<WheelCollider>().radius;
    }

    void Update()
    {
        // Don't let the bike lean
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    void FixedUpdate()
    {
        ApplyLocalPositionToVisuals(coll_frontWheel);
        ApplyLocalPositionToVisuals(coll_rearWheel);

        // Don't let the bike lean
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        //////////////////////////////////// part where rear pendulum, wheelbar and wheels meshes matched to wheelsColliers and so on
        //beauty - rear pendulumn is looking at rear wheel(if you have both suspension bike)
        if (rearPend)
        {//rear pendulum moves only when bike is full suspension
            var tmp_cs1 = rearPendulumn.transform.localRotation;
            var tmp_cs2 = tmp_cs1.eulerAngles;
            tmp_cs2.x = 0 - 8 + (meshRearWheel.transform.localPosition.y * 100);
            tmp_cs1.eulerAngles = tmp_cs2;
            rearPendulumn.transform.localRotation = tmp_cs1;
        }
        //beauty - wheel bar rotating by front wheel
        var tmp_cs3 = suspensionFront_down.transform.localPosition;
        tmp_cs3.y = (meshFrontWheel.transform.localPosition.y - 0.15f);
        suspensionFront_down.transform.localPosition = tmp_cs3;
        var tmp_cs4 = meshFrontWheel.transform.localPosition;
        tmp_cs4.z = meshFrontWheel.transform.localPosition.z - (suspensionFront_down.transform.localPosition.y + 0.4f) / 5;
        meshFrontWheel.transform.localPosition = tmp_cs4;

        // debug - all wheels are white in idle(no accelerate, no brake)
        meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
        meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

        // drag and angular drag for emulate air resistance
        GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 210 * airRes; // when 250 bike can easy beat 200km/h // ~55 m/s
        GetComponent<Rigidbody>().angularDrag = 7 + GetComponent<Rigidbody>().velocity.magnitude / 20;


        ///bicycle code
        coll_frontWheel.forceAppPointDistance = frontWheelAPD - bikeSpeedKPH / 1000;
        if (coll_frontWheel.forceAppPointDistance < 0.001f)
        {
            coll_frontWheel.forceAppPointDistance = 0.001f;
        }

        // Key Control 1: Speed
        bikeSpeedKPH = 0;
        if (outsideControls.Vertical > 0)
        {
            // Set the spped of the bike
            bikeSpeedKPH = outsideControls.bikeSpeedKPH;
            bikeSpeedMPS = bikeSpeedKPH / 3.6f;
            bikeSpeedCPS = bikeSpeedMPS / (2 * Mathf.PI * wheelRadius);
            bikeSpeedCPM = bikeSpeedCPS * 60.0f;
            // This is a old variable, which will be removed in the future
            bikeSpeed = bikeSpeedKPH;

            coll_frontWheel.brakeTorque = 0;//we need that to fix strange unity bug when bike stucks if you press "accelerate" just after "brake".

            if (GetComponent<Rigidbody>().velocity.magnitude < 0.5f)
            {
                coll_rearWheel.motorTorque = initialForce;
            }
            else
            {
                Vector3 velocity = GetComponent<Rigidbody>().velocity /
                                    GetComponent<Rigidbody>().velocity.magnitude * bikeSpeedMPS;
                // Avoid the bike go backward
                if (Mathf.Abs(Vector3.Angle(velocity, transform.forward)) > 50)
                {
                    velocity = new Vector3(0, 0, 0);
                }
                // Avoid the bike go up
                else if (Mathf.Abs(Vector3.Angle(velocity, transform.up)) < 80)
                {
                    velocity = new Vector3(0, 0, 0);
                }

                GetComponent<Rigidbody>().velocity = velocity;
            }

            // debug - rear wheel is green when accelerate
            meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

            // when normal accelerating CoM z is averaged
            var tmp_cs5 = CoM.localPosition;
            tmp_cs5.z = 0.0f + tmpMassShift;
            tmp_cs5.y = normalCoM;
            CoM.localPosition = tmp_cs5;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        else if (outsideControls.Vertical == 0 && !isFrontWheelInAir)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        else
        {
            RearSuspensionRestoration();
        }

        // Key Control 2: Turn
        tempMaxwheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeedKPH);//associate speed with curve which you've tuned in Editor

        if (outsideControls.Horizontal != 0)
        {
            coll_frontWheel.steerAngle = (outsideControls.TurnByUdp) ?
                                            outsideControls.wheelAngle :
                                            tempMaxwheelAngle * outsideControls.Horizontal;
            steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler(0, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
        }
        else coll_frontWheel.steerAngle = 0;


        /////////////////////////////////////////////////// PILOT'S MASS //////////////////////////////////////////////////////////
        // it's part about moving of pilot's center of mass. It can be used for wheelie or stoppie control and for motocross section in future
        //not polished yet. For mobile version it should back pilot's mass smooth not in one tick
        if (outsideControls.VerticalMassShift > 0)
        {
            tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
            var tmp_cs19 = CoM.localPosition;
            tmp_cs19.z = tmpMassShift;
            CoM.localPosition = tmp_cs19;

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        if (outsideControls.VerticalMassShift < 0)
        {
            tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
            var tmp_cs20 = CoM.localPosition;
            tmp_cs20.z = tmpMassShift;
            CoM.localPosition = tmp_cs20;

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        if (outsideControls.HorizontalMassShift < 0)
        {
            var tmp_cs21 = CoM.localPosition;
            tmp_cs21.x = outsideControls.HorizontalMassShift / 40;
            CoM.localPosition = tmp_cs21;//40 to get 0.025m at final

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);

        }
        if (outsideControls.HorizontalMassShift > 0)
        {
            var tmp_cs22 = CoM.localPosition;
            tmp_cs22.x = outsideControls.HorizontalMassShift / 40;
            CoM.localPosition = tmp_cs22;//40 to get 0.025m at final

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }


        //auto back CoM when any key not pressed
        if (outsideControls.Vertical == 0 && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn || (outsideControls.Vertical < 0 && isFrontWheelInAir))
        {
            var tmp_cs23 = CoM.localPosition;
            tmp_cs23.y = normalCoM;
            tmp_cs23.z = 0.0f + tmpMassShift;
            CoM.localPosition = tmp_cs23;
            coll_frontWheel.motorTorque = 0;
            coll_frontWheel.brakeTorque = 0;
            coll_rearWheel.motorTorque = 0;
            coll_rearWheel.brakeTorque = 0;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        //autoback pilot's CoM along
        if (outsideControls.VerticalMassShift == 0 && outsideControls.Vertical >= 0 && outsideControls.Vertical <= 0.9f && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn)
        {
            var tmp_cs24 = CoM.localPosition;
            tmp_cs24.z = 0.0f;
            CoM.localPosition = tmp_cs24;
            tmpMassShift = 0.0f;
        }
        //autoback pilot's CoM across

        if (outsideControls.HorizontalMassShift == 0 && outsideControls.Vertical <= 0 && !outsideControls.rearBrakeOn)
        {
            var tmp_cs25 = CoM.localPosition;
            tmp_cs25.x = 0.0f;
            CoM.localPosition = tmp_cs25;
        }

        /////////////////////////////////////////////////////// RESTART KEY ///////////////////////////////////////////////////////////
        // Restart key - recreate bike few meters above current place
        if (outsideControls.restartBike)
        {
            if (outsideControls.fullRestartBike)
            {
                transform.position = new Vector3(0, 0.5f, 2f);
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            else
            {
                Vector3 currentRoadPos = GameObject.Find(outsideControls.CurrentRoad).transform.position;
                Vector3 currentRoadDir = GameObject.Find(outsideControls.CurrentRoad).transform.eulerAngles;
                transform.position = currentRoadPos + new Vector3(0, 0.5f, 0);
                transform.eulerAngles = new Vector3(currentRoadDir.x, currentRoadDir.y, 0);
            }
            transform.position += new Vector3(0, 0.1f, 0);
            transform.rotation = Quaternion.Euler(0.0f, transform.localEulerAngles.y, 0.0f);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            var tmp_cs26 = CoM.localPosition;
            tmp_cs26.x = 0.0f;
            tmp_cs26.y = normalCoM;
            tmp_cs26.z = 0.0f;
            CoM.localPosition = tmp_cs26;
            //for fix bug when front wheel IN ground after restart(sorry, I really don't understand why it happens);
            coll_frontWheel.motorTorque = 0;
            coll_frontWheel.brakeTorque = 0;
            coll_rearWheel.motorTorque = 0;
            coll_rearWheel.brakeTorque = 0;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }



        ///////////////////////////////////////// CRASH happens /////////////////////////////////////////////////////////
        //// conditions when crash is happen
        //if ((this.transform.localEulerAngles.z >= crashAngle01 && this.transform.localEulerAngles.z <= crashAngle02) && !linkToStunt.stuntIsOn || (this.transform.localEulerAngles.x >= crashAngle03 && this.transform.localEulerAngles.x <= crashAngle04 && !linkToStunt.stuntIsOn))
        //{
        //    GetComponent<Rigidbody>().drag = 0.1f; // when 250 bike can easy beat 200km/h // ~55 m/s
        //    GetComponent<Rigidbody>().angularDrag = 0.01f;
        //    crashed = true;
        //    var tmp_cs27 = CoM.localPosition;
        //    tmp_cs27.x = 0.0f;
        //    tmp_cs27.y = CoMWhenCrahsed;//move CoM a little bit up for funny bike rotations when fall
        //    tmp_cs27.z = 0.0f;
        //    CoM.localPosition = tmp_cs27;
        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        //}

        //if (crashed) coll_rearWheel.motorTorque = 0;//to prevent some bug when bike crashed but still accelerating
    }

    //void Update (){
    //not use that because everything here is about physics
    //}
    ///////////////////////////////////////////// FUNCTIONS /////////////////////////////////////////////////////////
    void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);
        wheelCCenter = collider.transform.TransformPoint(collider.center);


        //dpn't need movement of rear suspension because MTB have no rear suspension
        if (!rearPend)
        {//case where MTB have no rear suspension
            if (collider.gameObject.name != "coll_rear_wheel")
            {
                if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
                {
                    visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;
                }
                else
                {
                    visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
                }
            }
        }
        else
        {//case where bicycle has sull suspension
            if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
            {
                visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;

            }
            else
            {
                visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
            }

        }

        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        collider.GetWorldPose(out position, out rotation);

        visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x, collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
        visualWheel.Rotate(collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);

    }
    //need to restore spring power for rear suspension after make it harder for wheelie
    void RearSuspensionRestoration()
    {
        var tmpRearSusp = coll_rearWheel.suspensionSpring;
        tmpRearSusp.spring = normalRearSuspSpring;
        coll_rearWheel.suspensionSpring = tmpRearSusp;
    }
    //need to restore spring power for front suspension after make it weaker for stoppie
    void FrontSuspensionRestoration(int sprWeakness)
    {
        if (forgeBlocked)
        {//supress front spring power to avoid too much force back
            var tmpFrntSusp = coll_frontWheel.suspensionSpring;
            tmpFrntSusp.spring = sprWeakness;
            coll_frontWheel.suspensionSpring = tmpFrntSusp;
            forgeBlocked = false;
        }
        if (coll_frontWheel.suspensionSpring.spring < normalFrontSuspSpring)
        {//slowly returning force to front spring
            var tmpFrntSusp2 = coll_frontWheel.suspensionSpring;
            tmpFrntSusp2.spring += 500.0f;
            coll_frontWheel.suspensionSpring = tmpFrntSusp2;
        }
    }
}