using UnityEngine;

public class SmokeGren : MonoBehaviour
{
    public VoxelGrid VoxelGrid;
    public float Radius;
    public int maxSmokeSize;
    public int defuse;
    private void Start()
    {
        VoxelGrid = VoxelGrid.Instance;
        //VoxelGrid.deploySmoke(transform.position);
    }


}
