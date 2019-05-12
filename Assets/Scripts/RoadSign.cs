using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoadSign : MonoBehaviour
{

    controlHub outsideControls;

    void Start()
    {
        outsideControls = GameObject.FindGameObjectWithTag("manager").GetComponent<controlHub>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "collider_01")
        {
            outsideControls.currentSignUser = Int32.Parse(gameObject.name.Split('_')[1]);
        }
        else if (collision.gameObject.name == "AIcollider_01")
        {
            outsideControls.currentSignAI = Int32.Parse(gameObject.name.Split('_')[1]);
        }
    }
}
