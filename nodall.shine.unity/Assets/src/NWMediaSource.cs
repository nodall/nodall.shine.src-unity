using UnityEngine;
using System.Collections;

public class NWMediaSource : MonoBehaviour {

    public Texture Texture { get; set; }
    public bool NeedsFlipY { get; set; }
    public bool IsVisible { get; set; }
    public double UnusedSeconds { get { return (System.DateTime.Now - lastUsedTime).TotalSeconds; } }

    System.DateTime lastUsedTime = System.DateTime.Now;

    public double timeUnused;

    // Use this for initialization
    void Start () {
        Touch();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Unused "+gameObject.name+ " " + UnusedSeconds);

        if (UnusedSeconds > 5)
        {
            //Debug.Log("DELETE " + gameObject.name);
            //Destroy(this.gameObject);
        }	
	}

    public void Touch()
    {
        //Debug.Log("TOUCH " + gameObject.name);
        lastUsedTime = System.DateTime.Now;
        timeUnused = -1;
    }

}
