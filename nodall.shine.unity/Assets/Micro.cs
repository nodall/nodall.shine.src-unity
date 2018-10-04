using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Micro : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("...........");
        foreach(var device in Microphone.devices)
        Debug.Log(device);

        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start(Microphone.devices[0], true, 1000, 44100);
        aud.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
