/// Writen by Boris Chuprin smokerr@mail.ru
/// Great gratitude to everyone who helps me to convert it to C#
/// Thank you so much !!
using UnityEngine;
using System.Collections;

public class skidMarks : MonoBehaviour
{
    public bicycle_code linkToBike;
    public Transform skidMarkDecal;
    private WheelHit hit;
    private Vector3 skidMarkPos;
    private Quaternion rotationToLastSkidmark;
    private Vector3 lastSkidMarkPos;
    private Vector3 relativePos;

    void Start()
    {
        linkToBike = GetComponent<bicycle_code>();
    }

    void FixedUpdate()
    {
        //skidmarks for rear wheel(braking drifting)
        if (linkToBike.coll_rearWheel.sidewaysFriction.stiffness < 0.5f && linkToBike.bikeSpeed > 1)
        {
            if (linkToBike.coll_rearWheel.GetGroundHit(out hit))
            {
                skidMarkPos = hit.point;
                skidMarkPos.y += 0.02f;
                skidMarkDecal.transform.localScale = Vector3.one;
                if (lastSkidMarkPos != Vector3.zero)
                {
                    relativePos = lastSkidMarkPos - skidMarkPos;
                    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
                    Instantiate(skidMarkDecal, skidMarkPos, rotationToLastSkidmark);
                }
                lastSkidMarkPos = skidMarkPos;
            }
        }
        //skidmarks for front wheel(braking)
        if (linkToBike.coll_frontWheel.brakeTorque >= linkToBike.frontBrakePower && linkToBike.bikeSpeed > 1)
        {
            if (linkToBike.coll_frontWheel.GetGroundHit(out hit))
            {
                skidMarkPos = hit.point;
                skidMarkPos.y += 0.02f;
                skidMarkDecal.transform.localScale = Vector3.one * 0.6f;
                if (lastSkidMarkPos != Vector3.zero)
                {
                    relativePos = lastSkidMarkPos - skidMarkPos;
                    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
                    Instantiate(skidMarkDecal, skidMarkPos, rotationToLastSkidmark);
                }
                lastSkidMarkPos = skidMarkPos;
            }
        }
    }

}
