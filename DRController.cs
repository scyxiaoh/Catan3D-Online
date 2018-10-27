using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DRController : MonoBehaviour {

	public GameObject mainCamera;
	private Vector3 offset;
	private float angle;
	private float distance = 40f;



	void Update () {
		angle = mainCamera.transform.eulerAngles.y;
		offset = new Vector3 (Mathf.Sin(Mathf.Deg2Rad*angle) / Mathf.Sqrt (2), -1 / Mathf.Sqrt (2), Mathf.Cos(Mathf.Deg2Rad*angle) / Mathf.Sqrt (2)) * distance;
		transform.position = mainCamera.transform.position + offset;
		transform.eulerAngles = new Vector3 (0, angle, 0);
	}
}
	