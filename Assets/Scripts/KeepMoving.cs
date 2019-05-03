using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMoving : MonoBehaviour
{
    public float speedKM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make the bike attached to road
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        {
            Transform current_block = hit.transform;
            GetComponent<Rigidbody>().velocity = speedKM * 1000 / 3600.0f * current_block.forward.normalized;
        }
    }
}
