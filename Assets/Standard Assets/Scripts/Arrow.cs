using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public GameObject explosionObject;
	public float mSpeed = 100f;

	// Private
	private RaycastHit mHit;

	void OnTriggerEnter(Collider col) {
		Debug.Log ("sonydb: hit " + col.name + ", tag " + col.tag);
		if (col.tag == "Terrain" || col.tag == "Monster") {
			if (explosionObject && col.tag == "Monster") {
				GameObject newExplosion = (GameObject)Instantiate (explosionObject, transform.position, transform.rotation);
				Debug.Log ("sonydb: blood explosion");
				Destroy (newExplosion, 2f);
			}
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 nextPosition = transform.position + (transform.forward * mSpeed + Vector3.down * 10f * Time.deltaTime) * Time.deltaTime;

		if (Physics.Raycast (transform.position, transform.forward, out mHit, 5f)) {
			if ((mHit.collider.tag == "Terrain" || mHit.collider.tag == "Monster") &&
				mHit.distance < (nextPosition - transform.position).magnitude) {
				Debug.Log ("sonydb: raycast at work");
				nextPosition = mHit.point;
			}
		}
		transform.forward = (nextPosition - transform.position).normalized;
		transform.position = nextPosition;
	}
}
