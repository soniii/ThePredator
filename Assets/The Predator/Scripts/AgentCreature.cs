using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class AgentCreature : MonoBehaviour {

	// Public
	public enum State {
		none = 0,
		idle,
		walk,
		run,
		attack,
		horn,
		scream,
		hurt,
	};

	public float mWalkSpeed = 3f;
	public float mRunSpeed = 14f;
	public float MaxStateDuration = 10f;

	// Private
	private Animator mAnimator;
	private State mState;
	private NavMeshAgent mNavMeshAgent = null;
	private float mTimer;

	private List<GameObject> mTargets;
	private int mCurrentTarget;
	private Vector3 mChargingTarget;

	/* State variables - might be used for Animator Controller */
	private bool mHornCharging;
	private bool mHornAttacking;

	// Use this for initialization
	void Start () {
		mAnimator = GetComponent<Animator> ();
		mState = State.idle;
		mNavMeshAgent = GetComponent<NavMeshAgent> ();
		mTimer = 0f;
		mTargets = new List<GameObject> ();
		mCurrentTarget = 0;
		mChargingTarget = transform.position;

		// Animation states
		mHornCharging = false;
		mHornAttacking = false;
	}

	/*
	void OnTriggerEnter(Collider col) {
		//col.gameObject.SendMessage ("Stun", 5f, SendMessageOptions.DontRequireReceiver);
		if (col.tag != "Terrain" &&
			!mTargets.Contains(col.gameObject)) {
			Debug.Log ("sonydb: Monster Hit " + col.name);
			//Debug.Log ("sonydb: Add " + other.name);
			mTargets.Add (col.gameObject);
		}
	}

	void OnTriggerStay(Collider col) {
		//Debug.Log ("sonydb: something hit");
	}

	void OnTriggerExit(Collider col) {
	}
	*/

	float GetSpeed() {
		if (mNavMeshAgent != null) {
			return mNavMeshAgent.desiredVelocity.magnitude;
		}
		return 0f;
	}

	// Update is called once per frame
	void Update () {
		if (mNavMeshAgent != null) {
			//Debug.Log ("sonydb: Time = " + Time.time + ", timer = " + mTimer);

			//Debug.Log ("sonydb: Speed = " + GetSpeed ());
			switch (mState) {
			case State.idle:
				//Debug.Log ("sonydb: Idling..");
				if (Time.time - mTimer > MaxStateDuration) {
					if (mTargets.Count > 0 || Random.Range (0f, 1f) > 1f) {
						// Switch to Walk
						mTimer = Time.time;
						mState = State.walk;
						mNavMeshAgent.SetDestination (transform.position + new Vector3 (Random.Range (-50f, 50f), 0f, Random.Range (-50f, 50f)));
					} else if (false) {
						if (mCurrentTarget >= mTargets.Count) {
							mCurrentTarget = Random.Range (0, mTargets.Count);
						}

						// 3 types of attacks: Run->Bite, Scream, Run->Horn
						mState = State.horn;
					}
				}
				mAnimator.SetFloat ("Speed", GetSpeed ());
				break;
			case State.walk:
				//Debug.Log ("sonydb: Walking..");
				mNavMeshAgent.speed = mWalkSpeed;
				if (Time.time - mTimer > MaxStateDuration) {
					// Switch to Idle
					mTimer = Time.time;
					mState = State.idle;
					mNavMeshAgent.SetDestination (transform.position);
				}
				mAnimator.SetFloat ("Speed", GetSpeed ());
				break;
			case State.horn:
				//Debug.Log ("sonydb: Charging.. " + mNavMeshAgent.remainingDistance);
				if (!mHornAttacking) {
					//Debug.Log ("sonydb: Attack...");
					mNavMeshAgent.SetDestination (mTargets [mCurrentTarget].transform.position);
					mNavMeshAgent.speed = mRunSpeed;
					mHornAttacking = true;
				} else if (mHornAttacking && mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance) {
					//Debug.Log ("sonydb: switching..");
					mHornAttacking = false;
					mState = State.idle;
				}
				mAnimator.SetFloat ("Speed", GetSpeed());
				mAnimator.SetBool ("HornAttack", mHornAttacking);
				break;
			default:
				break;
			}

			// Play Animation



		}
	}
}
