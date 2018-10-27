using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EdgeUnit : MonoBehaviour
{

    public Player owner;
    public bool ifship;
	public int edgeIndex;
	public bool isClose;
	public bool isBuiltThisRound;

    // Use this for initialization
	void OnMouseEnter(){
		if (ifship) {
			gameObject.transform.localScale = new Vector3 (1.3f, 1.3f, 1.3f);
		}
	}

	void OnMouseExit(){
		if (ifship) {
			gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}
	}
	void OnMouseUp(){
		if (!ifship) {
			return;
		}
		Edge myEdge = gameObject.transform.parent.gameObject.GetComponent<Edge>();
		Transform panelShipAction = myEdge.gameBoard.panelActions.transform.GetChild (6);
		//disable all villages or knights selections
		myEdge.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myEdge.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myEdge.gameBoard.panelActions.transform.GetChild(5).GetChild(4).gameObject.GetComponent<Button>().onClick.Invoke();
		myEdge.gameBoard.panelActions.transform.GetChild(6).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myEdge.gameObject.transform.GetChild (1).gameObject.SetActive (true);		//show the selection mark (arrow)

		for (int i = 1; i < 7; i++) {
			myEdge.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (false);
		}
		panelShipAction.gameObject.SetActive (true);

		panelShipAction.GetChild (0).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		panelShipAction.GetChild (1).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		//panelShipAction.GetChild (0).gameObject.GetComponent<Button> ().onClick.AddListener ();
		panelShipAction.GetChild (1).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			myEdge.gameObject.transform.GetChild(1).gameObject.SetActive(false);
		});
	}

}
