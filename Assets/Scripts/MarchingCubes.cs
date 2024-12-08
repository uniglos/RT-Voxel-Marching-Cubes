
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TreeEditor;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

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
        
        Debug.Log("Done");


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

    public void MarchCube(int x, int y, int z, VoxelGridData voxelGrid)    {
        
        int[] triangulation = getTriangulation(x, y, z, voxelGrid);
        positions = new List<Vector3>();
        foreach (var edgeIndx in triangulation)
        {
            if (edgeIndx < 0) {
                break;
            };

            Vector2 pointIndices = Edges[edgeIndx];

            Vector3 p1 = points[(int)pointIndices.x];
            Vector3 p2 = points[(int)pointIndices.y];

            Vector3 posA = new Vector3((x + p1.x), (y + p1.y), (z + p1.z));
            Vector3 posB = new Vector3((x + p2.x), (y + p2.y), (z + p2.z));

            Vector3 position = (posA + posB) * 0.5f;
            
            positions.Add(position);

            // Add vertex to list and record its index
            int vertexIndex = vertices.Count;
            vertices.Add(position);

            triangles.Add(vertexIndex);
            //// Add triangle indices (triangles are defined in sets of 3)
            //// Ensure CCW winding order
            //if (triangles.Count % 3 == 0) // Start of a new triangle
            //{
            //    triangles.Add(vertexIndex);
            //}
            //else if (triangles.Count % 3 == 1)
            //{
            //    triangles.Add(vertexIndex);
            //}
            //else
            //{
            //    triangles.Add(vertexIndex);
            //    ReverseLastTriangle(); // Reverse if winding is wrong
            //}
        }        

    }

    private void ReverseLastTriangle()
{
    int i = triangles.Count - 3;
    int temp = triangles[i + 1];
    triangles[i + 1] = triangles[i + 2];
    triangles[i + 2] = temp;
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
        //// Define the center of the sphere
        //Vector3 sphereCenter = new Vector3(resolution * 0.5f, resolution * 0.5f, resolution * 0.5f);

        //// Define the radius of the sphere
        //float radius = resolution * 0.25f;

        //// Calculate the squared distance from the center of the sphere
        //float distanceSquared = (x - sphereCenter.x) * (x - sphereCenter.x) +
        //                        (y - sphereCenter.y) * (y - sphereCenter.y) +
        //                        (z - sphereCenter.z) * (z - sphereCenter.z);

        //// Return the signed distance to the surface of the sphere
        //return distanceSquared - radius * radius;
        float xcoord = x / resolution;
        float ycoord = y / resolution;
        float zcoord = z / resolution;

        return ClampedRemap(PerlinNoise3D(xcoord, ycoord,zcoord),0,1,-1,1) ;

        //return y - 4;
    }

    public static float PerlinNoise3D(float x, float y, float z)
    {
        y += 1;
        z += 2;
        float xy = _perlin3DFixed(x, y);
        float xz = _perlin3DFixed(x, z);
        float yz = _perlin3DFixed(y, z);
        float yx = _perlin3DFixed(y, x);
        float zx = _perlin3DFixed(z, x);
        float zy = _perlin3DFixed(z, y);

        return xy * xz * yz * yx * zx * zy;
    }

    static float _perlin3DFixed(float a, float b)
    {
        return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
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

    public static float ClampedRemap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        // Clamp the input value to the input range first
        float clampedValue = Mathf.Clamp(value, inputMin, inputMax);

        // Remap the clamped value to the output range
        float remappedValue = outputMin + (clampedValue - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);

        // Return the remapped value
        return remappedValue;
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
