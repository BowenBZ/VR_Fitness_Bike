using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
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


    float speed = 5.0f;
    void KeyDetect()
    { 
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(speed, 0, 0));
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(-speed, 0, 0));
        }

        if (gameObject.tag != "back_wheel")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.localRotation = Quaternion.Euler(0, -30, 0);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.localRotation = Quaternion.Euler(0, 30, 0);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
