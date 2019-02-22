using UnityEngine;
using System.Collections;

public class DesktopDuplicationSource : NWMediaSource {

    uDesktopDuplication.Texture ddTexture;
    public Texture texture;

	// Use this for initialization
	void Start () {
        NeedsFlipY = true;
        ddTexture = GetComponent<uDesktopDuplication.Texture>();

        StartCoroutine(CoStart());
	}

    IEnumerator CoStart()
    {
        yield return new WaitForSeconds(0.2f);
        ddTexture.monitorId = 0;
    }
	
	// Update is called once per frame
	void Update () {
        Texture = ddTexture.monitor.texture;
        texture = Texture;
	}
}
