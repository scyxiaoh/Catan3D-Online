using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Number : NetworkBehaviour
{
	[SyncVar]
	public int value = -1;


	private void Update()
	{
		if (value != -1)
		{
			this.transform.gameObject.GetComponent<TextMesh>().text = "" + value;
		}
	}
}
