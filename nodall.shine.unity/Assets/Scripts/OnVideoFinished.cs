using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using UnityEngine.Events;

public class OnVideoFinished : MonoBehaviour {

    public bool isFinished = false;
    public float lastTime = -1;

    public UnityEvent onEndAction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var mp = GetComponent<MediaPlayer>();
        var curTime = mp.Control.GetCurrentTimeMs();

        if (curTime < lastTime || mp.Control.IsFinished())
        {
            isFinished = true;
            onEndAction.Invoke();
        }
        lastTime = curTime;	
	}
}
