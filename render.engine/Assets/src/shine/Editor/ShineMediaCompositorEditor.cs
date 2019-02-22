using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShineMediaCompositor))]
public class ShineMediaCompositorEditor : Editor {

    public override void OnInspectorGUI()
    {
        var shine = target as ShineMediaCompositor;
        base.OnInspectorGUI();
        GUILayout.Space(20);
        GUILayout.Label("Debug Shine Compositor");
        if (GUILayout.Button("Show Desktop Duplication"))
        {
            shine.Play(0, "desktkop", null, null);
        }

        if (GUILayout.Button("Hide Desktop Duplication"))
        {
            shine.Stop(0);
        }

    }
}
