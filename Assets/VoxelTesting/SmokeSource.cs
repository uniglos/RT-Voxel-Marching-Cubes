using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SmokeSource : MonoBehaviour
{
    VoxelGrid _voxelGrid;

    public Mesh voxelMesh;
    public Material voxelMaterial;
    bool emitting = false;

    public List<Vector3> array = new List<Vector3>();

    public List<Vector3> todraw = new List<Vector3>();



    private void Start()
    {
        _voxelGrid = VoxelGrid.Instance;

        
    }

    public void StartSmoke(List<Vector3> arr)
    {
        List<Vector3> a = new List<Vector3>();
        List<Vector3> b = new List<Vector3>();
        for (int i = 0; i < arr.Count * 0.5f; i++)
        {
            if (i % 2 == 0)
            {
                a.Add(arr[i]);
            }
            else
            {
                b.Add(arr[i]);
            }
        }
        StartCoroutine(BeginDraw(a));
        StartCoroutine(BeginDraw(b));
    }

    IEnumerator BeginDraw(List<Vector3> arr)
    {
        float time = 0.00001f;
        for (int i = 0; i < arr.Count; i++) {
            todraw.Add(arr[i]);
            if (i > arr.Count * 0.9f)
            {
                time = ease(time);
            }
            yield return new WaitForSeconds(time);
        }
    }


    float ease(float time)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(time - 1, 2));
    }


    private void Update()
    {

        foreach (Vector3 v in todraw)
        {
            Graphics.DrawMesh(voxelMesh, v, Quaternion.identity, voxelMaterial, 0);
        }
        foreach (Vector3 v in array)
        {
            Graphics.DrawMesh(voxelMesh, v, Quaternion.identity, voxelMaterial, 0);
        }


    }

   
}
