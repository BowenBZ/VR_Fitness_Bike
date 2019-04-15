using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRoads : MonoBehaviour
{
    public GameObject roadPrefab;
    int cnt = 1;
    float AbsAngle = 360;
    Vector3 AbsLocation = new Vector3(0,0,0);
    int[] DistanceArray = new int[29] { 10, 20, 90, 20, 250, 190, 200, 250, 190, 200,
     250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 750};
    int[] DegreeArray = new int[29] { 360, 15, 360, -5, 15, 360, -5, 15, 360, -5, 15, 360, -5, 15, 360, -5,
     15, 360, -5, 15, 360, -5, 15, 360, -5, 15, 360, -5, 360};
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<DistanceArray.Length;i++)
        {
            GenerateRoad(DistanceArray[i], DegreeArray[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        DetectKey();
    }

    void DetectKey()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
        }
    }

    void GenerateRoad(int length, int angle)// For level road use 360 degree instead of 0
    {
        float slope = 2f * Mathf.PI / (float)(360 / angle);
        float AbsSlope;
        for (int i = 0; i < length / 10; i++)
        {
            AbsSlope = 2f * Mathf.PI / (float)(360 / (90-AbsAngle));
            GameObject obj = GameObject.Instantiate(roadPrefab);
            AbsLocation = AbsLocation + new Vector3(0, 1 / 2f * 10f * (Mathf.Sin(slope) + Mathf.Cos(AbsSlope)),
                1 / 2f * 10f * (Mathf.Cos(slope) + Mathf.Sin(AbsSlope)));
            obj.transform.position =  AbsLocation;
            obj.transform.Rotate(-1f * angle, 0, 0);
            AbsAngle = angle;
        }
        
    }
}
