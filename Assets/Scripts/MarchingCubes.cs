
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{

    public Vector3 offSet;
    VoxelGridData voxelGrid;
    public int resolution = 100;
    List<Vector3> positions = new List<Vector3>();
    
    public GameObject prefab;

    private void Start()
    {

        voxelGrid.newGrid();
        voxelGrid.setRes(resolution);
        createGrid();
        testfunc();


    }

    void createGrid()
    {
        for (int z = 0; z < resolution; z++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    voxelGrid.add(scalarField(x, y, z));
                }
            }
        }
        for (int z = 0; z < resolution-1; z++)
        {
            for (int y = 0; y < resolution-1; y++)
            {
                for (int x = 0; x < resolution - 1; x++)
                {
                    MarchCube(x, y, z,voxelGrid);
                }
            }
        }
        Debug.Log(positions.Count);
    }

    public void MarchCube(int x, int y, int z, VoxelGridData voxelGrid)
    {
        int[] triangulation = getTriangulation(x, y, z, voxelGrid);
        foreach (var item in triangulation)
        {
            Debug.Log(item);
        }
        foreach (var edgeIndx in triangulation)
        {
            if (edgeIndx < 0) {
                Debug.Log("break");
                break;
            };
            Debug.Log("Not break");

            Vector2 pointIndices = Edges[edgeIndx];

            Vector3 p1 = points[(int)pointIndices.x];
            Vector3 p2 = points[(int)pointIndices.y];

            Vector3 posA = new Vector3((x+p1.x),(y+p1.y),(z+p1.z));
            Vector3 posB = new Vector3((x + p2.x), (y + p2.y), (z + p2.z));

            Vector3 position = (posA + posB) * 0.5f;
            
            positions.Add(position);
        }
           
    }

    float scalarField(float x, float y, float z)
    {
        // Define the center of the sphere
        Vector3 sphereCenter = new Vector3(resolution * 0.5f, resolution * 0.5f, resolution * 0.5f);

        // Define the radius of the sphere
        float radius = resolution * 0.25f;

        // Calculate the squared distance from the center of the sphere
        float distanceSquared = (x - sphereCenter.x) * (x - sphereCenter.x) +
                                (y - sphereCenter.y) * (y - sphereCenter.y) +
                                (z - sphereCenter.z) * (z - sphereCenter.z);

        // Return the signed distance to the surface of the sphere
        return distanceSquared - radius * radius;
    }


    void testfunc()
    {
        foreach (var item in positions)
        {
            Instantiate(prefab,item,Quaternion.identity);
        }
        
    }

    public int[] getTriangulation(int x,int y, int z, VoxelGridData voxelGrid)
    {
        byte configIndx = 0b00000000;
        
        configIndx |= (byte)((voxelGrid.read(x, y, z) < 0 ? 1 : 0) << 0);
        configIndx |= (byte)((voxelGrid.read(x, y, z+1) < 0 ? 1 : 0) << 1);
        configIndx |= (byte)((voxelGrid.read(x+1, y, z+1) < 0 ? 1 : 0) << 2);
        configIndx |= (byte)((voxelGrid.read(x+1, y, z) < 0 ? 1 : 0) << 3);
        configIndx |= (byte)((voxelGrid.read(x, y+1, z) < 0 ? 1 : 0) << 4);
        configIndx |= (byte)((voxelGrid.read(x, y+1, z+1) < 0 ? 1 : 0) << 5);
        configIndx |= (byte)((voxelGrid.read(x+1, y+1, z+1) < 0 ? 1 : 0) << 6);
        configIndx |= (byte)((voxelGrid.read(x+1, y+1, z) < 0 ? 1 : 0) << 7);
        
        return _TRIANGULATIONS.TRIANGULATIONS[configIndx];

    }


    public readonly Vector3[] points = new Vector3[8] { new Vector3(0, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 1, 1),
        new Vector3(1, 1, 1),
        new Vector3(1, 1, 0)
    };

    public readonly Vector2[] Edges = new Vector2[12]
    {
        new Vector2(0,1),
        new Vector2(1,2),
        new Vector2(2,3),
        new Vector2(3,0),
        new Vector2(4,5),
        new Vector2(5,6),
        new Vector2(6,7),
        new Vector2(7,4),
        new Vector2(0,4),
        new Vector2(1,5),
        new Vector2(2,6),
        new Vector2(3,7)
    };

    public struct VoxelGridData
    {
        List<float> Data;
        int resolution;

        public void newGrid()
        {
            Data = new List<float>();
        }

        public void setRes(int res)
        {
            resolution = res;

        }

        public void add(float value)
        {
            Data.Insert(0,value);
        }

        public float read(int x, int y, int z)
        {
            
            return Data[x + y * resolution + z * resolution * resolution];
            
        }

        public int dataCount()
        {
            return Data.Count;
        }

    }


    

}
