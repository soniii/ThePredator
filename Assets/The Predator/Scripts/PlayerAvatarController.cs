using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Oculus.Avatar;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerAvatarController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.Get (OVRInput.RawButton.RIndexTrigger)) {
			Debug.Log ("sonydb: success for the day...");
		}
	}
}
