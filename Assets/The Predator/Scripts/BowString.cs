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

	public float mReboundDuration = 0.5f;

	// Private
	private GameObject mHand;
	private GameObject mUpperString;
	private GameObject mLowerString;

	private float mEndReboundTime = 0f;

	void StartHandGrab(GameObject hand) {
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
		    mBowCenter != null) {
			Vector3 desiredLine = mHand.transform.position - mBowCenter.position;
			transform.position = mHand.transform.position;
			if (desiredLine.magnitude > mMaxDistance) {
				transform.position = mBowCenter.position + desiredLine.normalized * mMaxDistance;
			}
		} else if (mHand == null &&
		           mUpperStringAnchor != null &&
		           mLowerStringAnchor != null) {
			Vector3 restPoint = (mUpperStringAnchor.position + mLowerStringAnchor.position) / 2f;
			if ((transform.position - restPoint).magnitude > 0.01f) {
				if (mEndReboundTime < Time.time) {
					mEndReboundTime = Time.time + mReboundDuration;
				}
				float progress = 1f - (mEndReboundTime - Time.time) / mReboundDuration;
				transform.position = Vector3.Lerp (transform.position, restPoint, progress);
			}
		}

		if (mBowCenter != null) {
			transform.LookAt (mBowCenter.position);
		}
			
		// Create strings
		CreateStringBetween2Points (mUpperStringAnchor.position, transform.position, ref mUpperString);
		CreateStringBetween2Points (mLowerStringAnchor.position, transform.position, ref mLowerString);
	}

	void CreateStringBetween2Points(Vector3 point1, Vector3 point2, ref GameObject bowString) {
		Vector3 offset = point2 - point1;
		Vector3 scale = new Vector3 (mStringRadius, offset.magnitude / 2f, mStringRadius);
		Vector3 position = point1 + offset / 2f;

		if (bowString == null && mStringObject != null) {
			bowString = (GameObject)Instantiate (mStringObject, position, Quaternion.identity);
			bowString.transform.SetParent (transform.parent);
		}
		bowString.transform.position = position;
		bowString.transform.up = offset.normalized;
		bowString.transform.localScale = scale;
	}
}
