
using System;
using System.Collections.Generic;

using UnityEngine;

using MCD = MarchingCubeData;

public class MarchingCubes : MonoBehaviour
{

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private Mesh mesh;

    Vector3 offset;
    public Vector3 offSet;
    VoxelGridData voxelGrid;
    public int resolution = 100;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;

    private void Start()
    {        
        offset = transform.position;
        voxelGrid.newGrid();
        voxelGrid.setRes(resolution);
        createGrid();
        calcMesh();
    }

    void calcMesh()
    {
        for (int z = 0; z < resolution - 1; z++)
        {
            for (int y = 0; y < resolution - 1; y++)
            {
                for (int x = 0; x < resolution - 1; x++)
                {
                    MarchCube(x, y, z, voxelGrid);
                }
            }
        }

        BuildMesh();
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
        
    }

    public void MarchCube(int x, int y, int z, VoxelGridData voxelGrid)    {
        
        int[] triangulation = getTriangulation(x, y, z, voxelGrid);
        positions = new List<Vector3>();
        foreach (var edgeIndx in triangulation)
        {
            if (edgeIndx < 0) {
                break;
            };

            Vector2 pointIndices = MCD.Edges[edgeIndx];

            Vector3 p1 = MCD.points[(int)pointIndices.x];
            Vector3 p2 = MCD.points[(int)pointIndices.y];

            Vector3 posA = new Vector3((x + p1.x), (y + p1.y), (z + p1.z));
            Vector3 posB = new Vector3((x + p2.x), (y + p2.y), (z + p2.z));

            Vector3 position = (posA + posB) * 0.5f;
            
            positions.Add(position);

            // Add vertex to list and record its index
            int vertexIndex = vertices.Count;
            vertices.Add(position);

            triangles.Add(vertexIndex);
            
        }        

    }


    void BuildMesh()
    {
        // Create a new mesh object
        mesh = new Mesh();

        // Assign vertices and triangles
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        // Assign mesh to the MeshFilter
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;

        // Optionally assign a material
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }



    float scalarField(float x, float y, float z)
    {        
        float xcoord = x / resolution;
        float ycoord = y / resolution;
        float zcoord = z / resolution;

        return MyMath.ClampedRemap(MyMath.PerlinNoise3D(xcoord + offset.x, ycoord + offset.y, zcoord + offset.z),0,1,-1,1) ;
    }  

    public int[] getTriangulation(int x,int y, int z, VoxelGridData voxelGrid)
    {
        byte configIndx = 0b00000000;
        
        for (int i = 0; i < 8; i++)
        {
            Vector3 vec = new Vector3(x,y,z) + MCD.points[i];
            configIndx |= (byte)((voxelGrid.read((int)vec.x, (int)vec.y, (int)vec.z) < 0 ? 1 : 0) << i);
        }
        //configIndx |= (byte)((voxelGrid.read(x, y, z) < 0 ? 1 : 0) << 0);
        //configIndx |= (byte)((voxelGrid.read(x, y, z+1) < 0 ? 1 : 0) << 1);
        //configIndx |= (byte)((voxelGrid.read(x+1, y, z+1) < 0 ? 1 : 0) << 2);
        //configIndx |= (byte)((voxelGrid.read(x+1, y, z) < 0 ? 1 : 0) << 3);
        //configIndx |= (byte)((voxelGrid.read(x, y+1, z) < 0 ? 1 : 0) << 4);
        //configIndx |= (byte)((voxelGrid.read(x, y+1, z+1) < 0 ? 1 : 0) << 5);
        //configIndx |= (byte)((voxelGrid.read(x+1, y+1, z+1) < 0 ? 1 : 0) << 6);
        //configIndx |= (byte)((voxelGrid.read(x+1, y+1, z) < 0 ? 1 : 0) << 7);
        
        return MCD.TRIANGULATIONS[configIndx];

    }    

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
