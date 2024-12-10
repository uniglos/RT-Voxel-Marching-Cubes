using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;


public class VoxelGrid : MonoBehaviour
{
    public static VoxelGrid Instance { get; private set; }

    

    VoxelGridData voxelGrid;
    public Vector3 Size = new Vector3(10, 10, 10);    

    public float voxelSize = 0.5f;
    public LayerMask mask;
    public GameObject prefab;
    public GameObject prefab2;

    public bool DrawGizmos = true;


    GameObject SmokeOrigin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

        voxelGrid = new VoxelGridData();
        voxelGrid.setRes(Size);

        createGrid();
        //drawGrid();
    }



    public void deploySmoke(Vector3 pos, float radius)
    {
        
        Vector3 position = MyMath.RoundToNearestVoxel(pos,voxelSize);
        Debug.Log(position);
        Debug.Log(voxelGrid.read(20f, 0.5f, 24.5f,voxelSize));
        if (voxelGrid.read(position.x, position.y, position.z,voxelSize) == 1) {
            
            SmokeOrigin = Instantiate(prefab2,position,Quaternion.identity);
            CheckArea(radius);
        }

    }


    public void CheckArea(float radius)
    {
        
        List<Vector3> positions = new List<Vector3>();
        for (int y = 0; y <= 5; y++)
        {            
            for (int x = -5; x < 6; x++)
            {                
                for (int z = -5 ; z < 6; z++)
                {                    
                    Vector3 testPos = new Vector3(SmokeOrigin.transform.position.x + voxelSize * x, SmokeOrigin.transform.position.y + voxelSize*y, SmokeOrigin.transform.position.z + voxelSize * z);
                    
                    if (Vector3.Distance(SmokeOrigin.transform.position, testPos) > Mathf.Sqrt(Mathf.PI)*radius)
                    {
                        continue;
                    }

                    Vector3 voxelPos = MyMath.RoundToNearestVoxel(testPos, voxelSize);
                    float indx = voxelGrid.read(voxelPos.x, voxelPos.y, voxelPos.z, voxelSize);
                    if (indx != -1 && indx == 1)
                    {
                        positions.Add(voxelPos);
                    }


                }
            }
        }
        List<Vector3> ValidPositions = VoxelPathFind.PathFind(positions.ToArray(), MyMath.RoundToNearestVoxel(SmokeOrigin.transform.position,voxelSize),voxelSize);
        SpawnVoxels(ValidPositions);
    }

    void SpawnVoxels(List<Vector3> validPositions)
    {
        Vector3[] arr = validPositions.ToArray();
       
        arr = arr.OrderBy((d) => (d - SmokeOrigin.transform.position).sqrMagnitude).ToArray();
        

        StartCoroutine(interp(arr));
        
    }

    public IEnumerator interp(Vector3[] arr)
    {
        foreach (Vector3 validPos in arr)
        {
            Instantiate(prefab2, validPos, Quaternion.identity, SmokeOrigin.transform);
            yield return new WaitForSeconds(0.01f);
        }
        
    }
   

    private void Update()
    {
        //createGrid();
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
        Collider[] hit = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(voxelSize*0.5f,voxelSize* 0.5f, voxelSize * 0.5f), Quaternion.identity,mask);
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


