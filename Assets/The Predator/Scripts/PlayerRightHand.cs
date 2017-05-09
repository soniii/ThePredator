using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Oculus.Avatar;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerRightHand : MonoBehaviour {

	public GameObject mArrowObject;
	public Transform mBowCenter;

	private GameObject mStringGrabBox;
	private GameObject mCurrentArrow;
	private bool mCanGrab;
	private bool mHoldingString;

	void OnTriggerEnter (Collider col) {
		Debug.Log ("sonydb: trigger hand");
		if (col.name == "Quiver") {
			mCanGrab = true;
		} else if (col.name == "StringGrabBox" && mCurrentArrow != null) {
			mStringGrabBox = col.gameObject;
			mStringGrabBox.SendMessage ("StartHandGrab", this.gameObject);
			mCurrentArrow.SendMessage ("AttachToString", mStringGrabBox);
			mHoldingString = true;
		}
	}

	void OnTriggerStay (Collider col) {
		Debug.Log ("sonydb: trigger hand stay");
		if (col.name == "Quiver") {
			mCanGrab = true;
		} else if (col.name == "StringGrabBox" && mCurrentArrow != null) {
			mHoldingString = true;
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.name == "Quiver") {
			mCanGrab = false;
		} else if (col.name == "StringGrabBox" && mCurrentArrow != null) {
			ShootArrow ();
		}
	}

	void ShootArrow () {
		mCurrentArrow.SendMessage ("Shoot", (mBowCenter.position - mStringGrabBox.transform.position).magnitude);
		mStringGrabBox.SendMessage ("ReleaseHandGrab");
		mCurrentArrow.transform.SetParent (null);
		mCurrentArrow = null;
		mStringGrabBox = null;
		mHoldingString = false;
	}

	void DropArrow() {
		mCurrentArrow.SendMessage ("Fall");
		mCurrentArrow.transform.SetParent (null);
		mCurrentArrow = null;
	}

	// Use this for initialization
	void Start () {
		mCurrentArrow = null;
		mCanGrab = false;
		mHoldingString = false;
	}

	// Update is called once per frame
	void Update () {
		if (OVRInput.GetDown (OVRInput.RawButton.RIndexTrigger)) {
			Debug.Log ("sonydb: pressed down");
			if (mArrowObject != null &&
			    mCanGrab) {
				Debug.Log ("sonydb: grabbing arrow!");
				mCurrentArrow = (GameObject)Instantiate (mArrowObject, transform.position, transform.rotation);
				mCurrentArrow.transform.SetParent (transform);
			}
		} else if (OVRInput.GetUp (OVRInput.RawButton.RIndexTrigger)) {
			if (mHoldingString &&
			    mCurrentArrow != null &&
				mBowCenter != null) {
				ShootArrow ();
			} else if (mCurrentArrow != null) {
				DropArrow ();
			}
		}
	}
}
