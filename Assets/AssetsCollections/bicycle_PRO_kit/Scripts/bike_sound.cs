// Writen by Boris Chuprin smokerr@mail.ru
using UnityEngine;
using System.Collections;

public class bike_sound : MonoBehaviour
{

    public bicycle_code linkToBike;// making a link to corresponding bike's script

    private AudioSource skidSound;// makeing another audioSource for skidding sound

    // creating sounds(Link it to real sound files at editor)
    public AudioClip skid;

    //we need to know is any wheel skidding
    public bool isSkidingFront = false;
    public bool isSkidingRear = false;

    private GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's scriptt
    void Start()
    {
        ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        //assign sound to audioSource
        skidSound = gameObject.AddComponent<AudioSource>();
        skidSound.loop = false;
        skidSound.playOnAwake = false;
        skidSound.clip = skid;
        skidSound.pitch = 1.0f;
        skidSound.volume = 1.0f;

        //real-time linking to current bike
        linkToBike = this.GetComponent<bicycle_code>();

    }
    void Update()
    {

        //skids sound
        if (linkToBike.coll_rearWheel.sidewaysFriction.stiffness < 0.5f && !isSkidingRear && linkToBike.bikeSpeed > 1)
        {
            skidSound.Play();
            isSkidingRear = true;
        }
        else if (linkToBike.coll_rearWheel.sidewaysFriction.stiffness >= 0.5f && isSkidingRear || linkToBike.bikeSpeed <= 1)
        {
            skidSound.Stop();
            isSkidingRear = false;
        }
        if (linkToBike.coll_frontWheel.brakeTorque >= (linkToBike.frontBrakePower - 10) && !isSkidingFront && linkToBike.bikeSpeed > 1)
        {
            skidSound.Play();
            isSkidingFront = true;
        }
        else if (linkToBike.coll_frontWheel.brakeTorque < linkToBike.frontBrakePower && isSkidingFront || linkToBike.bikeSpeed <= 1)
        {
            skidSound.Stop();
            isSkidingFront = false;
        }
    }
}