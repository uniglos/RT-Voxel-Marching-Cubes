using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmokeGren))]
public class GrenCustomEditor : Editor
{
    public override void OnInspectorGUI()

    {
        SmokeGren _target = (SmokeGren)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Deploy"))
        {
            _target.VoxelGrid.deploySmoke(_target.transform.position,_target.Radius);
        }

    }

}
