using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkScript))]
public class NetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = ((NetworkScript)target);

        if (GUILayout.Button("Update Local IP"))
            script.UpdateLocalIp();
    }
}