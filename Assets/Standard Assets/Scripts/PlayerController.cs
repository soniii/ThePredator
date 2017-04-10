using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	// Public variables
	public float mSpeed = 1f;
	public float mJumpForce = 100f;
	public float mTurnSensitivity = 15f;

	// Private
	private Transform mCameraAnchor;
	private Vector3 mBottomAnchor;
	private RaycastHit mGround;
	private Vector3 mCurrentDirection;
	private Vector3 mForward;
	private Vector3 mRight;

	private Quaternion mOriginalRotation;
	private Quaternion mCameraRotation;
	private float mRotationX;
	private float mRotationY;

	private bool mIsGrounded;
	private float mJumpVelocity;

	void Start () {
		gameObject.tag = "Player";
		mCameraAnchor = transform.Find ("CameraAnchor");
		if (mCameraAnchor) {
			Debug.Log ("sonydb: found anchor");
		}
		mBottomAnchor = transform.position;

		mCurrentDirection = transform.forward;
		mForward = transform.forward;
		mRight = Vector3.Cross (transform.forward, transform.up).normalized;

		mOriginalRotation = transform.localRotation;
		mCameraRotation = mCameraAnchor.localRotation;
		mRotationX = 0f;
		mRotationY = 0f;

		mIsGrounded = false;
		mJumpVelocity = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		mBottomAnchor = transform.position;
		mRotationX += Input.GetAxis ("Mouse X") * mTurnSensitivity;
		mRotationY -= Input.GetAxis ("Mouse Y") * mTurnSensitivity / 2f;
		mRotationX = ClampAngle (mRotationX);
		mRotationY = Mathf.Clamp (mRotationY, -40f, 20f);
		Quaternion xQuaternion = Quaternion.AngleAxis (mRotationX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis (mRotationY, Vector3.right);

		transform.localRotation = mOriginalRotation * xQuaternion;
		mCameraAnchor.localRotation = mCameraRotation * yQuaternion;


		mRight = Vector3.Cross (transform.up, transform.forward).normalized;
		mForward = transform.forward;
		if (Physics.Raycast (mBottomAnchor, Vector3.down, out mGround, 1f, LayerMask.GetMask("Terrain"))) {
			mRight = Vector3.Cross (mGround.normal, transform.forward).normalized;
			mForward = Vector3.Cross (mRight, mGround.normal).normalized;
			mIsGrounded = true;
		} else {
			mIsGrounded = false;
		}

		Debug.DrawRay (mBottomAnchor, Vector3.down, Color.red);

		if (mIsGrounded) {
			mJumpVelocity = 0f;
			mCurrentDirection = new Vector3 (0f, 0f, 0f);
			Vector3 tmpDirection = new Vector3 (0f, 0f, 0f);
			if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				tmpDirection = tmpDirection + mForward;
			}

			if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				tmpDirection = tmpDirection + mRight;
			}

			if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				tmpDirection = tmpDirection + mRight * (-1f);
			}

			if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				tmpDirection = tmpDirection + mForward * (-1f);
			}

			if (Input.GetKey (KeyCode.Space) && mJumpVelocity == 0f) {
				mCurrentDirection = (new Vector3(tmpDirection.x, 0f, tmpDirection.z)).normalized;
				mJumpVelocity += 10f;
			}

			tmpDirection.Normalize ();
			transform.position += tmpDirection * mSpeed * Time.deltaTime;

		} else {
			// Fall velocity
			mJumpVelocity -= 40f * Time.deltaTime;
		}

		transform.position += (Vector3.up * mJumpVelocity + mCurrentDirection * mSpeed) * Time.deltaTime;
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
