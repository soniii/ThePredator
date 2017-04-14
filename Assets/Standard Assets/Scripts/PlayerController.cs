using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	// Public variables
	public float mSpeed = 1f;
	public float mJumpForce = 100f;
	public float mTurnSensitivity = 15f;
	public float mBaseOffset = 0.25f;

	// Private
	private Rigidbody mRigidBody;

	private Transform mCameraAnchor;
	private Vector3 mBottomAnchor;

	private Vector3 mForwardVelocity;
	private Vector3 mCurrentDirection;
	private Vector3 mForward;
	private Vector3 mRight;

	private Quaternion mOriginalRotation;
	private Quaternion mCameraRotation;
	private float mRotationX;
	private float mRotationY;

	private bool mIsGrounded;
	private float mJumpVelocity;

	void OnCollisionEnter(Collision col) {
		Debug.Log ("sonydb: Player hit");
	}

	void Start () {
		// Initialize
		gameObject.tag = "Player";
		mRigidBody = GetComponent<Rigidbody> ();
		mRigidBody.constraints = RigidbodyConstraints.FreezeAll;

		mCameraAnchor = transform.Find ("CameraAnchor");
		if (mCameraAnchor) {
			Debug.Log ("sonydb: found anchor");
		}
		mBottomAnchor = transform.position + new Vector3(0f, -mBaseOffset, 0f);

		mForwardVelocity = transform.forward;
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
		mBottomAnchor = transform.position + new Vector3(0f, -mBaseOffset, 0f);
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
		RaycastHit mGround;
		RaycastHit mFront;
		bool mHitGround = Physics.Raycast (mBottomAnchor, Vector3.down, out mGround, 5.25f, LayerMask.GetMask("Terrain"));
		if (mHitGround) {
			mRight = Vector3.Cross (mGround.normal, transform.forward).normalized;
			mForward = Vector3.Cross (mRight, mGround.normal).normalized;
			if (mGround.distance > 0.25f && mGround.distance < 0.6f) {
				mIsGrounded = true;
			} else {
				mIsGrounded = false;
			}
		} else {
			mIsGrounded = false;
		}

		Debug.DrawRay (mBottomAnchor, Vector3.down * 5f, Color.red);

		if (mIsGrounded) {
			mJumpVelocity = 0f;
			mForwardVelocity = new Vector3 (0f, 0f, 0f);
			if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				mForwardVelocity = mForwardVelocity + mForward;
			}

			if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				mForwardVelocity = mForwardVelocity + mRight;
			}

			if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				mForwardVelocity = mForwardVelocity + mRight * (-1f);
			}

			if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				mForwardVelocity = mForwardVelocity + mForward * (-1f);
			}

			if (Input.GetKey (KeyCode.Space) && mJumpVelocity == 0f) {
				mForwardVelocity = (new Vector3(mForwardVelocity.x, 0f, mForwardVelocity.z)).normalized;
				mJumpVelocity += 10f;
			}

			mForwardVelocity.Normalize ();
		} else {
			//Debug.Log ("not grounded");
			// Fall velocity
			mJumpVelocity -= 40f * Time.deltaTime;
		}

		// Raycast in front
		bool mHitFront = Physics.Raycast (mBottomAnchor, mForwardVelocity.normalized, out mFront, 5f);
		Debug.DrawRay (mBottomAnchor, mForwardVelocity.normalized * 5f, Color.red);
		if (mHitFront) {
			if (mFront.distance - 0.25f < (mForwardVelocity * mSpeed * Time.deltaTime).magnitude) {
				mForwardVelocity = new Vector3 (0f, 0f, 0f);
			}
		}

		Vector3 velocity = (Vector3.up * mJumpVelocity + mForwardVelocity * mSpeed) * Time.deltaTime;
		// Prevent falling through the ground
		if (mIsGrounded || (mHitGround && mGround.distance - 0.25f >= -velocity.y) || (!mHitGround && transform.position.y > 5.25f)) {
			//Debug.Log ("valid");
			transform.position += velocity;
		} else if (mHitGround && mGround.distance - 0.25f < -velocity.y) {
			//Debug.Log ("adjust");
			transform.position += new Vector3 (velocity.x, 0.26f - mGround.distance, velocity.z);
		} else if (!mHitGround && transform.position.y < 5f) {
			//Debug.Log ("reposition");
			transform.position = new Vector3 (transform.position.x, mBaseOffset + Terrain.activeTerrain.SampleHeight (transform.position) + 0.5f, transform.position.z);
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
