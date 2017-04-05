using UnityEngine;
using System.Collections;

public class AgentBalanceUtil : MonoBehaviour {
	// Public
	public Transform BackLeft;
	public Transform BackRight;
	public Transform FrontLeft;
	public Transform FrontRight;

	public float LerpRotationSpeed = 15f;
	public GameObject Target = null;

	// Private
	private NavMeshAgent mNavMeshAgent = null;

	private RaycastHit lr;
	private RaycastHit rr;
	private RaycastHit lf;
	private RaycastHit rf;

	private Vector3 tmpForward;
	private Vector3 tmpUp;

	void Start () {
		mNavMeshAgent = GetComponent<NavMeshAgent> ();
		mNavMeshAgent.SetDestination (transform.position + new Vector3 (10f, 0f, 0f));
		mNavMeshAgent.updateRotation = false;
		mNavMeshAgent.updatePosition = false;
	}

	void Update () {
		if (mNavMeshAgent != null) {
			tmpForward = transform.forward;
			Vector3 direction = (mNavMeshAgent.nextPosition - transform.position).normalized;

			if (direction.magnitude > 0f) {
				tmpForward = Vector3.Lerp(tmpForward, direction, Time.deltaTime * 20f);
			}

			// The forward vector should have been changed
			// Now calculate the side vector
			Physics.Raycast (BackLeft.position, Vector3.down, out lr, LayerMask.NameToLayer("Terrain"));
			Physics.Raycast (BackRight.position, Vector3.down, out rr, LayerMask.NameToLayer("Terrain"));
			Physics.Raycast (FrontLeft.position, Vector3.down, out lf, LayerMask.NameToLayer("Terrain"));
			Physics.Raycast (FrontRight.position, Vector3.down, out rf, LayerMask.NameToLayer("Terrain"));

			tmpUp = (lr.normal + rr.normal + lf.normal + rf.normal).normalized;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (tmpForward, tmpUp), Time.deltaTime * LerpRotationSpeed);

			// Set the new position to actually move the agent
			transform.position = mNavMeshAgent.nextPosition;
		}
	}
}