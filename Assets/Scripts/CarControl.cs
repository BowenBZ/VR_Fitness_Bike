using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KeyDetect();
    }

    float speed = 0.05f;
    void KeyDetect()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (speed <= 0.3333)
            {
                speed += 0.01f / 50;
            }
            transform.Translate(new Vector3(0, 0, speed), Space.Self);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -speed), Space.Self);
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(0, -0.2f, 0));
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(0, 0.2f, 0));
        }


        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(0, 0.2f, 0));
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(0, -0.2f, 0));
        }
    }
}
