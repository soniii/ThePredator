using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float Speed = 1f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 leftVector = Vector3.Cross (this.transform.up, this.transform.forward);
		if (Input.GetKey (KeyCode.UpArrow)) {
			this.transform.position += this.transform.forward * Speed;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			this.transform.Rotate (new Vector3(0f, 1f, 0f));
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			this.transform.Rotate (new Vector3 (0f, -1f, 0f));
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			this.transform.position += this.transform.forward * (-Speed);
		}
	}
}
