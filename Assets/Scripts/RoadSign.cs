using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoadSign : MonoBehaviour
{

    controlHub outsideControls;
    Sound[] sounds;
    Sound s;

    AudioClip[] clips;
    AudioSource audioSource;


    void Start()
    {
        outsideControls = GameObject.FindGameObjectWithTag("manager").GetComponent<controlHub>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "collider_01")
        {
            SignSound();
            ChangeMusic();

            outsideControls.currentSignUser = Int32.Parse(gameObject.name.Split('_')[1]);
            // Debug.Log(outsideControls.currentSignUser);
            

        }
        else if (collision.gameObject.name == "AIcollider_01")
        {
            outsideControls.currentSignAI = Int32.Parse(gameObject.name.Split('_')[1]);
        }
    }

    // Expected: Triggers sound effect to notify sign marker
    private void SignSound()
    {
            sounds = GameObject.Find("Audio_Manager").GetComponent<AudioManager>().sounds;
            s = Array.Find(sounds, sound => sound.name == "signSound");
            s.source.Play();
    }

    // Expected: Changes song to a random song in playlist
    private void ChangeMusic() 
    {
            clips = GameObject.Find("music player").GetComponent<Music_Player>().clips;
            audioSource = GameObject.Find("music player").GetComponent<Music_Player>().audioSource;

            audioSource.Stop();
            audioSource.clip = clips[UnityEngine.Random.Range(0,clips.Length)];
            audioSource.Play();
    }

}
