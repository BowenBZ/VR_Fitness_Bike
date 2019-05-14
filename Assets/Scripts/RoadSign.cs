using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoadSign : MonoBehaviour
{

    controlHub outsideControls;
    Sound[] sounds;
    Sound s;

    void Start()
    {
        outsideControls = GameObject.FindGameObjectWithTag("manager").GetComponent<controlHub>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "collider_01")
        {
            // trigger sound
            sounds = GameObject.Find("Audio_Manager").GetComponent<AudioManager>().sounds;
            s = Array.Find(sounds, sound => sound.name == "signSound");
            s.source.Play();
            // Debug.Log("boom, collider hit!");

            outsideControls.currentSignUser = Int32.Parse(gameObject.name.Split('_')[1]);

        }
        else if (collision.gameObject.name == "AIcollider_01")
        {
            outsideControls.currentSignAI = Int32.Parse(gameObject.name.Split('_')[1]);
        }
    }
}
