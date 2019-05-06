using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMoving : MonoBehaviour
{ 
    // The speed of AI rider
    public float speedKM;
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

    // Start is called before the first frame update
    void Start()
    {
        waitTime = 0;
        moveRange = 0;
        leftMoveRangeBar = -4.5f - transform.position.x;
        rightMoveRangeBar = 4.5f - transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        {
            current_block = hit.transform;
            GetComponent<Rigidbody>().velocity = speedKM * 1000 / 3600.0f * current_block.forward.normalized;
            transform.forward = current_block.forward;
        }

        UpdateHorizontalPosition();
    }

    void UpdateHorizontalPosition()
    {
        if(waitTime++ > 100)
        {
            moveRange = Random.Range(-1.0f, 1.0f);
            laterPosition = transform.position + moveRange * transform.right;
            if((laterPosition - current_block.position).x > rightMoveRangeBar || 
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
