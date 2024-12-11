using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SmokeSource : MonoBehaviour
{
    VoxelGrid _voxelGrid;

    public Mesh voxelMesh;
    public Material voxelMaterial;
    bool emitting = false;

    public List<Vector3> arr = new List<Vector3>();

    public List<Vector3> todraw = new List<Vector3>();



    private void Start()
    {
        _voxelGrid = VoxelGrid.Instance;
    }

    /// <summary>
    /// NEXT NEED TO ADD ARR TO DRAW ARR OVER LERP TIME  TO SHOW SMOKE EXPANDING
    /// TO DRAW WILL ALWAYS BE DRAWN IN UPDATE 
    /// CAN MAKE IENUMBERABLE FOR ADDING TO LIST AS ITS QUICKER THAN INSTANTIATING
    /// 
    /// </summary>


    private void Update()
    {

        foreach (Vector2 v in todraw) {
            Graphics.DrawMesh(voxelMesh, v, Quaternion.identity, voxelMaterial, 0);
        }


    }

   
}
