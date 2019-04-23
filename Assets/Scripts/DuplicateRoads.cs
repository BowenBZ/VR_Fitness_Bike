using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRoads : MonoBehaviour
{
    public GameObject roadPrefab;
    GameObject LastBlock;
    int cnt = 1;
    float AbsAngle = 0;
    float RoadLength = 1f;
    int PreAngle = 0;

    Vector3 AbsLocation = new Vector3(0, 0, 0);
    Vector3 AbsRotation = new Vector3(0, 0, 0);
    List<float> DistanceList = new List<float>{ 440, 250, 190, 200, 250, 190, 200, 250, 190, 200,
     250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 250, 190, 200, 750};
    List<float> DegreeList = new List<float>{ 0, 15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5,
     15, 0, -5, 15, 0, -5, 15, 0, -5, 15, 0, -5, 0};
    List<float> TurningList = new List<float> {};


    //List<float> List1 = new List<float> { 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
    //List<float> List2 = new List<float>{ 10, 10, 10, 10, 8, 6, 4,2,0,2,4,6,8,10,10,10,10,10,10,10  };
    // Start is called before the first frame update
    void Start()
    {
        LastBlock = GameObject.Instantiate(roadPrefab);
        SmoothDegChange(DistanceList, DegreeList);
        RoadTurning();
        for (int i = 0; i < DistanceList.Count - 1; i++)
        {
            GenerateRoad(DistanceList[i], DegreeList[i], TurningList[i]);
            Debug.Log(LastBlock.transform.position);
            //GenerateRoad(List1[i], List2[i]);
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

    void GenerateRoad(float length, float angle, float TurningAngle)// For level road use 360 degree instead of 0
    {
        float slope = 2f * Mathf.PI * angle / 360f;
        float AbsSlope;
        float Turning;
        GameObject Temp = GameObject.Instantiate(roadPrefab);
        
        float unitTurnAngle = TurningAngle / 60;
        float TA = 0;

        for (int i = 0; i < length / RoadLength; i++)
        {

            Turning = 1f * i;
            AbsSlope = 2f * Mathf.PI / (360f / (90 - AbsAngle));
            GameObject obj = GameObject.Instantiate(roadPrefab);
            obj.transform.eulerAngles = new Vector3(-1f * angle, 0, 0);
            //AbsLocation = AbsLocation + new Vector3(0, 1 / 2f * RoadLength * (Mathf.Sin(slope) + Mathf.Cos(AbsSlope)),
            //   1 / 2f * RoadLength * (Mathf.Cos(slope) + Mathf.Sin(AbsSlope)));
            obj.transform.position = LastBlock.transform.position + LastBlock.transform.forward.normalized * RoadLength;
            if (LastBlock.transform.eulerAngles != new Vector3(0,0,0))
            {
                obj.transform.eulerAngles = LastBlock.transform.eulerAngles;
            }
            //obj.transform.Rotate(-1f * angle, 0, 0);
            if (i > 20 && i <= 80)//Start turing When i = 20 end at i =50
            {
                TA = TA + unitTurnAngle;
                obj.transform.position = Temp.transform.position;
                obj.transform.rotation = Temp.transform.rotation;
                obj.transform.RotateAround(Temp.transform.position + Temp.transform.right.normalized * 60, Temp.transform.up.normalized, TA);
            }
            LastBlock = obj;          
            AbsAngle = angle;
            if (i == 20)
            {
                Temp = obj;
            }

        }
    }

    void GenerateRoadtest(float length, float angle, float TurningAngle)// For level road use 360 degree instead of 0
    {
        float TurningDistance = 60;
        float TurningStart = 20;//Random.Range(10, length - TurningDistance);
        float TurningEnd = TurningStart + 60f;
        float slope = 2f * Mathf.PI * angle / 360f;
        float AbsSlope;
        float Turning;

        GameObject TStartBlock = GameObject.Instantiate(roadPrefab);
        GameObject LastBlock = GameObject.Instantiate(roadPrefab);

        float UnitTurnAngle = TurningAngle / TurningDistance;
        float AccumulateAngle = 0;

        Vector3 ObRotation = new Vector3(0, 0, 0);

        for (int i = 0; i < length / RoadLength; i++)
        {
            GameObject obj = GameObject.Instantiate(roadPrefab);
            obj.transform.eulerAngles = new Vector3(-1f * angle, 0, 0);
            if (i == TurningStart - 1)// Store the last block before turning
            {
                TStartBlock = obj;
            }

            if (i >= TurningStart && i <= TurningEnd)//Turning process
            {
                AccumulateAngle = AccumulateAngle + UnitTurnAngle;
                obj.transform.position = TStartBlock.transform.position;
                obj.transform.rotation = TStartBlock.transform.rotation;
                obj.transform.RotateAround(TStartBlock.transform.position + TStartBlock.transform.right.normalized * 60,
                    TStartBlock.transform.up.normalized, AccumulateAngle);
                LastBlock.transform.position = obj.transform.position;
                LastBlock.transform.rotation = obj.transform.rotation;
            }
            else
            {
                Turning = 1f * i;
                AbsSlope = 2f * Mathf.PI / (360f / (90 - AbsAngle));

                AbsLocation = AbsLocation + new Vector3(0, 1 / 2f * RoadLength * (Mathf.Sin(slope) + Mathf.Cos(AbsSlope)),
                    1 / 2f * RoadLength * (Mathf.Cos(slope) + Mathf.Sin(AbsSlope)));
                obj.transform.position = AbsLocation;
                LastBlock.transform.position = obj.transform.position;
                LastBlock.transform.rotation = obj.transform.rotation;
            }


            AbsLocation = obj.transform.position;
            ObRotation = obj.transform.eulerAngles;
            AbsAngle = angle;//To Store the current rotate angle 


        }
    }







    void SmoothDegChange(List<float> DisList, List<float> DegList)
    {
        float DegChange = 0;
        int index = 1;
        while (true)
        {
            if (index + 4 > DisList.Count - 1)
                break;

            if (DegList[index] * DegList[index - 1] < 0)
            {

                float DegChange2 = Mathf.Abs(DegList[index]) - 0;
                float DegChange1 = 0 - Mathf.Abs(DegList[index - 1]);

                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);

                DegList.Insert(index, 4 * DegChange2 / 5);
                DegList.Insert(index, 3 * DegChange2 / 5);
                DegList.Insert(index, 2 * DegChange2 / 5);
                DegList.Insert(index, 1 * DegChange2 / 5);
                DegList.Insert(index, 0);

                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);
                DisList.Insert(index, 2 * RoadLength);

                DegList.Insert(index, 1 * DegChange1 / 5);
                DegList.Insert(index, 2 * DegChange1 / 5);
                DegList.Insert(index, 3 * DegChange1 / 5);
                DegList.Insert(index, 4 * DegChange1 / 5);
                index = index + 10;
            }
            else
            {
                if (Mathf.Abs(DegList[index]) - Mathf.Abs(DegList[index - 1]) < 0)
                {
                    DegChange = Mathf.Abs(DegList[index] - DegList[index - 1]);

                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);

                    DegList.Insert(index, 1 * DegChange / 5);
                    DegList.Insert(index, 2 * DegChange / 5);
                    DegList.Insert(index, 3 * DegChange / 5);
                    DegList.Insert(index, 4 * DegChange / 5);
                }
                else
                {
                    DegChange = DegList[index] - Mathf.Abs(DegList[index - 1]);

                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);
                    DisList.Insert(index, 2 * RoadLength);

                    DegList.Insert(index, 4 * DegChange / 5);
                    DegList.Insert(index, 3 * DegChange / 5);
                    DegList.Insert(index, 2 * DegChange / 5);
                    DegList.Insert(index, 1 * DegChange / 5);
                }
                index = index + 5;
            }

        }
    }

    void RoadTurning()
    {
        int Turingdegree;

        for (int i = 0; i < DistanceList.Count; i++)
        {
            if (DistanceList[i] > 5 * RoadLength)
            {
                //Turingdegree = Random.Range(0, 5);
                Turingdegree = 30;
            }
            else
            {
                Turingdegree = 0;
            }
            TurningList.Add(Turingdegree);
            Debug.Log("Turning Degree:   " + Turingdegree);
        }

    }

}
