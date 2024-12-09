using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelGrid))]
public class CustomEditorVG : Editor
{
    
    public override void OnInspectorGUI()
    
    {
        VoxelGrid _target = (VoxelGrid)target;


        DrawDefaultInspector();
        if (GUILayout.Button("Test"))
        {
            _target.createGrid();
        }
    }
}