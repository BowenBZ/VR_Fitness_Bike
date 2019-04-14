using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 originPosition;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        KeyDetect();
    }

    float moveDistance = 1.8f;
    float currentPosition = 0;
    float delta = 0.01f;
    bool release = false;
    int type = 0; //-1-left 1-right
    int type2 = 0; //1-forward -1-backward
    void KeyDetect()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            release = false;
            type2 = 1;
            currentPosition += delta;
            currentPosition = (moveDistance < currentPosition) ? moveDistance : currentPosition;
            transform.localPosition = originPosition + new Vector3(-currentPosition, 0, 0);
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            release = false;
            type2 = -1;
            currentPosition += delta;
            currentPosition = (moveDistance < currentPosition) ? moveDistance : currentPosition;
            transform.localPosition = originPosition + new Vector3(currentPosition, 0, 0);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            type = -1;
            release = true;
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            release = false;
            type2 = 1;
            currentPosition += delta;
            currentPosition = (moveDistance < currentPosition) ? moveDistance : currentPosition;
            transform.localPosition = originPosition + new Vector3(currentPosition, 0, 0);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            release = false;
            type2 = -1;
            currentPosition += delta;
            currentPosition = (moveDistance < currentPosition) ? moveDistance : currentPosition;
            transform.localPosition = originPosition + new Vector3(-currentPosition, 0, 0);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            type = 1;
            release = true;
        }

        if (release)
        {
            if (currentPosition <= 0)
            {
                currentPosition = 0;
                release = false;
                transform.localPosition = originPosition;
            }
            else
            {
                currentPosition -= delta * 3;
                transform.localPosition = originPosition + new Vector3(type * type2 * currentPosition, 0, 0);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(new Vector3(-0.05f, 0, 0));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(new Vector3(0.05f, 0, 0));
        }
    }
}
