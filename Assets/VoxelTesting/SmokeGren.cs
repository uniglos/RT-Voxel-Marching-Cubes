using UnityEngine;

public class SmokeGren : MonoBehaviour
{
    public VoxelGrid VoxelGrid;
    public float Radius;

    private void Start()
    {
        VoxelGrid = VoxelGrid.Instance;
        //VoxelGrid.deploySmoke(transform.position);
    }


}
