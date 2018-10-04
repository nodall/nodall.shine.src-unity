using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace nexcode.nwcore
{
    [CustomEditor(typeof(NWCoreBase), true)]
    public class NWCoreBaseEditor : Editor
    {
        int selected;

        string[] options = new string[]
        {
            "MediaBrowser", "MediaCanvas", "MediaDesktopDuplication",
            "MediaImage", "MediaVideo", "MediaTextureUnity",

            "QuadSurfaceComponent",
        };

        Type[] types = new Type[]
        {
            typeof(MediaBrowser), typeof(MediaCanvas), typeof(MediaDesktopDuplication),
            typeof(MediaImage), typeof(MediaVideo), typeof(MediaTextureUnity),

            typeof(QuadSurfaceComponent)
        };

        ComponentProps[] props = new ComponentProps[]
        {
            new MediaBrowserProps(), new MediaCanvasProps(), new MediaDesktopDuplicationProps(),
            new MediaImageProps(), new MediaVideoProps(), new MediaTextureUnityProps(),

            new QuadSurfaceComponentProps()
        };

        public override void OnInspectorGUI()
        {
            var dtm = target as TextureOutput;
            base.OnInspectorGUI();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Component"))
            {
                Debug.Log("Creating component "+options[selected]);
                ComponentManager.New(types[selected], props[selected].Clone());
            }
            selected = EditorGUILayout.Popup(selected, options);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
    }

}