// Writen by Boris Chuprin smokerr@mail.ru
// Modified by Bowen Zhang
using UnityEngine;
using System.Collections;

public class camSwitcher : MonoBehaviour
{
    public GameObject cameras;
    Camera firstViewCamera;
    Camera thirdViewCamera;
    Camera aroundCamera;
    public bool firstView = false;
    public Transform cameraTarget;

    Camera currentCamera;

    //////////////////// for back Camera 
    float dist = 3.0f;
    float height = 1.0f;

    //////////////////// for around Camera
    private float distance = 3.0f;
    private float xSpeed = 10.0f;
    private float ySpeed = 10.0f;

    private float yMinLimit = -90;
    private float yMaxLimit = 90;

    private float distanceMin = 2;
    private float distanceMax = 10;

    private float x = 0.0f;
    private float y = 0.0f;

    private float smoothTime = 0.2f;

    private float xSmooth = 0.0f;
    private float ySmooth = 0.0f;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;

    //new camera behaviour
    private float currentTargetAngle;

    private GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's script

  
    // Use this for initialization
    void Start()
    {
        ctrlHub = GameObject.FindGameObjectWithTag("manager");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        // Find camera
        firstViewCamera = cameras.transform.Find("FirstView").GetComponent<Camera>();
        thirdViewCamera = cameras.transform.Find("ThirdView").GetComponent<Camera>();
        aroundCamera = cameras.transform.Find("AroundView").GetComponent<Camera>();

        if (firstView)
        {
            // Enable the first view camera
            firstViewCamera.enabled = true;
            thirdViewCamera.enabled = false;
            aroundCamera.enabled = false;
            currentCamera = firstViewCamera;
        }
        else
        {
            // Enable the third view camera
            firstViewCamera.enabled = false;
            thirdViewCamera.enabled = true;
            aroundCamera.enabled = false;
            currentCamera = thirdViewCamera;
        }


        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;

        // Get the current rotation of bike
        currentTargetAngle = cameraTarget.transform.eulerAngles.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {

            // Disable the currentCamera
            currentCamera.enabled = false;
            currentCamera.gameObject.SetActive(false);
            // Enable the around camera
            aroundCamera.enabled = true;
            aroundCamera.gameObject.SetActive(true);
            // Update currentCamera
            currentCamera = aroundCamera;

            x += Input.GetAxis("Mouse X") * xSpeed;
            y -= Input.GetAxis("Mouse Y") * ySpeed;

            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

            xSmooth = Mathf.SmoothDamp(xSmooth, x, ref xVelocity, smoothTime);
            ySmooth = Mathf.SmoothDamp(ySmooth, y, ref yVelocity, smoothTime);

            distance = Mathf.Clamp(distance + Input.GetAxis("Mouse ScrollWheel") * distance, distanceMin, distanceMax);

            currentCamera.transform.localRotation = Quaternion.Euler(ySmooth, xSmooth, 0);
            currentCamera.transform.position = currentCamera.transform.rotation * new Vector3(0.0f, 0.0f, -distance) + cameraTarget.position;

        }
        else
        {

            if (!firstView)
            {
                // Disable the currentCamera
                currentCamera.enabled = false;
                currentCamera.gameObject.SetActive(false);
                // Enable the third view camera
                thirdViewCamera.enabled = true;
                thirdViewCamera.gameObject.SetActive(true);
                // Update current Camera
                currentCamera = thirdViewCamera;

                #region
                ////////////////////// code for back Camera
                //thirdViewCamera.fieldOfView = thirdViewCamera.fieldOfView + outsideControls.Vertical * 20f * Time.deltaTime;
                //if (thirdViewCamera.fieldOfView > 85)
                //{
                //    thirdViewCamera.fieldOfView = 85;
                //}
                //if (thirdViewCamera.fieldOfView < 50)
                //{
                //    thirdViewCamera.fieldOfView = 50;
                //}
                //if (thirdViewCamera.fieldOfView < 60)
                //{
                //    thirdViewCamera.fieldOfView = thirdViewCamera.fieldOfView += 10f * Time.deltaTime;
                //}
                //if (thirdViewCamera.fieldOfView > 60)
                //{
                //    thirdViewCamera.fieldOfView = thirdViewCamera.fieldOfView -= 10f * Time.deltaTime;
                //}

                //float wantedRotationAngle = cameraTarget.eulerAngles.y;
                //float wantedHeight = cameraTarget.position.y + height;
                //float currentRotationAngle = currentCamera.transform.eulerAngles.y;
                //float currentHeight = currentCamera.transform.position.y;

                //currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, 3 * Time.deltaTime);
                //currentHeight = Mathf.Lerp(currentHeight, wantedHeight, 2 * Time.deltaTime);

                //Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
                //currentCamera.transform.position = cameraTarget.position;
                //currentCamera.transform.position -= currentRotation * Vector3.forward * dist;
                //currentCamera.transform.position = new Vector3(currentCamera.transform.position.x, currentHeight, currentCamera.transform.position.z);
                //currentCamera.transform.LookAt(cameraTarget);

                ////New camera features.
                ////Now camera leaning with biker, so horizon is not always horizontal :)
                ////If you don't like it, just disable
                ////from this -----------------------------------------------------------------------

                //// rotate camera according with bike leaning
                //if (cameraTarget.transform.eulerAngles.z > 0 && cameraTarget.transform.eulerAngles.z < 180)
                //{
                //    currentTargetAngle = cameraTarget.transform.eulerAngles.z / 10;
                //}
                //if (cameraTarget.transform.eulerAngles.z > 180)
                //{
                //    currentTargetAngle = -(360 - cameraTarget.transform.eulerAngles.z) / 10;
                //}
                //currentCamera.transform.rotation = Quaternion.Euler(height * 10, currentRotationAngle, currentTargetAngle);
                ////to this -------------------------------------------------------------------------
                #endregion
            }
            else // The first view camera was enabled, which follows the bike
            {
                // Disable the currentCamera
                currentCamera.enabled = false;
                currentCamera.gameObject.SetActive(false);
                // Enable the first view camera
                firstViewCamera.enabled = true;
                firstViewCamera.gameObject.SetActive(true);
                // Update currentCamera
                currentCamera = firstViewCamera;
            }
        }
    }
}