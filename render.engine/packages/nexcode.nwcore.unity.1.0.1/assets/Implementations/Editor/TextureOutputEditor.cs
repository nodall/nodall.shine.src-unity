using UnityEngine;
using System.Collections;
using UnityEditor;
using nexcode.nwcore;

[CustomEditor(typeof(TextureOutput), true)]
public class TextureOutputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dtm = target as TextureOutput;
        base.OnInspectorGUI();

        GUILayout.Space(10);

        var tex = dtm.Texture;
        var width = EditorGUIUtility.currentViewWidth - 50;

        if (tex != null)
        {
            //var guiStyle = 
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            GUILayout.Label("<b>" + dtm.CurrentMedia.GetType().Name + "</b>: " + tex.width + "x"+tex.height, style);
            if (dtm.CurrentMedia is INeedsVerticalFlip)
                GUILayout.Label("Needs vertical flip: " + (dtm.CurrentMedia as INeedsVerticalFlip).NeedsVerticalFlip);

            if (dtm.CurrentMedia is IUpdateable)
                GUILayout.Label("Is updateable");

            GUILayout.Label(dtm.Texture, GUILayout.Width(width), GUILayout.Height(width * tex.width/tex.height));
        }

        EditorUtility.SetDirty(target);
    }
}
