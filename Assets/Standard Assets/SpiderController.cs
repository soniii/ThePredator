using UnityEngine;
using System.Collections;

public class SpiderController : MonoBehaviour {
	public float RunSpeed = 20f;

	private Animation mAnimation = null;

	private float rotationX = 0f;
	private float rotationY = 0f;
	private Quaternion originalRotation;

	// Use this for initialization
	void Start () {
		mAnimation = this.GetComponent<Animation> ();
		mAnimation ["Attack"].speed = 2f;
		mAnimation ["Attack_Right"].speed = 2f;
		mAnimation ["Attack_Left"].speed = 2f;
		originalRotation = transform.localRotation;
	}

	// Update is called once per frame
	void Update () {
		if (mAnimation == null) {
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
			mAnimation.CrossFadeQueued ("Attack_Right", 0.2f, QueueMode.PlayNow);
		}

		Vector3 forward =  new Vector3(0f, 0f, 0f);
		Vector3 left = new Vector3 (0f, 0f, 0f);

		Vector3 leftVector = Vector3.Cross (this.transform.up, this.transform.forward).normalized;
		if (Input.GetKey (KeyCode.W)) {
			forward = this.transform.forward;
			mAnimation.CrossFade ("Run");
		}

		if (Input.GetKey (KeyCode.D)) {
			left = leftVector;
			mAnimation.CrossFade ("Run");
		}

		if (Input.GetKey (KeyCode.A)) {
			left = leftVector * (-1f);
			mAnimation.CrossFade ("Run");
		}

		if (Input.GetKey (KeyCode.S)) {
			forward = this.transform.forward * (-1f);
			mAnimation.CrossFade ("Run");
		}

		this.transform.position += (forward + left).normalized * RunSpeed * Time.deltaTime;

		if (!mAnimation.IsPlaying("Attack_Right")) {
			// Idle
			if (!Input.anyKey) {
				mAnimation.CrossFade ("Idle");
			}
		}

		rotationX += Input.GetAxis ("Mouse X") * 5f;
		Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, this.transform.up);
		transform.localRotation = originalRotation * xQuaternion;
	}
}
