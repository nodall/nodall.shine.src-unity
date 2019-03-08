using Newtonsoft.Json;
using nexcode.nwcore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditorAppManager : MonoBehaviour {

    DateTime lastSelectedTime;

    public float curGridAlpha = 0;
    float lastGridAlpha = -1;

    string surfaceSelected;

    Texture2D texCircleTarget;

    [Serializable]
    public class SurfacePoint
    {
        public string surfaceId;
        public int pointIndex;
    }

    public List<SurfacePoint> pointsSelected = new List<SurfacePoint>();

	// Use this for initialization
	void Start () {

        texCircleTarget = Resources.Load<Texture2D>("circle-target");

        NWCore.hub.Subscribe("stageeditor")
            .On("surfaceSelected", (msg) =>
            {
                Debug.LogWarning("[StageEditorApp] SurfaceSelected "+msg);
                surfaceSelected = msg.ToString();
                lastGridAlpha = -1;
                lastSelectedTime = DateTime.Now;

            })
            .On("pointsSelected", (msg) =>
            {
                Debug.LogWarning("[StageEditorApp] Points Selected ");
                pointsSelected = msg.ToObject<List<SurfacePoint>>();

            })
            .On("gridOn", (msg) =>
            {
                lastSelectedTime = DateTime.Now;
            })
            .On("gridOff", (msg) =>
            {
                Debug.LogWarning("[StageEditorApp] Close - Grid Off");
                pointsSelected.Clear();
                lastSelectedTime = DateTime.MinValue;
            });

        ComponentManager.OnComponentCreate += (s, e) =>
        {
            var surface = e.Component as QuadSurfaceComponent;
            if (surface != null)
                lastGridAlpha = -1;
        };
	}

    // Update is called once per frame
    void Update () {

        if ((DateTime.Now - lastSelectedTime).TotalSeconds < 1)
        {
            curGridAlpha += 0.02f;
            curGridAlpha *= 1.05f;
        }

        if ((DateTime.Now - lastSelectedTime).TotalSeconds > 4)
            curGridAlpha *= 0.993f;

        if (curGridAlpha > 1)
            curGridAlpha = 1;
        if (curGridAlpha < 0.01)
            curGridAlpha = 0;

        if (curGridAlpha != lastGridAlpha)
        {
            // Update alpha in surfaces
            var surfaces = ComponentManager.GetAll<QuadSurfaceComponent>();
            foreach (var s in surfaces)
            {
                var renderer = s.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                    renderer.material.SetFloat("_FadeFX", curGridAlpha * (s.name == surfaceSelected ? 2.5f : 1f));
            }
        }

        lastGridAlpha = curGridAlpha;

	}

    private void OnGUI()
    {
        if (pointsSelected == null || pointsSelected.Count == 0)
            return;

        GUI.color = new Color(1, 1, 1, curGridAlpha * 0.7f);
        var sizeCircle = 48;

        foreach (var pts in pointsSelected)
        {
            if (pts.surfaceId == null)
                continue;

            var surface = ComponentManager.GetById(pts.surfaceId) as QuadSurfaceComponent;
            if (surface == null)
                continue;

            var pt = surface.props.bezier.vectors[pts.pointIndex];

            var px = pt.x * Screen.width - sizeCircle / 2;
            var py = pt.y * Screen.height - sizeCircle / 2;
            GUI.DrawTexture(new Rect(px, py, sizeCircle, sizeCircle), texCircleTarget);
        }
    }
}
