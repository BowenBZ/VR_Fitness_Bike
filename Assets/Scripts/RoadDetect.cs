using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadDetect : MonoBehaviour
{
    [HideInInspector]
    public Vector3 roadPosition;

    [HideInInspector]
    public Vector3 roadDirection;

    controlHub outsideControls;

    void Start()
    {
        roadPosition = new Vector3(0, 0, 0);
        roadDirection = new Vector3(0, 0, 0);
        outsideControls = GameObject.FindGameObjectWithTag("manager").GetComponent<controlHub>();
    }
     
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "collider_01")
        {
            roadPosition = transform.position;
            roadDirection = transform.eulerAngles;
            outsideControls.CurrentRoad = gameObject.name;
            Debug.Log(gameObject.name);
        }
    }
}
