using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dashboard : MonoBehaviour
{
    TextMesh speedText;
    TextMesh timeText;
    float timeSpend;
    int hours, minutes, seconds;
    TextMesh distanceText;
    float distance;
    TextMesh progressText;
    int totalBlock;
    int currentBlock;
    bicycle_code bike;
    controlHub outsideControl;


    // Start is called before the first frame update
    void Start()
    {
        // Bike
        bike = transform.root.gameObject.GetComponent<bicycle_code>();
        // Speed
        speedText = transform.Find("Speed").gameObject.GetComponent<TextMesh>();
        // Time
        timeText = transform.Find("Time").gameObject.GetComponent<TextMesh>();
        timeSpend = 0;
        hours = 0;
        minutes = 0;
        seconds = 0;
        // Distance
        distanceText = transform.Find("Distance").gameObject.GetComponent<TextMesh>();
        distance = 0;
        // Progress
        progressText = transform.Find("Progress").gameObject.GetComponent<TextMesh>();
        totalBlock = 7212;
        outsideControl = GameObject.FindWithTag("manager").GetComponent<controlHub>();
        currentBlock = 0;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Set speed
        speedText.text = Mathf.Round(bike.bikeSpeedRPM) + " RPM";
        // Set time
        timeSpend += Time.fixedDeltaTime;
        hours = (int)timeSpend / 3600;
        minutes = ((int)timeSpend - hours * 3600) / 60;
        seconds = (int)timeSpend - hours * 3600 - minutes * 60;
        timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        // Set distance
        distance += bike.bikeSpeedMPS * Time.fixedDeltaTime;
        distanceText.text = Mathf.Round(distance) + " m";
        // Set progress
        currentBlock = Int32.Parse(outsideControl.CurrentBlock.Split('_')[1]);
        progressText.text = "Finished " + Math.Round((double)currentBlock / (double)totalBlock * 100, 2) + "%";
    }
}
