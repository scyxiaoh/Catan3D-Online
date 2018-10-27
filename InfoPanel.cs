using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

	public Text infoText;
	public delegate void InfoDelegate();
	public InfoDelegate confirmationDelegate;
	public InfoDelegate rejectionDelegate;
	public Transform panel;
	public Button confirmationButton;
	public Button rejectionButton;

	private bool isWaiting = false; 

	class Message{
		public string text;
		public InfoDelegate cDelegate;
		public InfoDelegate rDelegate;
		public Message(string s, InfoDelegate cD, InfoDelegate rD){
			text = s;
			cDelegate = cD;
			rDelegate = rD;
		}
	}
	private Queue<Message> messageQueue = new Queue<Message>();

	public void RunConfirmationDelegate(){
		if (confirmationDelegate != null) {
			confirmationDelegate ();
			confirmationDelegate = null;
			rejectionDelegate = null;
		}
		isWaiting = false;
	}

	public void RunRejectionDelegate(){
		if (rejectionDelegate != null) {
			rejectionDelegate ();
			confirmationDelegate = null;
			rejectionDelegate = null;
		}
		isWaiting = false;
	}

	public void Update(){
		if (!isWaiting) {
			if (messageQueue.Count > 0) {
				panel.localPosition = Vector3.zero;
				Message m = messageQueue.Dequeue ();
				infoText.text = m.text;
				panel.gameObject.SetActive (true);
				confirmationDelegate = m.cDelegate;
				rejectionDelegate = m.rDelegate;
				isWaiting = true;
			}
		}
	}
	public void pushMessage(string s, InfoDelegate cD, InfoDelegate rD){
		messageQueue.Enqueue (new Message(s,cD,rD));
	}
}
