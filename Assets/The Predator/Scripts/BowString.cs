using UnityEngine;
using System.Collections;

public class BowString : MonoBehaviour {

	// Public
	public Transform mUpperStringAnchor;
	public Transform mLowerStringAnchor;
	public Transform mBowCenter;

	public GameObject mStringObject;

	public float mMaxDistance = 16f;
	public float mStringRadius = 0.0075f;

	// Private
	private Transform mHand;
	private GameObject mUpperString;
	private GameObject mLowerString;

	void StartHandGrab(Transform hand) {
		mHand = hand;
	}

	void ReleaseHandGrab() {
		mHand = null;
	}

	// Use this for initialization
	void Start () {
		tag = "Bow";
		mHand = null;

		mUpperString = null;
		mLowerString = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (mHand != null &&
			mUpperStringAnchor != null &&
			mLowerStringAnchor != null &&
			mBowCenter != null &&
			mStringObject != null) {
			Vector3 desiredLine = mHand.position - mBowCenter.position;
			transform.position = mHand.position;
			if (desiredLine.magnitude > mMaxDistance) {
				transform.position = mBowCenter.position + desiredLine.normalized * mMaxDistance;
			}
		}
		// Create strings
		CreateStringBetween2Points (mUpperStringAnchor.position, transform.position, ref mUpperString);
		CreateStringBetween2Points (mLowerStringAnchor.position, transform.position, ref mLowerString);
	}

	void CreateStringBetween2Points(Vector3 point1, Vector3 point2, ref GameObject bowString) {
		Vector3 offset = point2 - point1;
		Vector3 scale = new Vector3 (mStringRadius, offset.magnitude / 2f, mStringRadius);
		Vector3 position = point1 + offset / 2f;

		if (bowString == null) {
			bowString = (GameObject)Instantiate (mStringObject, position, Quaternion.identity);
			bowString.transform.SetParent (transform.parent);
		}
		bowString.transform.position = position;
		bowString.transform.up = offset.normalized;
		bowString.transform.localScale = scale;
	}
}
