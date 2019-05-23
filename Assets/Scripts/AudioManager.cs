using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("chain");
        Play("tires_moving");

    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
       
    }

    private void Update()
    {
        float speedScaler = GameObject.Find("BikerEthan").GetComponent<bicycle_code>().bikeSpeedKPH;

        foreach (Sound s in sounds)
        {
            // Change sound volume in relation to speed of cycling
            s.source.volume = (s.volume * speedScaler / 30f);
            if (s.name == "music") {
                s.source.volume += 0.1f;
            }
        }

    }
}
