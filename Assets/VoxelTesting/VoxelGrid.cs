using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class VoxelGrid : MonoBehaviour
{
    VoxelGridData voxelGrid;
    public Vector3 Size = new Vector3(10, 10, 10);    

    public GameObject prefab;

    private void Start()
    {
        Debug.Log("AAA");
        voxelGrid = new VoxelGridData();
        voxelGrid.setRes(Size);

        Debug.Log("start");
        createGrid();
    }


    private void Update()
    {
        //createGrid();
    }

    public void createGrid()
    {
        voxelGrid.newGrid();
        //Starts at 0,0,0
        for (int z = 0; z < Size.z; z++)
        {
            Debug.Log("Z");
            for (int y = 0; y < Size.y; y++)
            {
                Debug.Log("Y");
                for (int x = 0; x < Size.x; z++)
                {
                    Debug.Log("X");
                    voxelGrid.add(scalarField(x, y, z));
                }
            }
        }

    }

    float scalarField(float x, float y, float z)
    {
        Collider[] hit = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(.75f, 0.75f, 0.75f), Quaternion.identity);
        if (hit.Length > 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

 




    public struct VoxelGridData
    {
        List<float> Data;
        Vector3 Size;

        public void newGrid()
        {
            Data = new List<float>();
        }

        public void setRes(Vector3 res)
        {
            Size = res;

        }

        public void add(float value)
        {
            //Data.Insert(0, value);
            Data.Add(value);
        }

        public float read(int x, int y, int z)
        {

            return Data[(int)(x)  + (int)(y) * (int)Size.y + (int)(z) * (int)Size.z * (int)Size.x];

        }

        public int dataCount()
        {
            return Data.Count;
        }
    }



}


