using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateTerrain : MonoBehaviour
{
    public GameObject terrainPrefab;
    public int number;

    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrains();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateTerrains()
    {
        for(int i = 1; i < number; i++)
        {
            GameObject obj = GameObject.Instantiate(terrainPrefab);
            obj.transform.position = terrainPrefab.transform.position + new Vector3(0, 0, 500.0f * i);
        }
    }
}
