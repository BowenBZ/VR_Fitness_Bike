// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class AIPedalControls : MonoBehaviour
{

    private AIBicycle_code linkToBike;// making a link to corresponding bike's script
    private biker_logic_mecanim linkToRider;//link to rider's script

    public GameObject pedalLeft;
    public GameObject pedalRight;

    private GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's script

    private float energy = 0;//energy of pedaling to release after acceleration off

    //we need it to move CoM left and right to immitate rotation of pedals
    public Transform CoM; //CoM object

    //to move pelvis of rider
    public Transform veloMan;

    //BIKE for stunts
    public Transform stuntBike;

    //special temp status "on stunt" to prevent fall after maximus angle exceed
    public bool stuntIsOn = false;

    //tmp "true" during "in stunt" 
    private bool inStunt = false;
    void Start()
    {

        ctrlHub = GameObject.FindGameObjectWithTag("manager");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        linkToBike = transform.root.GetComponent<AIBicycle_code>();
        linkToRider = transform.root.GetComponentInChildren<biker_logic_mecanim>();

    }


    void FixedUpdate()
    {
        //pedals rotation part
        float bikeSpeed = transform.root.GetComponent<KeepMoving>().speedKM;
        this.transform.rotation = this.transform.rotation * Quaternion.Euler(bikeSpeed / 4, 0, 0);
        //Debug.Log("AIBikeSpeed:" + linkToBike.bikeSpeed);
        pedalRight.transform.rotation = pedalRight.transform.rotation * Quaternion.Euler(-bikeSpeed / -4, 0, 0);
        pedalLeft.transform.rotation = pedalLeft.transform.rotation * Quaternion.Euler(-bikeSpeed / 4, 0, 0);
        if (energy < 10)
        {
            energy = energy + 0.01f;
        }

        if (Mathf.Abs(CoM.transform.localPosition.x) < 0.1f)
        {
            var tmpRidPlvs01 = veloMan.transform.localEulerAngles;
            tmpRidPlvs01.z = CoM.transform.localPosition.x * 200;
            veloMan.transform.localEulerAngles = tmpRidPlvs01;
            //(sometimes looks strange on bicycles with high seat. So, you might just disable it when needed)
        }
        var tmpCoM01 = CoM.transform.localPosition;
        //tmpCoM01.x = -0.02f + (Mathf.Abs(this.transform.localRotation.x) / 25);//leaning bicycle when pedaling
        tmpCoM01.x = 0;
        CoM.transform.localPosition = tmpCoM01;

        //else EnergyWaste();//need to move pedals some time after stop acceleration

        //movement body of rider's pelvis when cornering(sometimes looks strange on bicycles with high seat. So, you might just disable it when needed)
        //var tmpRidPlvs02 = veloMan.transform.localPosition;
        //tmpRidPlvs02.x = outsideControls.Horizontal / 10;
        //veloMan.transform.localPosition = tmpRidPlvs02;



    }

    //function when player stop accelerating and rider still slowly rotating pedals
    void EnergyWaste()
    {
        if (energy > 0)
        {
            var tmpEnergy = 10 - energy;
            this.transform.rotation = this.transform.rotation * Quaternion.Euler((linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
            pedalRight.transform.rotation = pedalRight.transform.rotation * Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / -4, 0, 0);
            pedalLeft.transform.rotation = pedalLeft.transform.rotation * Quaternion.Euler(-(linkToBike.bikeSpeed - tmpEnergy) / 4, 0, 0);
            energy = energy - 0.1f;

        }
    }

    //trick to do not crash for one second. You need that for riding ramps
    IEnumerator StuntHoldForOneSecond()
    {
        stuntIsOn = true;
        yield return new WaitForSeconds(0.5f);//1 second seems too long :) now it's half of second 0.5ff. Make 1 for actually 1 second
        if (!inStunt)
        {
            stuntIsOn = false;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // applying physical forces to immitate stunts///////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //void StuntBunnyHope (){
    IEnumerator StuntBunnyHope()
    {
        linkToRider.PlayA("bannyhope");//animation is optional. You may delete this string with no bad aftermath
        stuntBike.GetComponent<Rigidbody>().AddForce(Vector3.up * 40000);//push bike up
        yield return new WaitForSeconds(0.1f);//a little pause between applying force
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -14000);//pull front wheel(turn bike around CoM)
        yield return new WaitForSeconds(0.2f);//a little pause between applying force
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * 20000);//push front down and pull rear up
    }
    void StuntManual()
    {
        linkToRider.PlayA("manual");
    }

    //here is stunts
    IEnumerator StuntBackFlip360()
    {
        linkToRider.PlayA("backflip360");
        stuntIsOn = true;
        inStunt = true;
        //CoM.transform.localPosition.y = 0;
        var tmpCoM01 = CoM.transform.localPosition;
        tmpCoM01.y = 0;
        CoM.transform.localPosition = tmpCoM01;
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddTorque(transform.right * -2500000);
        yield return new WaitForSeconds(0.7f);
        inStunt = false;
        stuntIsOn = false;
    }

    IEnumerator StuntTurnLeft180()
    {
        linkToRider.PlayA("rightflip180");
        stuntIsOn = true;
        inStunt = true;
        var tmpCoM02 = CoM.transform.localPosition;
        tmpCoM02.y = 0;
        CoM.transform.localPosition = tmpCoM02;
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 10000);
        yield return new WaitForSeconds(0.7f);
        inStunt = false;
        stuntIsOn = false;
    }

    IEnumerator StuntBunnyShiftRight()
    {
        linkToRider.PlayA("bannyhope");

        stuntBike.GetComponent<Rigidbody>().AddForce(Vector3.up * 45000);//push bike up
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -4000);//pull front wheel(turn bike around CoM)
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 1000);//turn bike right
        yield return new WaitForSeconds(0.1f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * 24000);//push bike right
        yield return new WaitForSeconds(0.2f);
        stuntBike.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * -3000);//turn bike left


    }
}