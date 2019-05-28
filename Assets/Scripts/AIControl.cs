using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    // Make the bike attached to road
    RaycastHit hit;
    // Current block
    Transform current_block;
    // Later position
    Vector3 laterPosition;

    // Wait time to update horizontal position
    int waitTime;
    // Move range
    float moveRange;
    // Current move range
    float curMoveRange;
    // left move range bar
    float leftMoveRangeBar;
    // right move range bar
    float rightMoveRangeBar;

    float[] requireBikeSpeedRPM = new float[] {
                                                70, 80, 90, 65, 80,
                                                90, 65, 80, 90, 65,
                                                80, 90, 65, 80, 90,
                                                65, 80, 90, 65, 80,
                                                90, 65, 80, 90, 65,
                                                80, 90, 65, 60,
                                            };
    // The speed of AI rider
    [HideInInspector]
    public float bikeSpeedMPS;
    controlHub outsideControl;
    float wheelRadius;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = 0;
        moveRange = 0;
        leftMoveRangeBar = -4.5f - transform.position.x;
        rightMoveRangeBar = 4.5f - transform.position.x;
        outsideControl = GameObject.FindWithTag("manager").GetComponent<controlHub>();
        wheelRadius = GetComponentInChildren<WheelCollider>().radius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bikeSpeedMPS = (requireBikeSpeedRPM[outsideControl.currentSignAI] + 10) / 60.0f * (2 * Mathf.PI * wheelRadius);
        //Debug.Log(outsideControl.currentSignAI);
        //Debug.Log(bikeSpeedMPS);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        {
            current_block = hit.transform;
            GetComponent<Rigidbody>().velocity = bikeSpeedMPS * current_block.forward.normalized;
            transform.forward = current_block.forward;
        }

        UpdateHorizontalPosition();
    }

    void UpdateHorizontalPosition()
    {
        if (waitTime++ > 1000)
        {
            moveRange = Random.Range(-1.0f, 1.0f);
            laterPosition = transform.position + moveRange * transform.right;
            if ((laterPosition - current_block.position).x > rightMoveRangeBar ||
                (laterPosition - current_block.position).x < leftMoveRangeBar)
            {
                moveRange = 0;
            }
            else
            {
                waitTime = 0;
                curMoveRange = 0;
            }
        }
        if (Mathf.Abs(moveRange) - Mathf.Abs(curMoveRange) > 0)
        {
            transform.position += moveRange * Time.deltaTime * transform.right.normalized;
            curMoveRange += moveRange * Time.deltaTime;
        }
    }
}
