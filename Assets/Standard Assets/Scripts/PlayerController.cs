using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// Public variables
	public float mSpeed = 1f;
	public float mTurnSensitivity = 15f;

	// Private
	private Vector3 mBottomAnchor;
	private RaycastHit mGround;
	private Vector3 mForward;
	private Vector3 mRight;

	private Quaternion mOriginalRotation;
	private float mRotationX;

	void Start () {
		gameObject.tag = "Player";
		mBottomAnchor = transform.position;

		mForward = transform.forward;
		mRight = Vector3.Cross (transform.forward, transform.up).normalized;
		mRotationX = 0f;
		mOriginalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		mBottomAnchor = transform.position;
		mRotationX += Input.GetAxis ("Mouse X") * mTurnSensitivity;
		mRotationX = ClampAngle (mRotationX);
		Quaternion xQuaternion = Quaternion.AngleAxis (mRotationX, Vector3.up);

		transform.localRotation = mOriginalRotation * xQuaternion;
		//Debug.Log ("sonydb: forward = " + transform.forward.x + ", " + transform.forward.y + ", " + transform.forward.z);

		mRight = Vector3.Cross (transform.up, transform.forward).normalized;
		mForward = transform.forward;
		if (Physics.Raycast (mBottomAnchor, Vector3.down, out mGround, LayerMask.NameToLayer ("Terrain"))) {
			//Debug.Log ("sonydb: object name = " + mGround.collider.gameObject.name);
			//Debug.Log ("sonydb: anchor hit normal: " + mGround.normal.x + ", " + mGround.normal.y + ", " + mGround.normal.z);
			mRight = Vector3.Cross (mGround.normal, transform.forward).normalized;
			//Debug.Log ("sonydb: right: " + mRight.x + ", " + mRight.y + ", " + mRight.z);
			mForward = Vector3.Cross (mRight, mGround.normal).normalized;
			//Debug.Log ("sonydb: forward: " + mForward.x + ", " + mForward.y + ", " + mForward.z);
		}
		Debug.DrawRay (mBottomAnchor + Vector3.up * 0.1f, Vector3.down, Color.red);

		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.position += mForward * mSpeed;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.position += mRight * mSpeed;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.position += mRight * (-mSpeed);
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			transform.position += mForward * (-mSpeed);
		}
	}

	public static float ClampAngle (float angle) {
		if (angle < -360f) {
			angle += 360f;
		} else if (angle > 360f) {
			angle -= 360f;
		}

		return Mathf.Clamp (angle, -360f, 360f);
	}
}
