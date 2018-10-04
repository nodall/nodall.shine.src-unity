using UnityEngine;
using System.Collections;
using nexcode.nwcore;

public class VideoLayerResize : MonoBehaviour {

	// Use this for initialization
	void Start () {

        var hub = NWCoreBase.hub;
        hub.Subscribe("_syscmd")
            .On("videoLayerResize", (msg) =>
            {
                var x = float.Parse(msg["x"].ToString());
                var y = float.Parse(msg["y"].ToString());
                var w = float.Parse(msg["w"].ToString());
                var h = float.Parse(msg["h"].ToString());

                /*
                 * Smooth
                var r = GetComponent<NWLayer>().rectangle;
                float t = 0.2f;

                x = Mathf.Lerp(x, r.x, t);
                y = Mathf.Lerp(y, r.x, t);
                w = Mathf.Lerp(w, r.width, t);
                h = Mathf.Lerp(h, r.height, t);
                */
                //GetComponent<NWLayer>().rectangle = new Rect(x, y, w, h);
            });


    }

}
