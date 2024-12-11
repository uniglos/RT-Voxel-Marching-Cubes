using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGrid))]
public class CustomEditorVG : Editor
{
    
    public override void OnInspectorGUI()
    
    {
        VoxelGrid _target = (VoxelGrid)target;


        DrawDefaultInspector();
        if (GUILayout.Button("Update Grid"))
        {
            _target.createGrid();
        }
        if (GUILayout.Button("Test Deploy"))
        {
            _target.deploySmoke(new Vector3(23.882f,0.631f,15.556f),0.5f,5,200);
        }

    }
}