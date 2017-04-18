using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Player Character Behaviors:
//- Player can rotate camera and move forward, backward, left, and right.
//- Player should interact with the terrain, obstacles, and monsters as follow:
//	- Player should never fall below the terrain.
//	- Player should not be able to move up terrain past a certain slope.
//	- Player should not be able to walk through obstacles and monsters.
//	- Player has less priority in terms of movemments than monsters.
//- Player is subjected to gravity.
//- Player, upon collision with monster, either: stops, gets moved out of the way lightly, or gets pushed out of the way at a distance.

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
	private float mStunned;

	// Collision count
	private List<GameObject> mObstacles;

	// Public facing function for interaction
	void Stun(float time) {
		Debug.Log ("sonydb: Stunned...");
		mStunned = time;
	}

	void OnTriggerEnter(Collider col) {
		Debug.Log ("sonydb: Player hit " + col.transform.root.name);
		if (col.gameObject.tag != "Terrain" && !mObstacles.Contains (col.gameObject)) {
			Debug.Log ("sonydb: new one");
		}
		/*
		if (col.tag != "Terrain" && !mObstacles.Contains(col.gameObject)) {
			Debug.Log ("sonydb: player hit " + col.name);
			mObstacles.Add (col.gameObject);
		}
		*/
	}

	void OnTriggerStay(Collider col) {
	}

	void OnTriggerExit(Collider col) {
		/*
		if (col.tag != "Terrain" && mObstacles.Contains(col.gameObject)) {
			Debug.Log ("sonydb: left " + col.name);
			mObstacles.Remove (col.gameObject);
		}
		*/
	}

	void Start () {
		// Initialize
		gameObject.tag = "Player";
		mRigidBody = GetComponent<Rigidbody> ();
		mRigidBody.constraints = RigidbodyConstraints.FreezeAll;

		mCameraAnchor = transform.Find ("CameraAnchor");
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

		mObstacles = new List<GameObject> ();
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

		if (mIsGrounded && mStunned <= 0f) {
			mJumpVelocity = 0f;
			mForwardVelocity = new Vector3 (0f, 0f, 0f);
			if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
				mForwardVelocity = mForwardVelocity + mForward;
			}

			if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
				mForwardVelocity = mForwardVelocity + mRight;
			}

			if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
				mForwardVelocity = mForwardVelocity + mRight * (-1f);
			}

			if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
				mForwardVelocity = mForwardVelocity + mForward * (-1f);
			}

			if (Input.GetKey (KeyCode.Space) && mJumpVelocity == 0f) {
				mForwardVelocity = (new Vector3 (mForwardVelocity.x, 0f, mForwardVelocity.z)).normalized;
				mJumpVelocity += 10f;
			}

			mForwardVelocity.Normalize ();
		} else if (!mIsGrounded) {
			//Debug.Log ("not grounded");
			// Fall velocity
			mJumpVelocity -= 40f * Time.deltaTime;
		} else if (mStunned > 0f) {
			mStunned -= Time.deltaTime;
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
			transform.position += velocity;
		} else if (mHitGround && mGround.distance - 0.25f < -velocity.y) {
			transform.position += new Vector3 (velocity.x, 0.26f - mGround.distance, velocity.z);
		} else if (!mHitGround && transform.position.y < 5f) {
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
