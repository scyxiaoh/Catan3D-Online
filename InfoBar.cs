using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : MonoBehaviour {

	public Text infoText;

	public void pushMessage(string s){
		gameObject.SetActive (true);
		infoText.text = s;
	}
}
