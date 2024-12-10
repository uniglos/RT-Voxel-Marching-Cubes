using NUnit.Framework;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class VoxelGrid : MonoBehaviour
{
    VoxelGridData voxelGrid;
    public Vector3 Size = new Vector3(10, 10, 10);    

    public float voxelSize = 0.5f;
    public GameObject prefab;
    public GameObject prefab2;

    public bool DrawGizmos = true;

    private void Start()
    {
        Debug.Log("AAA");
        voxelGrid = new VoxelGridData();
        voxelGrid.setRes(Size);

        Debug.Log("start");
        //createGrid();
        //drawGrid();
    }



    public void deploySmoke(Vector3 pos)
    {
        Vector3 position = MyMath.RoundToNearestVoxel(pos,voxelSize);
        if(voxelGrid.read(position.x, position.y, position.z,voxelSize) == 1) {
            Instantiate(prefab2,position,Quaternion.identity);
        }
    }












    private void Update()
    {
        createGrid();
    }

    public void createGrid()
    {
        voxelGrid.newGrid();
        //Starts at 0,0,0
        for (float z = 0; z < Size.z; z+= voxelSize)
        {            
            for (float y = 0; y < Size.y; y +=voxelSize)
            {                
                for (float x = 0; x < Size.x; x += voxelSize)
                {
                    
                    voxelGrid.add(scalarField(x, y, z));
                }
            }
        }
        
    }


    float scalarField(float x, float y, float z)
    {
        Collider[] hit = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(voxelSize*0.5f,voxelSize* 0.5f, voxelSize * 0.5f), Quaternion.identity);
        if (hit.Length > 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }



    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            for (float z = 0; z < Size.z; z += voxelSize)
            {

                for (float y = 0; y < Size.y; y += voxelSize)
                {

                    for (float x = 0; x < Size.x; x += voxelSize)
                    {

                        Gizmos.color = new Color(voxelGrid.read(x, y, z, voxelSize), voxelGrid.read(x, y, z, voxelSize), voxelGrid.read(x, y, z, voxelSize));
                        Gizmos.DrawSphere(new Vector3(x, y, z), 0.15f);

                    }
                }
            }
        }
    }

    //void drawGrid()
    //{
    //    for (float z = 0; z < Size.z; z += voxelSize)
    //    {

    //        for (float y = 0; y < Size.y; y += voxelSize)
    //        {

    //            for (float x = 0; x < Size.x; x += voxelSize)
    //            {
    //                if (voxelGrid.read(x, y, z, voxelSize) == 1)
    //                {
    //                    Instantiate(prefab, new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), Quaternion.identity);
    //                }
    //            }
    //        }
    //    }
    //}


    public struct VoxelGridData
    {
        List<float> Data;
        Vector3 Size;

        public void newGrid()
        {
            Data = new List<float>((int)(Size.x * Size.y * Size.z));
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

        public float read(float x, float y, float z, float voxelSize)
        {
            int ix = Mathf.FloorToInt(x / voxelSize);
            int iy = Mathf.FloorToInt(y / voxelSize);
            int iz = Mathf.FloorToInt(z / voxelSize);

            int gridWidth = Mathf.FloorToInt(Size.x / voxelSize);
            int gridHeight = Mathf.FloorToInt(Size.y / voxelSize);

            int index = ix + gridWidth * (iy + gridHeight * iz);

            if (index >= 0 && index < Data.Count)
            {
                return Data[index];
            }
            else
            {
                return -1;
            }
        }

        public int dataCount()
        {
            return Data.Count;
        }
    }



}


