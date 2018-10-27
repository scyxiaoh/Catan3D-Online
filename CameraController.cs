using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public bool locked = false;
	float speed = 10f;
	float rotationSpeed = 40f;
	float zoomSpeed = 10f;

	// Update is called once per frame
	void Update () {
		if (locked) {
			return;
		}
		// key ADSW - translation ref to camera
		if (Input.GetKey (KeyCode.RightArrow)|| Input.GetKey (KeyCode.D) ) {
			if (transform.position.x < 13f) {
				transform.Translate (new Vector3(1,0,0)*speed * Time.deltaTime);
			}
		}

		if (Input.GetKey (KeyCode.LeftArrow)|| Input.GetKey (KeyCode.A) ) {
			if (transform.position.x > 0.866f) {
				transform.Translate (new Vector3(-1,0,0)*speed * Time.deltaTime);
			}
		}

		if (Input.GetKey (KeyCode.DownArrow)|| Input.GetKey (KeyCode.S) ) {
			if (transform.position.z > (-transform.position.y/Mathf.Tan(Mathf.PI/3))) {
				transform.Translate (new Vector3(0,-Mathf.Sin(Mathf.PI/3),-Mathf.Cos(Mathf.PI/3))*speed * Time.deltaTime);
			}
		}

		if (Input.GetKey (KeyCode.UpArrow)|| Input.GetKey (KeyCode.W) ) {
			if (transform.position.z < (11.5 - transform.position.y/Mathf.Tan(Mathf.PI/3))) {
				transform.Translate (new Vector3(0,Mathf.Sin(Mathf.PI/3),Mathf.Cos(Mathf.PI/3))*speed * Time.deltaTime);
			}
		}
		/* disable rotation for now
		// key QE - rotation ref to world
		if (Input.GetKey (KeyCode.Q)) {
			transform.Rotate (new Vector3(0,1,0)* rotationSpeed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey (KeyCode.E)) {
			transform.Rotate (new Vector3(0,-1,0)* rotationSpeed * Time.deltaTime, Space.World);
		}
		*/
		// mouse scrollwheel - zooming
		if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
			if (transform.position.y > 2) {
				transform.Translate (new Vector3(0,0,1)*zoomSpeed * Time.deltaTime);
			}
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
			if (transform.position.y < 6) {
				transform.Translate (new Vector3(0,0,-1)*zoomSpeed * Time.deltaTime);
			}
		}
	}
}
