/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System.Collections;

public class AIBicycle_code : MonoBehaviour
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

    // crashed status. To know when we need to desable controls because bike is too leaned.
    [HideInInspector]
    public bool crashed = false;

    // there is angles when bike takes status crashed(too much lean, or too much stoppie/wheelie)
    float crashAngle01 = 60;//crashed status is on if bike have more Z(side fall) angle than this												
    float crashAngle02 = 300;//crashed status is on if bike have less Z(side fall) angle than this 												
    float crashAngle03 = 60;//crashed status is on if bike have more X(front fall) angle than this 												
    float crashAngle04 = 280;//crashed status is on if bike have more X(back fall) angle than this												

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
    private float tempMaxWheelAngle;

    //variable for cut off wheel bar rotation angle at high speed
    //private float wheelPossibleAngle = 0.0f;

    //for wheels vusials match up the wheelColliders
    private Vector3 wheelCCenter;
    private RaycastHit hit;

    /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
    [HideInInspector]
    public float frontBrakePower = 25; //brake power absract - 100 is good brakes																		

    //float LegsPower; // Leg's power to wheels. Abstract it's not HP or KW or so...
    
    // airRes is for wind resistance to large bikes more than small ones
    float airRes; //Air resistant 																										// 1 is neutral
    
    /////////////////////////////////////////////////// BICYCLE CODE ///////////////////////////////////////////////////////
    [HideInInspector]
    public float frontWheelAPD;// usualy 0.05f

    private GameObject pedals;
    private AIPedalControls linkToStunt;
    private bool rearPend;

    [HideInInspector]
    public float bikeSpeed; //to know bike speed km/h

    [HideInInspector]
    public bool isReverseOn = false; //to turn On and Off reverse speed

    /// <summary>
    /// Below are those key control elements that will influence the movement of bikes
    /// </summary>
    // Key control
    GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's script
    public float initialForce; // The initial torque added to the wheel when start

    ////////////////////////////////////////////////  ON SCREEN INFO ///////////////////////////////////////////////////////
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

        linkToStunt = GetComponentInChildren<AIPedalControls>();
        pedals = linkToStunt.gameObject;

        Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass
        GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
        GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass

        // wheel colors for understanding of accelerate, idle, brake(white is idle status)
        meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
        meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

        //for better physics of fast moving bodies
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

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
        if (!crashed)
        {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 210 * airRes; // when 250 bike can easy beat 200km/h // ~55 m/s
            GetComponent<Rigidbody>().angularDrag = 7 + GetComponent<Rigidbody>().velocity.magnitude / 20;
        }

        //determinate the bike speed in km/h
        bikeSpeed = Mathf.Round((GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10.0f) * 0.1f; //from m/s to km/h

        ///bicycle code
        coll_frontWheel.forceAppPointDistance = frontWheelAPD - bikeSpeed / 1000;
        if (coll_frontWheel.forceAppPointDistance < 0.001f)
        {
            coll_frontWheel.forceAppPointDistance = 0.001f;
        }

        //////////////////////////////////// turnning /////////////////////////////////////////////////////////////			
        //tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeed);//associate speed with curve which you've tuned in Editor

        //if (!crashed && outsideControls.Horizontal != 0)
        //{ 
        //    coll_frontWheel.steerAngle = (outsideControls.TurnByUdp) ?
        //                                    outsideControls.WheelAngle :
        //                                    tempMaxWheelAngle * outsideControls.Horizontal;
        //    steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler(0, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
        //}
        //else coll_frontWheel.steerAngle = 0;


        /////////////////////////////////////////////////// PILOT'S MASS //////////////////////////////////////////////////////////
        // it's part about moving of pilot's center of mass. It can be used for wheelie or stoppie control and for motocross section in future
        //not polished yet. For mobile version it should back pilot's mass smooth not in one tick
        //if (outsideControls.VerticalMassShift > 0)
        //{
        //    tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
        //    var tmp_cs19 = CoM.localPosition;
        //    tmp_cs19.z = tmpMassShift;
        //    CoM.localPosition = tmp_cs19;

        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        //}
        //if (outsideControls.VerticalMassShift < 0)
        //{
        //    tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
        //    var tmp_cs20 = CoM.localPosition;
        //    tmp_cs20.z = tmpMassShift;
        //    CoM.localPosition = tmp_cs20;

        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        //}
        //if (outsideControls.HorizontalMassShift < 0)
        //{
        //    var tmp_cs21 = CoM.localPosition;
        //    tmp_cs21.x = outsideControls.HorizontalMassShift / 40;
        //    CoM.localPosition = tmp_cs21;//40 to get 0.025m at final

        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);

        //}
        //if (outsideControls.HorizontalMassShift > 0)
        //{
        //    var tmp_cs22 = CoM.localPosition;
        //    tmp_cs22.x = outsideControls.HorizontalMassShift / 40;
        //    CoM.localPosition = tmp_cs22;//40 to get 0.025m at final

        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        //}


        ////auto back CoM when any key not pressed
        //if (!crashed && outsideControls.Vertical == 0 && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn || (outsideControls.Vertical < 0 && isFrontWheelInAir))
        //{
        //    var tmp_cs23 = CoM.localPosition;
        //    tmp_cs23.y = normalCoM;
        //    tmp_cs23.z = 0.0f + tmpMassShift;
        //    CoM.localPosition = tmp_cs23;
        //    coll_frontWheel.motorTorque = 0;
        //    coll_frontWheel.brakeTorque = 0;
        //    coll_rearWheel.motorTorque = 0;
        //    coll_rearWheel.brakeTorque = 0;
        //    GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        //}
        ////autoback pilot's CoM along
        //if (outsideControls.VerticalMassShift == 0 && outsideControls.Vertical >= 0 && outsideControls.Vertical <= 0.9f && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn)
        //{
        //    var tmp_cs24 = CoM.localPosition;
        //    tmp_cs24.z = 0.0f;
        //    CoM.localPosition = tmp_cs24;
        //    tmpMassShift = 0.0f;
        //}
        ////autoback pilot's CoM across

        //if (outsideControls.HorizontalMassShift == 0 && outsideControls.Vertical <= 0 && !outsideControls.rearBrakeOn)
        //{
        //    var tmp_cs25 = CoM.localPosition;
        //    tmp_cs25.x = 0.0f;
        //    CoM.localPosition = tmp_cs25;
        //}

        /////////////////////////////////////////////////////// RESTART KEY ///////////////////////////////////////////////////////////
        //// Restart key - recreate bike few meters above current place
        //if (outsideControls.restartBike && outsideControls.fullRestartBike)
        //{
        //}
    }

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